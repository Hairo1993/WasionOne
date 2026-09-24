using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class TicketService : ITicketService
{
    // Fijo mientras solo exista el piloto de IT (AreaId = 3, sembrado en
    // ApplicationDbContext). Cuando se abra el módulo de Tickets a otras
    // áreas, esto debe volverse un parámetro.
    private const int AREA_ID_IT = 3;

    // Mapea el nombre "lógico" de cada campo al encabezado normalizado que
    // trae el Excel exportado del sistema de mesa de ayuda (ver
    // NormalizarEncabezado). Se conservan aquí, en un solo lugar, los
    // textos exactos de las columnas para que un cambio de encabezado en
    // el archivo de origen solo se corrija en este diccionario.
    private static readonly Dictionary<string, string> ColumnasTicket = new()
    {
        ["AnyDeskEquipo"] = "anydeskequipo",
        ["EstadoAprobacion"] = "estadodeaprobacion",
        ["TipoAsociacion"] = "tipodeasociacion",
        ["Categoria"] = "categoria",
        ["HoraCierre"] = "horadecierre",
        ["HoraCreacion"] = "horadecreacion",
        ["Departamento"] = "departamento",
        ["Descripcion"] = "descripcion",
        ["IdTicket"] = "iddelticket",
        ["HoraVencimiento"] = "horadevencimiento",
        ["TiempoPrimeraRespuestaHoras"] = "tiempodeprimerarespuestaenhoras",
        ["EstadoPrimeraRespuesta"] = "estadodeprimerarespuesta",
        ["TiempoInicialRespuesta"] = "tiempoinicialderespuesta",
        ["Grupo"] = "grupo",
        ["Impacto"] = "impacto",
        ["InteraccionesCliente"] = "interaccionesdelcliente",
        ["Elemento"] = "elemento",
        ["Ubicacion"] = "ubicacion",
        // "Intercciones" (sin la primera "a") es el nombre real de la
        // columna en el reporte de origen, se conserva tal cual.
        ["InteraccionesAgente"] = "interccionesdelagente",
        ["Prioridad"] = "prioridad",
        ["CorreoSolicitante"] = "correoelectronicodelsolicitante",
        ["UbicacionSolicitante"] = "ubicaciondelsolicitante",
        ["NombreSolicitante"] = "nombredelsolicitante",
        ["SolicitanteVip"] = "solicitantevip",
        ["NotaResolucion"] = "notaderesolucion",
        ["EstadoResolucion"] = "estadoderesolucion",
        // "in horas" (en vez de "en horas") es el texto real del reporte.
        ["TiempoResolucionHoras"] = "tiempoderesolucioninhoras",
        ["HoraResolucion"] = "horaderesolucion",
        ["Agente"] = "agente",
        ["Origen"] = "origen",
        ["Estado"] = "estado",
        ["Subcategoria"] = "subcategoria",
        ["Asunto"] = "asunto",
        ["ResultadoEncuesta"] = "resultadodeencuestas",
        ["Etiquetas"] = "etiquetas",
        ["Tipo"] = "tipo",
        ["RegistroTiempo"] = "registrodetiempo",
        ["HoraUltimaActualizacion"] = "horadeultimaactualizacion",
        ["Urgencia"] = "urgencia",
    };

    // Plantilla descargable (24/sep/2026): mismo orden que "ColumnasTicket"
    // de arriba, con el encabezado "bonito" y un valor de ejemplo por
    // columna. Este módulo es legacy (no usa ExcelFilaLectora ni el
    // diccionario "Columnas" estándar de los demás servicios), así que los
    // encabezados se derivaron de "ColumnasTicket" y de las propiedades de
    // TicketDto — NO del texto .ayuda del componente (que no lista las 39
    // columnas). Dos encabezados preservan intencionalmente errores
    // ortográficos que ya existen en el reporte de origen y de los que
    // depende el matching de ColumnasTicket: "Intercciones del Agente"
    // (falta la primera "a") y "Tiempo de Resolución (in Horas)" ("in" en
    // vez de "en") — si se "corrigen", la plantilla descargada dejaría de
    // reconocerse al reimportarla.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("AnyDesk (Equipo)", "PC-00123"),
        new("Estado de Aprobación", "Aprobado"),
        new("Tipo de Asociación", "Incidente"),
        new("Categoría", "Hardware"),
        new("Hora de Cierre", "24/09/2026 16:00"),
        new("Hora de Creación", "20/09/2026 08:15"),
        new("Departamento", "Sistemas"),
        new("Descripción", "No enciende el equipo de cómputo"),
        new("ID del Ticket", "TKT-0001"),
        new("Hora de Vencimiento", "25/09/2026 08:15"),
        new("Tiempo de Primera Respuesta (en Horas)", "1.5"),
        new("Estado de Primera Respuesta", "Cumplido"),
        new("Tiempo Inicial de Respuesta", "30 min"),
        new("Grupo", "Soporte Nivel 1"),
        new("Impacto", "Medio"),
        new("Interacciones del Cliente", "2"),
        new("Elemento", "Laptop"),
        new("Ubicación", "Planta 1"),
        new("Intercciones del Agente", "3"),
        new("Prioridad", "Media"),
        new("Correo Electrónico del Solicitante", "juan.perez@wasion.com"),
        new("Ubicación del Solicitante", "Planta 1"),
        new("Nombre del Solicitante", "Juan Pérez"),
        new("Solicitante VIP", "No"),
        new("Nota de Resolución", "Se reinstaló el sistema operativo"),
        new("Estado de Resolución", "Resuelto"),
        new("Tiempo de Resolución (in Horas)", "4.5"),
        new("Hora de Resolución", "24/09/2026 14:00"),
        new("Agente", "María López"),
        new("Origen", "Correo electrónico"),
        new("Estado", "Abierto"),
        new("Subcategoría", "Equipo de cómputo"),
        new("Asunto", "Equipo no enciende"),
        new("Resultado de Encuestas", "Satisfecho"),
        new("Etiquetas", "hardware, urgente"),
        new("Tipo", "Incidente"),
        new("Registro de Tiempo", "2"),
        new("Hora de Última Actualización", "24/09/2026 09:00"),
        new("Urgencia", "Media"),
    };

    private readonly ApplicationDbContext _contexto;

    public TicketService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("IT Tickets", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<TicketDto>> ObtenerTicketsAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Tickets.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(t => t.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(t => t.FechaCreacion)
            .Select(t => new TicketDto
            {
                Id = t.Id,
                IdTicketOrigen = t.IdTicketOrigen,
                AreaUbicacionId = t.AreaUbicacionId,
                Asunto = t.Asunto,
                Descripcion = t.Descripcion,
                Categoria = t.Categoria,
                Subcategoria = t.Subcategoria,
                Tipo = t.Tipo,
                TipoAsociacion = t.TipoAsociacion,
                Etiquetas = t.Etiquetas,
                Prioridad = t.Prioridad,
                Urgencia = t.Urgencia,
                Impacto = t.Impacto,
                Estado = t.Estado,
                EstadoAprobacion = t.EstadoAprobacion,
                EstadoResolucion = t.EstadoResolucion,
                EstadoPrimeraRespuesta = t.EstadoPrimeraRespuesta,
                Grupo = t.Grupo,
                Agente = t.Agente,
                Origen = t.Origen,
                NombreSolicitante = t.NombreSolicitante,
                CorreoSolicitante = t.CorreoSolicitante,
                UbicacionSolicitante = t.UbicacionSolicitante,
                SolicitanteVip = t.SolicitanteVip,
                Elemento = t.Elemento,
                AnyDeskEquipo = t.AnyDeskEquipo,
                DepartamentoOrigen = t.DepartamentoOrigen,
                FechaCreacion = t.FechaCreacion,
                FechaVencimiento = t.FechaVencimiento,
                FechaCierre = t.FechaCierre,
                FechaResolucion = t.FechaResolucion,
                FechaUltimaActualizacion = t.FechaUltimaActualizacion,
                TiempoInicialRespuesta = t.TiempoInicialRespuesta,
                TiempoPrimeraRespuestaHoras = t.TiempoPrimeraRespuestaHoras,
                TiempoResolucionHoras = t.TiempoResolucionHoras,
                RegistroTiempo = t.RegistroTiempo,
                InteraccionesCliente = t.InteraccionesCliente,
                InteraccionesAgente = t.InteraccionesAgente,
                NotaResolucion = t.NotaResolucion,
                ResultadoEncuesta = t.ResultadoEncuesta,
                FechaImportacion = t.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<TicketDto?> ObtenerTicketPorIdAsync(int id)
    {
        return await _contexto.Tickets
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TicketDto
            {
                Id = t.Id,
                IdTicketOrigen = t.IdTicketOrigen,
                AreaUbicacionId = t.AreaUbicacionId,
                Asunto = t.Asunto,
                Descripcion = t.Descripcion,
                Categoria = t.Categoria,
                Subcategoria = t.Subcategoria,
                Tipo = t.Tipo,
                TipoAsociacion = t.TipoAsociacion,
                Etiquetas = t.Etiquetas,
                Prioridad = t.Prioridad,
                Urgencia = t.Urgencia,
                Impacto = t.Impacto,
                Estado = t.Estado,
                EstadoAprobacion = t.EstadoAprobacion,
                EstadoResolucion = t.EstadoResolucion,
                EstadoPrimeraRespuesta = t.EstadoPrimeraRespuesta,
                Grupo = t.Grupo,
                Agente = t.Agente,
                Origen = t.Origen,
                NombreSolicitante = t.NombreSolicitante,
                CorreoSolicitante = t.CorreoSolicitante,
                UbicacionSolicitante = t.UbicacionSolicitante,
                SolicitanteVip = t.SolicitanteVip,
                Elemento = t.Elemento,
                AnyDeskEquipo = t.AnyDeskEquipo,
                DepartamentoOrigen = t.DepartamentoOrigen,
                FechaCreacion = t.FechaCreacion,
                FechaVencimiento = t.FechaVencimiento,
                FechaCierre = t.FechaCierre,
                FechaResolucion = t.FechaResolucion,
                FechaUltimaActualizacion = t.FechaUltimaActualizacion,
                TiempoInicialRespuesta = t.TiempoInicialRespuesta,
                TiempoPrimeraRespuestaHoras = t.TiempoPrimeraRespuestaHoras,
                TiempoResolucionHoras = t.TiempoResolucionHoras,
                RegistroTiempo = t.RegistroTiempo,
                InteraccionesCliente = t.InteraccionesCliente,
                InteraccionesAgente = t.InteraccionesAgente,
                NotaResolucion = t.NotaResolucion,
                ResultadoEncuesta = t.ResultadoEncuesta,
                FechaImportacion = t.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TicketDto> CrearTicketAsync(TicketCrearDto dto)
    {
        var ticket = new Ticket
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Asunto = dto.Asunto,
            Descripcion = dto.Descripcion,
            Categoria = dto.Categoria,
            Subcategoria = dto.Subcategoria,
            Tipo = dto.Tipo,
            Etiquetas = dto.Etiquetas,
            Prioridad = dto.Prioridad,
            Urgencia = dto.Urgencia,
            Impacto = dto.Impacto,
            Estado = "Abierto",
            Grupo = dto.Grupo,
            Agente = dto.Agente,
            Origen = dto.Origen,
            NombreSolicitante = dto.NombreSolicitante,
            CorreoSolicitante = dto.CorreoSolicitante,
            UbicacionSolicitante = dto.UbicacionSolicitante,
            SolicitanteVip = dto.SolicitanteVip,
            Elemento = dto.Elemento,
            AnyDeskEquipo = dto.AnyDeskEquipo,
            FechaCreacion = DateTime.UtcNow,
            FechaVencimiento = dto.FechaVencimiento,
        };

        _contexto.Tickets.Add(ticket);
        await _contexto.SaveChangesAsync();

        return MapearDto(ticket);
    }

    public async Task<TicketDto?> ActualizarTicketAsync(int id, TicketActualizarDto dto)
    {
        var ticket = await _contexto.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket is null)
        {
            return null;
        }

        ticket.Estado = dto.Estado;
        ticket.EstadoResolucion = dto.EstadoResolucion;
        ticket.EstadoAprobacion = dto.EstadoAprobacion;
        ticket.Grupo = dto.Grupo;
        ticket.Agente = dto.Agente;
        ticket.NotaResolucion = dto.NotaResolucion;
        ticket.ResultadoEncuesta = dto.ResultadoEncuesta;
        ticket.FechaUltimaActualizacion = DateTime.UtcNow;

        var estadosDeCierre = new[] { "Cerrado", "Resuelto" };
        if (estadosDeCierre.Contains(dto.Estado) && ticket.FechaCierre is null)
        {
            ticket.FechaCierre = DateTime.UtcNow;
        }

        if (dto.Estado == "Resuelto" && ticket.FechaResolucion is null)
        {
            ticket.FechaResolucion = DateTime.UtcNow;
        }

        await _contexto.SaveChangesAsync();

        return MapearDto(ticket);
    }

    public async Task<TicketImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new TicketImportarResultadoDto();

        using var libro = new XLWorkbook(archivoExcel);
        var hoja = libro.Worksheets.First();
        var filaEncabezado = hoja.FirstRowUsed();
        if (filaEncabezado is null)
        {
            resultado.Errores.Add("El archivo está vacío.");
            return resultado;
        }

        var indicePorEncabezado = new Dictionary<string, int>();
        foreach (var celda in filaEncabezado.CellsUsed())
        {
            indicePorEncabezado[Normalizar(celda.GetString())] = celda.Address.ColumnNumber;
        }

        int? IndiceDe(string claveLogica)
        {
            var claveEsperada = ColumnasTicket[claveLogica];
            return indicePorEncabezado.TryGetValue(claveEsperada, out var indice) ? indice : (int?)null;
        }

        string? Texto(IXLRow fila, string claveLogica)
        {
            var indice = IndiceDe(claveLogica);
            if (indice is null)
            {
                return null;
            }

            var valor = fila.Cell(indice.Value).GetString().Trim();
            return string.IsNullOrWhiteSpace(valor) ? null : valor;
        }

        DateTime? Fecha(IXLRow fila, string claveLogica)
        {
            var indice = IndiceDe(claveLogica);
            if (indice is null)
            {
                return null;
            }

            var celda = fila.Cell(indice.Value);
            if (celda.TryGetValue(out DateTime valorFecha))
            {
                return valorFecha;
            }

            var texto = celda.GetString().Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            return DateTime.TryParse(texto, CultureInfo.GetCultureInfo("es-MX"), DateTimeStyles.None, out var parseado)
                ? parseado
                : null;
        }

        decimal? Numero(IXLRow fila, string claveLogica)
        {
            var indice = IndiceDe(claveLogica);
            if (indice is null)
            {
                return null;
            }

            var celda = fila.Cell(indice.Value);
            if (celda.TryGetValue(out double valorNumero))
            {
                return (decimal)valorNumero;
            }

            var texto = celda.GetString().Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.GetCultureInfo("es-MX"), out var parseado)
                ? parseado
                : null;
        }

        int? Entero(IXLRow fila, string claveLogica)
        {
            var valor = Numero(fila, claveLogica);
            return valor.HasValue ? (int)valor.Value : null;
        }

        bool Booleano(IXLRow fila, string claveLogica)
        {
            var texto = Texto(fila, claveLogica)?.ToLowerInvariant();
            return texto is "si" or "sí" or "yes" or "true" or "1" or "verdadero";
        }

        void MapearCampos(Ticket ticket, IXLRow fila, int areaUbicacionId)
        {
            ticket.AreaUbicacionId = areaUbicacionId;
            ticket.Asunto = Texto(fila, "Asunto") ?? "(sin asunto)";
            ticket.Descripcion = Texto(fila, "Descripcion") ?? string.Empty;
            ticket.Categoria = Texto(fila, "Categoria");
            ticket.Subcategoria = Texto(fila, "Subcategoria");
            ticket.Tipo = Texto(fila, "Tipo");
            ticket.TipoAsociacion = Texto(fila, "TipoAsociacion");
            ticket.Etiquetas = Texto(fila, "Etiquetas");
            ticket.Prioridad = Texto(fila, "Prioridad") ?? "Media";
            ticket.Urgencia = Texto(fila, "Urgencia");
            ticket.Impacto = Texto(fila, "Impacto");
            ticket.Estado = Texto(fila, "Estado") ?? "Abierto";
            ticket.EstadoAprobacion = Texto(fila, "EstadoAprobacion");
            ticket.EstadoResolucion = Texto(fila, "EstadoResolucion");
            ticket.EstadoPrimeraRespuesta = Texto(fila, "EstadoPrimeraRespuesta");
            ticket.Grupo = Texto(fila, "Grupo");
            ticket.Agente = Texto(fila, "Agente");
            ticket.Origen = Texto(fila, "Origen");
            ticket.NombreSolicitante = Texto(fila, "NombreSolicitante") ?? string.Empty;
            ticket.CorreoSolicitante = Texto(fila, "CorreoSolicitante");
            ticket.UbicacionSolicitante = Texto(fila, "UbicacionSolicitante");
            ticket.SolicitanteVip = Booleano(fila, "SolicitanteVip");
            ticket.Elemento = Texto(fila, "Elemento");
            ticket.AnyDeskEquipo = Texto(fila, "AnyDeskEquipo");
            ticket.DepartamentoOrigen = Texto(fila, "Departamento");
            ticket.FechaCreacion = Fecha(fila, "HoraCreacion") ?? ticket.FechaCreacion;
            ticket.FechaVencimiento = Fecha(fila, "HoraVencimiento");
            ticket.FechaCierre = Fecha(fila, "HoraCierre");
            ticket.FechaResolucion = Fecha(fila, "HoraResolucion");
            ticket.FechaUltimaActualizacion = Fecha(fila, "HoraUltimaActualizacion");
            ticket.TiempoInicialRespuesta = Texto(fila, "TiempoInicialRespuesta");
            ticket.TiempoPrimeraRespuestaHoras = Numero(fila, "TiempoPrimeraRespuestaHoras");
            ticket.TiempoResolucionHoras = Numero(fila, "TiempoResolucionHoras");
            ticket.RegistroTiempo = Numero(fila, "RegistroTiempo");
            ticket.InteraccionesCliente = Entero(fila, "InteraccionesCliente");
            ticket.InteraccionesAgente = Entero(fila, "InteraccionesAgente");
            ticket.NotaResolucion = Texto(fila, "NotaResolucion");
            ticket.ResultadoEncuesta = Texto(fila, "ResultadoEncuesta");
        }

        // Catálogo de Ubicaciones (nombre normalizado -> Id) y de
        // AreaUbicacion de IT (UbicacionId -> AreaUbicacionId), para
        // resolver la columna "Ubicación" del archivo contra el catálogo
        // interno.
        var ubicaciones = await _contexto.Ubicaciones
            .ToDictionaryAsync(u => Normalizar(u.Nombre), u => u.Id);

        var areaUbicacionesIt = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == AREA_ID_IT)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.Tickets
            .Where(t => t.IdTicketOrigen != null)
            .ToDictionaryAsync(t => t.IdTicketOrigen!, t => t);

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;

            var idOrigen = Texto(fila, "IdTicket");
            if (string.IsNullOrWhiteSpace(idOrigen))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'ID del Ticket', se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var ubicacionTexto = Texto(fila, "Ubicacion");
            int? areaUbicacionId = null;
            if (ubicacionTexto is not null
                && ubicaciones.TryGetValue(Normalizar(ubicacionTexto), out var ubicacionId)
                && areaUbicacionesIt.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila} (ticket {idOrigen}): la Ubicación '{ubicacionTexto}' no coincide con ninguna ubicación del catálogo de IT, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            if (existentes.TryGetValue(idOrigen, out var ticketExistente))
            {
                MapearCampos(ticketExistente, fila, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new Ticket
                {
                    IdTicketOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, fila, areaUbicacionId.Value);
                _contexto.Tickets.Add(nuevo);
                existentes[idOrigen] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static TicketDto MapearDto(Ticket t) => new()
    {
        Id = t.Id,
        IdTicketOrigen = t.IdTicketOrigen,
        AreaUbicacionId = t.AreaUbicacionId,
        Asunto = t.Asunto,
        Descripcion = t.Descripcion,
        Categoria = t.Categoria,
        Subcategoria = t.Subcategoria,
        Tipo = t.Tipo,
        TipoAsociacion = t.TipoAsociacion,
        Etiquetas = t.Etiquetas,
        Prioridad = t.Prioridad,
        Urgencia = t.Urgencia,
        Impacto = t.Impacto,
        Estado = t.Estado,
        EstadoAprobacion = t.EstadoAprobacion,
        EstadoResolucion = t.EstadoResolucion,
        EstadoPrimeraRespuesta = t.EstadoPrimeraRespuesta,
        Grupo = t.Grupo,
        Agente = t.Agente,
        Origen = t.Origen,
        NombreSolicitante = t.NombreSolicitante,
        CorreoSolicitante = t.CorreoSolicitante,
        UbicacionSolicitante = t.UbicacionSolicitante,
        SolicitanteVip = t.SolicitanteVip,
        Elemento = t.Elemento,
        AnyDeskEquipo = t.AnyDeskEquipo,
        DepartamentoOrigen = t.DepartamentoOrigen,
        FechaCreacion = t.FechaCreacion,
        FechaVencimiento = t.FechaVencimiento,
        FechaCierre = t.FechaCierre,
        FechaResolucion = t.FechaResolucion,
        FechaUltimaActualizacion = t.FechaUltimaActualizacion,
        TiempoInicialRespuesta = t.TiempoInicialRespuesta,
        TiempoPrimeraRespuestaHoras = t.TiempoPrimeraRespuestaHoras,
        TiempoResolucionHoras = t.TiempoResolucionHoras,
        RegistroTiempo = t.RegistroTiempo,
        InteraccionesCliente = t.InteraccionesCliente,
        InteraccionesAgente = t.InteraccionesAgente,
        NotaResolucion = t.NotaResolucion,
        ResultadoEncuesta = t.ResultadoEncuesta,
        FechaImportacion = t.FechaImportacion,
    };

    // Normaliza un encabezado o un valor de texto para comparar sin
    // importar acentos, mayúsculas ni espacios/paréntesis (ej. "Tiempo de
    // primera respuesta (en horas)" -> "tiempodeprimerarespuestaenhoras").
    private static string Normalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        var descompuesto = texto.Normalize(NormalizationForm.FormD);
        var sinAcentos = new string(descompuesto
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        var soloAlfanumerico = new string(sinAcentos.Where(char.IsLetterOrDigit).ToArray());
        return soloAlfanumerico.ToLowerInvariant();
    }
}
