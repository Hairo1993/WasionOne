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
}