using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

// Servicio del Dashboard de Dirección / Dashboard de Departamento
// (22/sep/2026). Es una vista de SOLO LECTURA sobre los 24 módulos de
// captura ya existentes — no toca ninguna de sus tablas ni sus reglas de
// negocio, solo las consulta y agrega.
//
// Cada módulo tiene su propio método privado CalcularXxxAsync porque cada
// uno vive en una tabla distinta con su propio campo de fecha y su propia
// fórmula de KPI — no hay forma limpia de generalizar esto con un solo
// método genérico sin construir árboles de expresión a mano (Expression.*),
// que sería más difícil de mantener que 25 métodos explícitos y cortos.
// Sigue el mismo criterio que el resto del proyecto: nunca llamar un
// método auxiliar DENTRO de un .Where()/.Select() sobre un IQueryable (EF
// Core no lo puede traducir a SQL) — cada consulta se arma en línea.
public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _contexto;

    public DashboardService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<DashboardDireccionDto?> ObtenerDireccionAsync(
        int direccionId, DateTime? fechaDesde, DateTime? fechaHasta, List<int>? ubicacionIds)
    {
        var direccion = await _contexto.Direcciones
            .AsNoTracking()
            .Include(d => d.Departamentos)
                .ThenInclude(dep => dep.Areas)
                    .ThenInclude(a => a.AreaUbicaciones)
            .FirstOrDefaultAsync(d => d.Id == direccionId);

        if (direccion is null)
        {
            return null;
        }

        // fechaHasta se trata como fin de día INCLUSIVE (si el usuario manda
        // 2026-09-22, se incluyen registros de ese día completo). Para no
        // depender de que el frontend mande hora 23:59:59, se convierte aquí
        // a un límite EXCLUSIVO sumando 1 día, y cada consulta compara con
        // "<" contra ese límite.
        var hastaExclusiva = fechaHasta?.Date.AddDays(1);

        var departamentos = new List<DashboardDepartamentoDto>();
        foreach (var departamento in direccion.Departamentos.OrderBy(d => d.Nombre))
        {
            departamentos.Add(await ConstruirDepartamentoDtoAsync(departamento, fechaDesde, hastaExclusiva, ubicacionIds));
        }

        return new DashboardDireccionDto
        {
            DireccionId = direccion.Id,
            Nombre = direccion.Nombre,
            Departamentos = departamentos,
        };
    }

    public async Task<DashboardDepartamentoDto?> ObtenerDepartamentoAsync(
        int departamentoId, DateTime? fechaDesde, DateTime? fechaHasta, List<int>? ubicacionIds)
    {
        var departamento = await _contexto.Departamentos
            .AsNoTracking()
            .Include(d => d.Areas)
                .ThenInclude(a => a.AreaUbicaciones)
            .FirstOrDefaultAsync(d => d.Id == departamentoId);

        if (departamento is null)
        {
            return null;
        }

        var hastaExclusiva = fechaHasta?.Date.AddDays(1);
        return await ConstruirDepartamentoDtoAsync(departamento, fechaDesde, hastaExclusiva, ubicacionIds);
    }

    public async Task<int?> ObtenerDireccionIdDeDepartamentoAsync(int departamentoId)
    {
        var departamento = await _contexto.Departamentos
            .AsNoTracking()
            .Where(d => d.Id == departamentoId)
            .Select(d => new { d.DireccionId })
            .FirstOrDefaultAsync();

        return departamento?.DireccionId;
    }

    private async Task<DashboardDepartamentoDto> ConstruirDepartamentoDtoAsync(
        Departamento departamento, DateTime? fechaDesde, DateTime? hastaExclusiva, List<int>? ubicacionIdsFiltro)
    {
        var areas = new List<DashboardAreaDto>();
        foreach (var area in departamento.Areas.OrderBy(a => a.Nombre))
        {
            areas.Add(await ConstruirAreaDtoAsync(area, fechaDesde, hastaExclusiva, ubicacionIdsFiltro));
        }

        return new DashboardDepartamentoDto
        {
            DepartamentoId = departamento.Id,
            Nombre = departamento.Nombre,
            Areas = areas,
        };
    }

    private async Task<DashboardAreaDto> ConstruirAreaDtoAsync(
        Area area, DateTime? fechaDesde, DateTime? hastaExclusiva, List<int>? ubicacionIdsFiltro)
    {
        // Modulos.Todos (Models/Modulos.cs) es la fuente de verdad de a qué
        // Área pertenece cada módulo — no se infiere de otra forma.
        var modulosDelArea = Modulos.Todos.Where(m => m.AreaId == area.Id).ToList();

        if (modulosDelArea.Count == 0)
        {
            // Área sin ningún módulo de captura construido todavía (ej. RH,
            // Proyectos, Seguridad e Higiene): se muestra igual, marcada
            // como "sin datos" en vez de omitirse.
            return new DashboardAreaDto
            {
                AreaId = area.Id,
                Nombre = area.Nombre,
                TieneModulosCaptura = false,
                Modulos = new List<DashboardModuloDto>(),
            };
        }

        // Todas las Plantas (AreaUbicacionId) que pertenecen a esta Área.
        var plantasDelArea = area.AreaUbicaciones.Select(au => au.Id).ToList();

        // ubicacionIdsFiltro trae Ids de Ubicación FÍSICA (catálogo
        // Ubicacion), no AreaUbicacionId — un mismo AreaUbicacionId solo
        // existe dentro de UN Área, así que no se puede usar tal cual para
        // filtrar todas las Áreas de este dashboard. Aquí se resuelve, para
        // ESTA Área en particular, cuáles de sus AreaUbicacionId
        // corresponden a alguna de las Ubicaciones seleccionadas (puede no
        // haber ninguno: esta Área quizá no tiene dada de alta ninguna de
        // las Plantas elegidas, y entonces no hay nada que mostrar).
        List<int>? areaUbicacionIdsResueltos = null;
        var filtroFueraDeArea = false;
        if (ubicacionIdsFiltro is { Count: > 0 })
        {
            areaUbicacionIdsResueltos = await _contexto.AreaUbicaciones
                .Where(au => au.AreaId == area.Id && ubicacionIdsFiltro.Contains(au.UbicacionId))
                .Select(au => au.Id)
                .ToListAsync();

            if (areaUbicacionIdsResueltos.Count == 0)
            {
                // Ninguna de las Ubicaciones seleccionadas está dada de
                // alta en esta Área: da 0/null en vez de ignorar el filtro
                // silenciosamente.
                filtroFueraDeArea = true;
            }
        }

        var modulosDto = new List<DashboardModuloDto>();
        foreach (var definicion in modulosDelArea)
        {
            DashboardModuloDto moduloDto;
            if (filtroFueraDeArea)
            {
                moduloDto = new DashboardModuloDto { Clave = definicion.Clave, Nombre = definicion.Nombre, Total = 0, Kpi = null, KpiEtiqueta = null };
            }
            else
            {
                var (total, kpi, kpiEtiqueta) = await CalcularModuloAsync(
                    definicion.Clave, plantasDelArea, areaUbicacionIdsResueltos, fechaDesde, hastaExclusiva);
                moduloDto = new DashboardModuloDto { Clave = definicion.Clave, Nombre = definicion.Nombre, Total = total, Kpi = kpi, KpiEtiqueta = kpiEtiqueta };
            }

            modulosDto.Add(moduloDto);
        }

        return new DashboardAreaDto
        {
            AreaId = area.Id,
            Nombre = area.Nombre,
            TieneModulosCaptura = true,
            Modulos = modulosDto,
        };
    }

    private Task<(decimal Total, decimal? Kpi, string? KpiEtiqueta)> CalcularModuloAsync(
        string clave, List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        return clave switch
        {
            Modulos.ItTickets => CalcularItTicketsAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItInventario => CalcularItInventarioAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItIncidentes => CalcularItIncidentesAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItRespaldos => CalcularItRespaldosAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItPlaticas => CalcularItPlaticasAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItAuditorias => CalcularItAuditoriasAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItDispServidores => CalcularItDispServidoresAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItDispRed => CalcularItDispRedAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItAlmacenamiento => CalcularItAlmacenamientoAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.ItActualizacionesEquipos => CalcularItActualizacionesEquiposAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegRecorridos => CalcularSegRecorridosAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegCredencializacion => CalcularSegCredencializacionAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegTestConsignas => CalcularSegTestConsignasAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegAlcoholimetria => CalcularSegAlcoholimetriaAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegDopings => CalcularSegDopingsAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegLockers => CalcularSegLockersAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegValesSalida => CalcularSegValesSalidaAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegEstacionamiento => CalcularSegEstacionamientoAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegReunionesProveedor => CalcularSegReunionesProveedorAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegEvaluacionesVigilancia => CalcularSegEvaluacionesVigilanciaAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.AdmMantenimientoVehicular => CalcularAdmMantenimientoVehicularAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.AdmTiempoRespuestaResolucion => CalcularAdmTiempoRespuestaResolucionAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.AdmDisponibilidadAbastecimiento => CalcularAdmDisponibilidadAbastecimientoAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.AdmCumplimientoDocumentacion => CalcularAdmCumplimientoDocumentacionAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.AdmCumplimientoPrograma => CalcularAdmCumplimientoProgramaAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegHigObservaciones => CalcularSegHigObservacionesAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegHigSafetyWalks => CalcularSegHigSafetyWalksAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegHigCumplimientoEpp => CalcularSegHigCumplimientoEppAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegHigEstatusLegal => CalcularSegHigEstatusLegalAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegHigBrigadas => CalcularSegHigBrigadasAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegHigEvaluacionesProveedores => CalcularSegHigEvaluacionesProveedoresAsync(plantas, filtro, desde, hastaExclusiva),
            Modulos.SegHigAccidentes => CalcularSegHigAccidentesAsync(plantas, filtro, desde, hastaExclusiva),
            // No debería ocurrir (clave viene de Modulos.Todos), pero se
            // cubre por si se agrega un módulo nuevo sin su caso aquí.
            _ => Task.FromResult<(decimal, decimal?, string?)>((0, null, null)),
        };
    }

    // --- IT ---

    private async Task<(decimal, decimal?, string?)> CalcularItTicketsAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Tickets.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(t => idsFiltro.Contains(t.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(t => t.FechaCreacion >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(t => t.FechaCreacion < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // Estado es texto libre (mesa de ayuda importada); se compara con
        // Contains en vez de una lista fija de valores.
        var resueltos = await query.CountAsync(t => t.Estado.ToLower().Contains("resuelto") || t.Estado.ToLower().Contains("cerrado"));
        return (total, Math.Round((decimal)resueltos / total * 100, 1), "% Resueltos/Cerrados");
    }

    private async Task<(decimal, decimal?, string?)> CalcularItInventarioAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.InventarioEquipos.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(e => idsFiltro.Contains(e.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(e => e.FechaAlta >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(e => e.FechaAlta < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var activos = await query.CountAsync(e => e.Estado == "Activo");
        return (total, Math.Round((decimal)activos / total * 100, 1), "% Equipos activos");
    }

    // Excepción deliberada (ver requerimiento): el KPI "días sin incidentes"
    // IGNORA el filtro de fecha (fechaDesde/fechaHasta) — no tendría sentido
    // acotar por rango de fecha algo que mide "cuánto llevamos sin uno
    // nuevo". Solo respeta el filtro de Ubicación. El Total sí respeta el
    // filtro de fecha normalmente.
    private async Task<(decimal, decimal?, string?)> CalcularItIncidentesAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var queryPlanta = _contexto.IncidentesCriticos.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        queryPlanta = queryPlanta.Where(i => idsFiltro.Contains(i.AreaUbicacionId));

        var queryTotal = queryPlanta;
        if (desde.HasValue) queryTotal = queryTotal.Where(i => i.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) queryTotal = queryTotal.Where(i => i.Fecha < hastaExclusiva.Value);
        var total = await queryTotal.CountAsync();

        var maxFecha = await queryPlanta.MaxAsync(i => (DateTime?)i.Fecha);
        if (maxFecha is null)
        {
            return (total, null, "Sin incidentes registrados");
        }

        var diasSinIncidentes = (decimal)(DateTime.UtcNow.Date - maxFecha.Value.Date).TotalDays;
        return (total, diasSinIncidentes, "Días sin incidentes");
    }

    private async Task<(decimal, decimal?, string?)> CalcularItRespaldosAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Respaldos.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(r => idsFiltro.Contains(r.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(r => r.FechaRespaldo >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(r => r.FechaRespaldo < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var completados = await query.CountAsync(r => r.Estado == "Completado");
        return (total, Math.Round((decimal)completados / total * 100, 1), "% Completados");
    }

    // Platica.AreaUbicacionId es nullable: null = envío a toda la empresa.
    // Sin filtro de Ubicación activo se incluyen los null junto con los de
    // esta Área; con filtro activo se excluyen los null (un filtro de
    // Planta específica no debe traer "toda la empresa").
    private async Task<(decimal, decimal?, string?)> CalcularItPlaticasAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Platicas.AsNoTracking().AsQueryable();
        query = filtro is { Count: > 0 }
            ? query.Where(p => p.AreaUbicacionId != null && filtro.Contains(p.AreaUbicacionId.Value))
            : query.Where(p => p.AreaUbicacionId == null || plantas.Contains(p.AreaUbicacionId!.Value));
        if (desde.HasValue) query = query.Where(p => p.FechaEnvio >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(p => p.FechaEnvio < hastaExclusiva.Value);

        var total = await query.CountAsync();
        return (total, null, null);
    }

    private async Task<(decimal, decimal?, string?)> CalcularItAuditoriasAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.AuditoriasEquipo.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(a => idsFiltro.Contains(a.AreaUbicacionId));
        // FechaRealizada es nullable: un registro sin fecha realizada
        // todavía no se puede evaluar contra el rango, así que no se excluye.
        if (desde.HasValue) query = query.Where(a => a.FechaRealizada == null || a.FechaRealizada >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(a => a.FechaRealizada == null || a.FechaRealizada < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var realizadas = await query.CountAsync(a => a.Estado == "Realizada");
        return (total, Math.Round((decimal)realizadas / total * 100, 1), "% Realizadas");
    }

    private async Task<(decimal, decimal?, string?)> CalcularItDispServidoresAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.DisponibilidadServidores.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(r => idsFiltro.Contains(r.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(r => r.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(r => r.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // Se castea a decimal? antes del promedio para que una colección
        // vacía regrese null en vez de lanzar excepción (Average sobre
        // decimal no nullable lanza si no hay elementos).
        var promedio = await query.Select(r => (decimal?)r.DisponibilidadPorcentaje).AverageAsync();
        return (total, promedio.HasValue ? Math.Round(promedio.Value, 1) : null, "Disponibilidad promedio (%)");
    }

    private async Task<(decimal, decimal?, string?)> CalcularItDispRedAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.DisponibilidadRedes.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(r => idsFiltro.Contains(r.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(r => r.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(r => r.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var promedio = await query.Select(r => (decimal?)r.DisponibilidadPorcentaje).AverageAsync();
        return (total, promedio.HasValue ? Math.Round(promedio.Value, 1) : null, "Disponibilidad promedio (%)");
    }

    private async Task<(decimal, decimal?, string?)> CalcularItAlmacenamientoAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.AlmacenamientoServidores.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(r => idsFiltro.Contains(r.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(r => r.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(r => r.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // KPI calculado solo sobre el subconjunto con Umbral configurado.
        var conUmbral = await query.CountAsync(r => r.Umbral != null);
        if (conUmbral == 0) return (total, null, null);

        var sobreUmbral = await query.CountAsync(r => r.Umbral != null && r.AlmacenamientoUtilizadoPorcentaje > r.Umbral);
        return (total, Math.Round((decimal)sobreUmbral / conUmbral * 100, 1), "% Sobre el umbral");
    }

    private async Task<(decimal, decimal?, string?)> CalcularItActualizacionesEquiposAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.ActualizacionesEquiposCriticos.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(a => idsFiltro.Contains(a.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(a => a.FechaRegistro >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(a => a.FechaRegistro < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var aplicadas = await query.CountAsync(a => a.SeAplico);
        return (total, Math.Round((decimal)aplicadas / total * 100, 1), "% Aplicadas");
    }

    // --- Seguridad Patrimonial ---

    private async Task<(decimal, decimal?, string?)> CalcularSegRecorridosAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Recorridos.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(r => idsFiltro.Contains(r.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(r => r.Fecha == null || r.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(r => r.Fecha == null || r.Fecha < hastaExclusiva.Value);

        return (await query.CountAsync(), null, null);
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegCredencializacionAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Credencializaciones.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(c => idsFiltro.Contains(c.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(c => c.FechaHoraEntrada == null || c.FechaHoraEntrada >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(c => c.FechaHoraEntrada == null || c.FechaHoraEntrada < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var conIne = await query.CountAsync(c => c.Identificacion != null && c.Identificacion.ToLower().Contains("ine"));
        return (total, Math.Round((decimal)conIne / total * 100, 1), "% Identificados con INE");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegTestConsignasAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.TestConsignas.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(t => idsFiltro.Contains(t.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(t => t.Fecha == null || t.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(t => t.Fecha == null || t.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var aprobados = await query.CountAsync(t => t.ResultadoTest == "Aprobado");
        return (total, Math.Round((decimal)aprobados / total * 100, 1), "% Aprobados");
    }

    // Total = SUM(Cantidad), no COUNT de filas: cada fila es un agregado por
    // Planta+Fecha+Turno, no una aplicación individual (a diferencia de
    // Dopings). El usuario confirmó que quiere "número de aplicaciones".
    private async Task<(decimal, decimal?, string?)> CalcularSegAlcoholimetriaAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Alcoholimetrias.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(a => idsFiltro.Contains(a.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(a => a.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(a => a.Fecha < hastaExclusiva.Value);

        // Sum sobre una secuencia vacía regresa 0 (a diferencia de Average),
        // no hace falta chequear total de filas primero.
        var totalCantidad = await query.SumAsync(a => (decimal)a.Cantidad);
        return (totalCantidad, null, null);
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegDopingsAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Dopings.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(d => idsFiltro.Contains(d.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(d => d.Fecha == null || d.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(d => d.Fecha == null || d.Fecha < hastaExclusiva.Value);

        return (await query.CountAsync(), null, null);
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegLockersAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Lockers.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(l => idsFiltro.Contains(l.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(l => l.Fecha == null || l.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(l => l.Fecha == null || l.Fecha < hastaExclusiva.Value);

        return (await query.CountAsync(), null, null);
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegValesSalidaAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.ValesSalida.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(v => idsFiltro.Contains(v.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(v => v.FechaVale == null || v.FechaVale >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(v => v.FechaVale == null || v.FechaVale < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var cerrados = await query.CountAsync(v => v.Estado == "Cerrado");
        return (total, Math.Round((decimal)cerrados / total * 100, 1), "% Cerrados");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegEstacionamientoAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Estacionamientos.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(e => idsFiltro.Contains(e.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(e => e.FechaRegistro == null || e.FechaRegistro >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(e => e.FechaRegistro == null || e.FechaRegistro < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var vigente = await query.CountAsync(e => e.EstatusDocumentacion != null && e.EstatusDocumentacion.ToLower().Contains("vigente"));
        return (total, Math.Round((decimal)vigente / total * 100, 1), "% Documentación vigente");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegReunionesProveedorAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.ReunionesProveedor.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(r => idsFiltro.Contains(r.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(r => r.Fecha == null || r.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(r => r.Fecha == null || r.Fecha < hastaExclusiva.Value);

        return (await query.CountAsync(), null, null);
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegEvaluacionesVigilanciaAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.EvaluacionesVigilancia.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(e => idsFiltro.Contains(e.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(e => e.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(e => e.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var promedio = await query.Select(e => (decimal?)e.Total).AverageAsync();
        return (total, promedio.HasValue ? Math.Round(promedio.Value, 1) : null, "Promedio general");
    }

    // --- Administración ---

    private async Task<(decimal, decimal?, string?)> CalcularAdmMantenimientoVehicularAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.MantenimientosVehiculares.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(m => idsFiltro.Contains(m.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(m => m.Fecha == null || m.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(m => m.Fecha == null || m.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var vencido = await query.CountAsync(m =>
            m.ProximoServicio != null && m.KilometrajeActual != null && (m.ProximoServicio.Value - m.KilometrajeActual.Value) < 0);
        return (total, Math.Round((decimal)vencido / total * 100, 1), "% Servicio vencido");
    }

    private async Task<(decimal, decimal?, string?)> CalcularAdmTiempoRespuestaResolucionAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.TiemposRespuestaResolucion.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(t => idsFiltro.Contains(t.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(t => t.FechaRecepcion >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(t => t.FechaRecepcion < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // KPI calculado solo sobre folios ya cerrados (con CumplimientoSla
        // ya calculado) — los que siguen abiertos todavía no tienen este dato.
        var conDato = await query.CountAsync(t => t.CumplimientoSla != null);
        if (conDato == 0) return (total, null, null);

        var cumplen = await query.CountAsync(t => t.CumplimientoSla == true);
        return (total, Math.Round((decimal)cumplen / conDato * 100, 1), "% Cumplimiento SLA");
    }

    private async Task<(decimal, decimal?, string?)> CalcularAdmDisponibilidadAbastecimientoAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.DisponibilidadesAbastecimiento.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(d => idsFiltro.Contains(d.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(d => d.FechaEntrega >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(d => d.FechaEntrega < hastaExclusiva.Value);

        return (await query.CountAsync(), null, null);
    }

    private async Task<(decimal, decimal?, string?)> CalcularAdmCumplimientoDocumentacionAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.CumplimientosDocumentacion.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(c => idsFiltro.Contains(c.AreaUbicacionId));
        // El filtro de rango de fecha usa FechaInicio; el KPI de "por
        // vencer" abajo usa FechaVencimiento — son campos independientes.
        if (desde.HasValue) query = query.Where(c => c.FechaInicio >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(c => c.FechaInicio < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // "(FechaVencimiento - hoy).TotalDays < 30" (incluye ya vencidos,
        // días negativos cuentan) es equivalente a "FechaVencimiento <
        // hoy + 30 días" — se escribe así porque SÍ se traduce a SQL
        // limpiamente (comparar dos fechas), a diferencia de restar fechas
        // y leer .TotalDays dentro de un Where sobre IQueryable.
        var limite = DateTime.UtcNow.Date.AddDays(30);
        var porVencer = await query.CountAsync(c => c.FechaVencimiento < limite);
        return (total, Math.Round((decimal)porVencer / total * 100, 1), "% Por vencer (<30 días)");
    }

    private async Task<(decimal, decimal?, string?)> CalcularAdmCumplimientoProgramaAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.CumplimientosPrograma.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(c => idsFiltro.Contains(c.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(c => c.FechaReporte >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(c => c.FechaReporte < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // Average sobre una columna decimal? ya ignora los null por sí solo
        // y regresa null si no queda ningún valor — no hace falta filtrar
        // el subconjunto a mano.
        var promedio = await query.Select(c => c.CumplimientoGeneralPorcentaje).AverageAsync();
        return (total, promedio.HasValue ? Math.Round(promedio.Value, 1) : null, "Cumplimiento promedio (%)");
    }

    // --- Seguridad e Higiene (Fase 3.6) ---

    private async Task<(decimal, decimal?, string?)> CalcularSegHigObservacionesAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.ObservacionesSeguridad.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(o => idsFiltro.Contains(o.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(o => o.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(o => o.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var cerradas = await query.CountAsync(o => o.Estado == "Cerrado");
        return (total, Math.Round((decimal)cerradas / total * 100, 1), "% Cerradas");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegHigSafetyWalksAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.SafetyWalks.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(s => idsFiltro.Contains(s.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(s => s.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(s => s.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var promedio = await query.Select(s => s.Cumplimiento).AverageAsync();
        return (total, Math.Round(promedio, 1), "Cumplimiento promedio (%)");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegHigCumplimientoEppAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.CumplimientosEpp.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(c => idsFiltro.Contains(c.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(c => c.FechaRegistro >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(c => c.FechaRegistro < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // Solo se puede evaluar "abastecido" en filas que capturaron ambos
        // valores (Existencias y Minimo); las demás se excluyen del % pero
        // sí cuentan en el Total.
        var evaluables = query.Where(c => c.Existencias != null && c.Minimo != null);
        var evaluablesTotal = await evaluables.CountAsync();
        if (evaluablesTotal == 0) return (total, null, null);

        var abastecidos = await evaluables.CountAsync(c => c.Existencias!.Value >= c.Minimo!.Value);
        return (total, Math.Round((decimal)abastecidos / evaluablesTotal * 100, 1), "% Abastecido");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegHigEstatusLegalAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.EstatusLegalesPlanta.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(e => idsFiltro.Contains(e.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(e => e.UltimaFechaRealizacion >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(e => e.UltimaFechaRealizacion < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        // Estatus es texto libre (a petición explícita del usuario), se
        // compara con Contains en vez de una lista fija de valores.
        var cumple = await query.CountAsync(e => e.Estatus.ToLower().Contains("cumple"));
        return (total, Math.Round((decimal)cumple / total * 100, 1), "% Cumple");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegHigBrigadasAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.Brigadas.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(b => idsFiltro.Contains(b.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(b => b.Fecha >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(b => b.Fecha < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var activos = await query.CountAsync(b => b.Estado == "Activo");
        return (total, Math.Round((decimal)activos / total * 100, 1), "% Activos");
    }

    private async Task<(decimal, decimal?, string?)> CalcularSegHigEvaluacionesProveedoresAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var query = _contexto.EvaluacionesProveedorSegHigiene.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        query = query.Where(e => idsFiltro.Contains(e.AreaUbicacionId));
        if (desde.HasValue) query = query.Where(e => e.Mes >= desde.Value);
        if (hastaExclusiva.HasValue) query = query.Where(e => e.Mes < hastaExclusiva.Value);

        var total = await query.CountAsync();
        if (total == 0) return (0, null, null);

        var promedio = await query.Select(e => e.Cumplimiento).AverageAsync();
        return (total, Math.Round(promedio, 1), "Cumplimiento promedio (%)");
    }

    // Mismo criterio que CalcularItIncidentesAsync: el KPI "días sin
    // accidentes" IGNORA el filtro de fecha (no tendría sentido acotar por
    // rango algo que mide "cuánto llevamos sin uno nuevo"). Solo respeta el
    // filtro de Ubicación. El Total sí respeta el filtro de fecha normalmente.
    private async Task<(decimal, decimal?, string?)> CalcularSegHigAccidentesAsync(List<int> plantas, List<int>? filtro, DateTime? desde, DateTime? hastaExclusiva)
    {
        var queryPlanta = _contexto.AccidentesTrabajo.AsNoTracking().AsQueryable();
        var idsFiltro = (filtro is { Count: > 0 }) ? filtro : plantas;
        queryPlanta = queryPlanta.Where(a => idsFiltro.Contains(a.AreaUbicacionId));

        var queryTotal = queryPlanta;
        if (desde.HasValue) queryTotal = queryTotal.Where(a => a.FechaOcurrido >= desde.Value);
        if (hastaExclusiva.HasValue) queryTotal = queryTotal.Where(a => a.FechaOcurrido < hastaExclusiva.Value);
        var total = await queryTotal.CountAsync();

        var maxFecha = await queryPlanta.MaxAsync(a => (DateTime?)a.FechaOcurrido);
        if (maxFecha is null)
        {
            return (total, null, "Sin accidentes registrados");
        }

        var diasSinAccidentes = (decimal)(DateTime.UtcNow.Date - maxFecha.Value.Date).TotalDays;
        return (total, diasSinAccidentes, "Días sin accidentes");
    }
}
