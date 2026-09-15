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
        };
    }
}