using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class UsuarioService : IUsuarioService
{
    private readonly ApplicationDbContext _contexto;

    public UsuarioService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<UsuarioDto>> ObtenerUsuariosAsync()
    {
        return await _contexto.Usuarios
            .AsNoTracking()
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                NombreUsuario = u.NombreUsuario,
                NombreCompleto = u.NombreCompleto,
                Rol = u.Rol,
                Activo = u.Activo,
                DireccionId = u.DireccionId,
                DepartamentoId = u.DepartamentoId,
                AreaId = u.AreaId,
                Modulos = u.ModulosAsignados.Select(m => m.ModuloClave).ToList(),
                ModuloPlantas = u.ModuloUbicacionesAsignadas
                    .Select(mp => new ModuloUbicacionAsignadaDto { ModuloClave = mp.ModuloClave, AreaUbicacionId = mp.AreaUbicacionId })
                    .ToList(),
            })
            .ToListAsync();
    }

    public async Task<UsuarioDto?> ObtenerUsuarioPorIdAsync(int id)
    {
        return await _contexto.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                NombreUsuario = u.NombreUsuario,
                NombreCompleto = u.NombreCompleto,
                Rol = u.Rol,
                Activo = u.Activo,
                DireccionId = u.DireccionId,
                DepartamentoId = u.DepartamentoId,
                AreaId = u.AreaId,
                Modulos = u.ModulosAsignados.Select(m => m.ModuloClave).ToList(),
                ModuloPlantas = u.ModuloUbicacionesAsignadas
                    .Select(mp => new ModuloUbicacionAsignadaDto { ModuloClave = mp.ModuloClave, AreaUbicacionId = mp.AreaUbicacionId })
                    .ToList(),
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UsuarioDto> CrearUsuarioAsync(UsuarioCrearDto dto)
    {
        var usuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            NombreCompleto = dto.NombreCompleto,
            Rol = dto.Rol,
            Activo = true,
            DireccionId = dto.DireccionId,
            DepartamentoId = dto.DepartamentoId,
            AreaId = dto.AreaId,
        };

        _contexto.Usuarios.Add(usuario);
        await _contexto.SaveChangesAsync();

        AsignarModulos(usuario.Id, dto.Modulos);
        AsignarModuloPlantas(usuario.Id, dto.Modulos, dto.ModuloPlantas);
        await _contexto.SaveChangesAsync();

        return new UsuarioDto
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Rol = usuario.Rol,
            Activo = usuario.Activo,
            DireccionId = usuario.DireccionId,
            DepartamentoId = usuario.DepartamentoId,
            AreaId = usuario.AreaId,
            Modulos = dto.Modulos.Where(Models.Modulos.EsValido).Distinct().ToList(),
            ModuloPlantas = FiltrarModuloPlantasValidas(dto.Modulos, dto.ModuloPlantas),
        };
    }

    public async Task<UsuarioDto?> ActualizarUsuarioAsync(int id, UsuarioActualizarDto dto)
    {
        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null)
        {
            return null;
        }

        usuario.NombreCompleto = dto.NombreCompleto;
        usuario.Rol = dto.Rol;
        usuario.Activo = dto.Activo;
        usuario.DireccionId = dto.DireccionId;
        usuario.DepartamentoId = dto.DepartamentoId;
        usuario.AreaId = dto.AreaId;

        var modulosActuales = await _contexto.UsuarioModulos.Where(m => m.UsuarioId == id).ToListAsync();
        _contexto.UsuarioModulos.RemoveRange(modulosActuales);
        AsignarModulos(id, dto.Modulos);

        var modulosPlantasActuales = await _contexto.UsuarioModuloUbicaciones.Where(m => m.UsuarioId == id).ToListAsync();
        _contexto.UsuarioModuloUbicaciones.RemoveRange(modulosPlantasActuales);
        AsignarModuloPlantas(id, dto.Modulos, dto.ModuloPlantas);

        await _contexto.SaveChangesAsync();

        return new UsuarioDto
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Rol = usuario.Rol,
            Activo = usuario.Activo,
            DireccionId = usuario.DireccionId,
            DepartamentoId = usuario.DepartamentoId,
            AreaId = usuario.AreaId,
            Modulos = dto.Modulos.Where(Models.Modulos.EsValido).Distinct().ToList(),
            ModuloPlantas = FiltrarModuloPlantasValidas(dto.Modulos, dto.ModuloPlantas),
        };
    }

    public async Task<bool> CambiarPasswordAsync(int id, string nuevaPassword)
    {
        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null)
        {
            return false;
        }

        usuario.PasswordHash = PasswordHasher.Hash(nuevaPassword);
        await _contexto.SaveChangesAsync();
        return true;
    }

    // Solo se guardan claves de módulo reconocidas (ver Modulos.EsValido);
    // cualquier clave desconocida que llegue del cliente se ignora en
    // silencio en vez de fallar toda la operación.
    private void AsignarModulos(int usuarioId, IEnumerable<string> modulos)
    {
        foreach (var clave in modulos.Where(Models.Modulos.EsValido).Distinct())
        {
            _contexto.UsuarioModulos.Add(new UsuarioModulo { UsuarioId = usuarioId, ModuloClave = clave });
        }
    }

    // Nivel de captura por Planta (20/sep/2026). Solo se guarda una fila
    // Módulo+Planta si: (a) el módulo es una clave reconocida, (b) ese
    // módulo está en la lista de módulos que se le está asignando al
    // usuario en esta misma operación (no tiene sentido restringir un
    // módulo que ni siquiera se le dio), y (c) la Planta pertenece al
    // Área de ese módulo. Cualquier combinación que no cumpla esto se
    // ignora en silencio, mismo criterio que AsignarModulos con claves
    // desconocidas.
    private void AsignarModuloPlantas(int usuarioId, IEnumerable<string> modulosAsignados, IEnumerable<ModuloUbicacionAsignadaDto> moduloPlantas)
    {
        foreach (var par in FiltrarModuloPlantasValidas(modulosAsignados, moduloPlantas))
        {
            _contexto.UsuarioModuloUbicaciones.Add(new UsuarioModuloUbicacion
            {
                UsuarioId = usuarioId,
                ModuloClave = par.ModuloClave,
                AreaUbicacionId = par.AreaUbicacionId,
            });
        }
    }

    // No valida que la Planta pertenezca al Área del módulo (igual que el
    // resto del formulario de Usuarios, que confía en las opciones que ya
    // filtró el propio frontend) — solo descarta módulos no reconocidos o
    // no incluidos en esta asignación.
    private static List<ModuloUbicacionAsignadaDto> FiltrarModuloPlantasValidas(
        IEnumerable<string> modulosAsignados,
        IEnumerable<ModuloUbicacionAsignadaDto> moduloPlantas)
    {
        var modulosValidos = modulosAsignados.Where(Models.Modulos.EsValido).ToHashSet();

        return moduloPlantas
            .Where(mp => modulosValidos.Contains(mp.ModuloClave))
            .DistinctBy(mp => (mp.ModuloClave, mp.AreaUbicacionId))
            .ToList();
    }
}
