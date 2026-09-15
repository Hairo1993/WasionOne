using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class InventarioService : IInventarioService
{
    private readonly ApplicationDbContext _contexto;

    public InventarioService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<InventarioEquipoDto>> ObtenerEquiposAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.InventarioEquipos.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(e => e.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderBy(e => e.TipoEquipo)
            .ThenBy(e => e.Marca)
            .Select(e => new InventarioEquipoDto
            {
                Id = e.Id,
                AreaUbicacionId = e.AreaUbicacionId,
                Codigo = e.Codigo,
                Almacen = e.Almacen,
                UbicacionExacta = e.UbicacionExacta,
                TipoEquipo = e.TipoEquipo,
                Hostname = e.Hostname,
                Marca = e.Marca,
                Modelo = e.Modelo,
                Serial = e.Serial,
                Ram = e.Ram,
                Procesador = e.Procesador,
                Almacenamiento = e.Almacenamiento,
                SistemaOperativo = e.SistemaOperativo,
                MacWireless = e.MacWireless,
                MacEthernet = e.MacEthernet,
                UsuarioAsignado = e.UsuarioAsignado,
                Estado = e.Estado,
                FechaCompra = e.FechaCompra,
                TerminoGarantia = e.TerminoGarantia,
                FechaAlta = e.FechaAlta,
                FechaBaja = e.FechaBaja,
                Observaciones = e.Observaciones,
            })
            .ToListAsync();
    }

    public async Task<InventarioEquipoDto?> ObtenerEquipoPorIdAsync(int id)
    {
        return await _contexto.InventarioEquipos
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new InventarioEquipoDto
            {
                Id = e.Id,
                AreaUbicacionId = e.AreaUbicacionId,
                Codigo = e.Codigo,
                Almacen = e.Almacen,
                UbicacionExacta = e.UbicacionExacta,
                TipoEquipo = e.TipoEquipo,
                Hostname = e.Hostname,
                Marca = e.Marca,
                Modelo = e.Modelo,
                Serial = e.Serial,
                Ram = e.Ram,
                Procesador = e.Procesador,
                Almacenamiento = e.Almacenamiento,
                SistemaOperativo = e.SistemaOperativo,
                MacWireless = e.MacWireless,
                MacEthernet = e.MacEthernet,
                UsuarioAsignado = e.UsuarioAsignado,
                Estado = e.Estado,
                FechaCompra = e.FechaCompra,
                TerminoGarantia = e.TerminoGarantia,
                FechaAlta = e.FechaAlta,
                FechaBaja = e.FechaBaja,
                Observaciones = e.Observaciones,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<InventarioEquipoDto> CrearEquipoAsync(InventarioEquipoCrearDto dto)
    {
        var equipo = new InventarioEquipo
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Codigo = dto.Codigo,
            Almacen = dto.Almacen,
            UbicacionExacta = dto.UbicacionExacta,
            TipoEquipo = dto.TipoEquipo,
            Hostname = dto.Hostname,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Serial = dto.Serial,
            Ram = dto.Ram,
            Procesador = dto.Procesador,
            Almacenamiento = dto.Almacenamiento,
            SistemaOperativo = dto.SistemaOperativo,
            MacWireless = dto.MacWireless,
            MacEthernet = dto.MacEthernet,
            UsuarioAsignado = dto.UsuarioAsignado,
            Estado = "Activo",
            FechaCompra = dto.FechaCompra,
            TerminoGarantia = dto.TerminoGarantia,
            FechaAlta = DateTime.UtcNow,
            Observaciones = dto.Observaciones,
        };

        _contexto.InventarioEquipos.Add(equipo);
        await _contexto.SaveChangesAsync();

        return new InventarioEquipoDto
        {
            Id = equipo.Id,
            AreaUbicacionId = equipo.AreaUbicacionId,
            Codigo = equipo.Codigo,
            Almacen = equipo.Almacen,
            UbicacionExacta = equipo.UbicacionExacta,
            TipoEquipo = equipo.TipoEquipo,
            Hostname = equipo.Hostname,
            Marca = equipo.Marca,
            Modelo = equipo.Modelo,
            Serial = equipo.Serial,
            Ram = equipo.Ram,
            Procesador = equipo.Procesador,
            Almacenamiento = equipo.Almacenamiento,
            SistemaOperativo = equipo.SistemaOperativo,
            MacWireless = equipo.MacWireless,
            MacEthernet = equipo.MacEthernet,
            UsuarioAsignado = equipo.UsuarioAsignado,
            Estado = equipo.Estado,
            FechaCompra = equipo.FechaCompra,
            TerminoGarantia = equipo.TerminoGarantia,
            FechaAlta = equipo.FechaAlta,
            FechaBaja = equipo.FechaBaja,
            Observaciones = equipo.Observaciones,
        };
    }

    public async Task<InventarioEquipoDto?> ActualizarEquipoAsync(int id, InventarioEquipoActualizarDto dto)
    {
        var equipo = await _contexto.InventarioEquipos.FirstOrDefaultAsync(e => e.Id == id);
        if (equipo is null)
        {
            return null;
        }

        equipo.Estado = dto.Estado;
        equipo.Almacen = dto.Almacen;
        equipo.UbicacionExacta = dto.UbicacionExacta;
        equipo.UsuarioAsignado = dto.UsuarioAsignado;
        equipo.Observaciones = dto.Observaciones;

        if (dto.Estado == "Baja" && equipo.FechaBaja is null)
        {
            equipo.FechaBaja = DateTime.UtcNow;
        }

        await _contexto.SaveChangesAsync();

        return new InventarioEquipoDto
        {
            Id = equipo.Id,
            AreaUbicacionId = equipo.AreaUbicacionId,
            Codigo = equipo.Codigo,
            Almacen = equipo.Almacen,
            UbicacionExacta = equipo.UbicacionExacta,
            TipoEquipo = equipo.TipoEquipo,
            Hostname = equipo.Hostname,
            Marca = equipo.Marca,
            Modelo = equipo.Modelo,
            Serial = equipo.Serial,
            Ram = equipo.Ram,
            Procesador = equipo.Procesador,
            Almacenamiento = equipo.Almacenamiento,
            SistemaOperativo = equipo.SistemaOperativo,
            MacWireless = equipo.MacWireless,
            MacEthernet = equipo.MacEthernet,
            UsuarioAsignado = equipo.UsuarioAsignado,
            Estado = equipo.Estado,
            FechaCompra = equipo.FechaCompra,
            TerminoGarantia = equipo.TerminoGarantia,
            FechaAlta = equipo.FechaAlta,
            FechaBaja = equipo.FechaBaja,
            Observaciones = equipo.Observaciones,
        };
    }
}
