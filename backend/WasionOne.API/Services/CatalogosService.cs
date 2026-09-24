using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class CatalogosService : ICatalogosService
{
    private readonly ApplicationDbContext _contexto;

    public CatalogosService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    // --- Direcciones ---

    public async Task<IEnumerable<DireccionDto>> ObtenerDireccionesAsync()
    {
        return await _contexto.Direcciones
            .AsNoTracking()
            .Select(d => new DireccionDto { Id = d.Id, Nombre = d.Nombre })
            .ToListAsync();
    }

    public async Task<DireccionDto?> ObtenerDireccionPorIdAsync(int id)
    {
        return await _contexto.Direcciones
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DireccionDto { Id = d.Id, Nombre = d.Nombre })
            .FirstOrDefaultAsync();
    }

    public async Task<DireccionDto> CrearDireccionAsync(DireccionCrearDto dto)
    {
        var direccion = new Direccion { Nombre = dto.Nombre };
        _contexto.Direcciones.Add(direccion);
        await _contexto.SaveChangesAsync();
        return new DireccionDto { Id = direccion.Id, Nombre = direccion.Nombre };
    }

    public async Task<DireccionDto?> ActualizarDireccionAsync(int id, DireccionActualizarDto dto)
    {
        var direccion = await _contexto.Direcciones.FirstOrDefaultAsync(d => d.Id == id);
        if (direccion is null)
        {
            return null;
        }

        direccion.Nombre = dto.Nombre;
        await _contexto.SaveChangesAsync();
        return new DireccionDto { Id = direccion.Id, Nombre = direccion.Nombre };
    }

    public async Task<ResultadoOperacionDto> EliminarDireccionAsync(int id)
    {
        var direccion = await _contexto.Direcciones.FirstOrDefaultAsync(d => d.Id == id);
        if (direccion is null)
        {
            return new ResultadoOperacionDto { Exito = false };
        }

        var tieneDependientes = await _contexto.Departamentos.AnyAsync(d => d.DireccionId == id)
            || await _contexto.Usuarios.AnyAsync(u => u.DireccionId == id);
        if (tieneDependientes)
        {
            return new ResultadoOperacionDto
            {
                Exito = false,
                Error = "No se puede eliminar: tiene Departamentos o Usuarios asociados.",
            };
        }

        _contexto.Direcciones.Remove(direccion);
        await _contexto.SaveChangesAsync();
        return new ResultadoOperacionDto { Exito = true };
    }

    // --- Departamentos ---

    public async Task<IEnumerable<DepartamentoDto>> ObtenerDepartamentosAsync(int? direccionId)
    {
        var consulta = _contexto.Departamentos.AsNoTracking().AsQueryable();

        if (direccionId.HasValue)
        {
            consulta = consulta.Where(d => d.DireccionId == direccionId.Value);
        }

        return await consulta
            .Select(d => new DepartamentoDto { Id = d.Id, Nombre = d.Nombre, DireccionId = d.DireccionId })
            .ToListAsync();
    }

    public async Task<DepartamentoDto?> ObtenerDepartamentoPorIdAsync(int id)
    {
        return await _contexto.Departamentos
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DepartamentoDto { Id = d.Id, Nombre = d.Nombre, DireccionId = d.DireccionId })
            .FirstOrDefaultAsync();
    }

    public async Task<DepartamentoDto> CrearDepartamentoAsync(DepartamentoCrearDto dto)
    {
        var departamento = new Departamento { Nombre = dto.Nombre, DireccionId = dto.DireccionId };
        _contexto.Departamentos.Add(departamento);
        await _contexto.SaveChangesAsync();
        return new DepartamentoDto { Id = departamento.Id, Nombre = departamento.Nombre, DireccionId = departamento.DireccionId };
    }

    public async Task<DepartamentoDto?> ActualizarDepartamentoAsync(int id, DepartamentoActualizarDto dto)
    {
        var departamento = await _contexto.Departamentos.FirstOrDefaultAsync(d => d.Id == id);
        if (departamento is null)
        {
            return null;
        }

        departamento.Nombre = dto.Nombre;
        departamento.DireccionId = dto.DireccionId;
        await _contexto.SaveChangesAsync();
        return new DepartamentoDto { Id = departamento.Id, Nombre = departamento.Nombre, DireccionId = departamento.DireccionId };
    }

    public async Task<ResultadoOperacionDto> EliminarDepartamentoAsync(int id)
    {
        var departamento = await _contexto.Departamentos.FirstOrDefaultAsync(d => d.Id == id);
        if (departamento is null)
        {
            return new ResultadoOperacionDto { Exito = false };
        }

        var tieneDependientes = await _contexto.Areas.AnyAsync(a => a.DepartamentoId == id)
            || await _contexto.Usuarios.AnyAsync(u => u.DepartamentoId == id);
        if (tieneDependientes)
        {
            return new ResultadoOperacionDto
            {
                Exito = false,
                Error = "No se puede eliminar: tiene Áreas o Usuarios asociados.",
            };
        }

        _contexto.Departamentos.Remove(departamento);
        await _contexto.SaveChangesAsync();
        return new ResultadoOperacionDto { Exito = true };
    }

    // --- Areas ---

    public async Task<IEnumerable<AreaDto>> ObtenerAreasAsync(int? departamentoId)
    {
        var consulta = _contexto.Areas.AsNoTracking().AsQueryable();

        if (departamentoId.HasValue)
        {
            consulta = consulta.Where(a => a.DepartamentoId == departamentoId.Value);
        }

        return await consulta
            .Select(a => new AreaDto { Id = a.Id, Nombre = a.Nombre, DepartamentoId = a.DepartamentoId })
            .ToListAsync();
    }

    public async Task<AreaDto?> ObtenerAreaPorIdAsync(int id)
    {
        return await _contexto.Areas
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AreaDto { Id = a.Id, Nombre = a.Nombre, DepartamentoId = a.DepartamentoId })
            .FirstOrDefaultAsync();
    }

    public async Task<AreaDto> CrearAreaAsync(AreaCrearDto dto)
    {
        var area = new Area { Nombre = dto.Nombre, DepartamentoId = dto.DepartamentoId };
        _contexto.Areas.Add(area);
        await _contexto.SaveChangesAsync();
        return new AreaDto { Id = area.Id, Nombre = area.Nombre, DepartamentoId = area.DepartamentoId };
    }

    public async Task<AreaDto?> ActualizarAreaAsync(int id, AreaActualizarDto dto)
    {
        var area = await _contexto.Areas.FirstOrDefaultAsync(a => a.Id == id);
        if (area is null)
        {
            return null;
        }

        area.Nombre = dto.Nombre;
        area.DepartamentoId = dto.DepartamentoId;
        await _contexto.SaveChangesAsync();
        return new AreaDto { Id = area.Id, Nombre = area.Nombre, DepartamentoId = area.DepartamentoId };
    }

    public async Task<ResultadoOperacionDto> EliminarAreaAsync(int id)
    {
        var area = await _contexto.Areas.FirstOrDefaultAsync(a => a.Id == id);
        if (area is null)
        {
            return new ResultadoOperacionDto { Exito = false };
        }

        var tieneDependientes = await _contexto.AreaUbicaciones.AnyAsync(au => au.AreaId == id)
            || await _contexto.Usuarios.AnyAsync(u => u.AreaId == id);
        if (tieneDependientes)
        {
            return new ResultadoOperacionDto
            {
                Exito = false,
                Error = "No se puede eliminar: tiene Ubicaciones asignadas o Usuarios asociados.",
            };
        }

        _contexto.Areas.Remove(area);
        await _contexto.SaveChangesAsync();
        return new ResultadoOperacionDto { Exito = true };
    }

    // --- Ubicaciones ---

    public async Task<IEnumerable<UbicacionDto>> ObtenerUbicacionesAsync()
    {
        return await _contexto.Ubicaciones
            .AsNoTracking()
            .Select(u => new UbicacionDto { Id = u.Id, Nombre = u.Nombre })
            .ToListAsync();
    }

    public async Task<UbicacionDto?> ObtenerUbicacionPorIdAsync(int id)
    {
        return await _contexto.Ubicaciones
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UbicacionDto { Id = u.Id, Nombre = u.Nombre })
            .FirstOrDefaultAsync();
    }

    public async Task<UbicacionDto> CrearUbicacionAsync(UbicacionCrearDto dto)
    {
        var ubicacion = new Ubicacion { Nombre = dto.Nombre };
        _contexto.Ubicaciones.Add(ubicacion);
        await _contexto.SaveChangesAsync();
        return new UbicacionDto { Id = ubicacion.Id, Nombre = ubicacion.Nombre };
    }

    public async Task<UbicacionDto?> ActualizarUbicacionAsync(int id, UbicacionActualizarDto dto)
    {
        var ubicacion = await _contexto.Ubicaciones.FirstOrDefaultAsync(u => u.Id == id);
        if (ubicacion is null)
        {
            return null;
        }

        ubicacion.Nombre = dto.Nombre;
        await _contexto.SaveChangesAsync();
        return new UbicacionDto { Id = ubicacion.Id, Nombre = ubicacion.Nombre };
    }

    public async Task<ResultadoOperacionDto> EliminarUbicacionAsync(int id)
    {
        var ubicacion = await _contexto.Ubicaciones.FirstOrDefaultAsync(u => u.Id == id);
        if (ubicacion is null)
        {
            return new ResultadoOperacionDto { Exito = false };
        }

        var tieneDependientes = await _contexto.AreaUbicaciones.AnyAsync(au => au.UbicacionId == id);
        if (tieneDependientes)
        {
            return new ResultadoOperacionDto
            {
                Exito = false,
                Error = "No se puede eliminar: tiene Áreas asignadas en esta Ubicación.",
            };
        }

        _contexto.Ubicaciones.Remove(ubicacion);
        await _contexto.SaveChangesAsync();
        return new ResultadoOperacionDto { Exito = true };
    }

    // --- Area x Ubicacion (nodo operativo) ---

    public async Task<IEnumerable<AreaUbicacionDto>> ObtenerAreaUbicacionesAsync(int? areaId)
    {
        var consulta = _contexto.AreaUbicaciones.AsNoTracking().AsQueryable();

        if (areaId.HasValue)
        {
            consulta = consulta.Where(au => au.AreaId == areaId.Value);
        }

        return await consulta
            .Select(au => new AreaUbicacionDto
            {
                Id = au.Id,
                AreaId = au.AreaId,
                UbicacionId = au.UbicacionId,
            })
            .ToListAsync();
    }

    public async Task<AreaUbicacionDto> CrearAreaUbicacionAsync(AreaUbicacionCrearDto dto)
    {
        var nodo = new AreaUbicacion { AreaId = dto.AreaId, UbicacionId = dto.UbicacionId };
        _contexto.AreaUbicaciones.Add(nodo);
        await _contexto.SaveChangesAsync();
        return new AreaUbicacionDto { Id = nodo.Id, AreaId = nodo.AreaId, UbicacionId = nodo.UbicacionId };
    }

    public async Task<ResultadoOperacionDto> EliminarAreaUbicacionAsync(int id)
    {
        var nodo = await _contexto.AreaUbicaciones.FirstOrDefaultAsync(au => au.Id == id);
        if (nodo is null)
        {
            return new ResultadoOperacionDto { Exito = false };
        }

        // Un nodo AreaUbicacion (ej. "IT en Planta 1") no se puede borrar si
        // ya tiene registros capturados en cualquiera de los módulos que se
        // ligan a él, para no dejar huérfano ese historial.
        var tieneDependientes = await _contexto.Tickets.AnyAsync(t => t.AreaUbicacionId == id)
            || await _contexto.InventarioEquipos.AnyAsync(e => e.AreaUbicacionId == id)
            || await _contexto.IncidentesCriticos.AnyAsync(i => i.AreaUbicacionId == id)
            || await _contexto.Respaldos.AnyAsync(r => r.AreaUbicacionId == id)
            || await _contexto.Platicas.AnyAsync(p => p.AreaUbicacionId == id)
            || await _contexto.AuditoriasEquipo.AnyAsync(a => a.AreaUbicacionId == id);
        if (tieneDependientes)
        {
            return new ResultadoOperacionDto
            {
                Exito = false,
                Error = "No se puede eliminar: ya tiene registros capturados en algún módulo.",
            };
        }

        _contexto.AreaUbicaciones.Remove(nodo);
        await _contexto.SaveChangesAsync();
        return new ResultadoOperacionDto { Exito = true };
    }

    // --- Modulos (catálogo fijo, no vive en base de datos) ---

    public Task<IEnumerable<ModuloDto>> ObtenerModulosAsync()
    {
        var modulos = Modulos.Todos
            .Select(m => new ModuloDto { Clave = m.Clave, Nombre = m.Nombre, AreaId = m.AreaId })
            .ToList();
        return Task.FromResult<IEnumerable<ModuloDto>>(modulos);
    }
}
