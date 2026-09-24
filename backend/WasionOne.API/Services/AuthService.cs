using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Services;

// Fase 1: valida contra la tabla Usuario real (contraseña con hash PBKDF2) y
// arma el JWT con el rol y la posición en la jerarquía (Dirección/
// Departamento/Área) del usuario, para que el resto de la aplicación pueda
// usarlos (ver IUsuarioContexto) al construir las pantallas y dashboards por rol.
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _contexto;
    private readonly IConfiguration _configuracion;

    public AuthService(ApplicationDbContext contexto, IConfiguration configuracion)
    {
        _contexto = contexto;
        _configuracion = configuracion;
    }

    public async Task<LoginRespuestaDto?> ValidarCredencialesAsync(LoginDto credenciales)
    {
        var usuario = await _contexto.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NombreUsuario == credenciales.Usuario && u.Activo);

        if (usuario is null || !PasswordHasher.Verificar(credenciales.Password, usuario.PasswordHash))
        {
            return null;
        }

        var roles = new[] { usuario.Rol };
        var modulos = await _contexto.UsuarioModulos
            .AsNoTracking()
            .Where(m => m.UsuarioId == usuario.Id)
            .Select(m => m.ModuloClave)
            .ToListAsync();

        var moduloPlantas = await _contexto.UsuarioModuloUbicaciones
            .AsNoTracking()
            .Where(m => m.UsuarioId == usuario.Id)
            .Select(m => new ModuloUbicacionAsignadaDto { ModuloClave = m.ModuloClave, AreaUbicacionId = m.AreaUbicacionId })
            .ToListAsync();

        var token = GenerarToken(usuario.NombreUsuario, roles, usuario.DireccionId, usuario.DepartamentoId, usuario.AreaId, modulos, moduloPlantas);

        return new LoginRespuestaDto
        {
            Token = token,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Roles = roles,
            DireccionId = usuario.DireccionId,
            DepartamentoId = usuario.DepartamentoId,
            AreaId = usuario.AreaId,
            Modulos = modulos,
            ModuloPlantas = moduloPlantas,
        };
    }

    public async Task<ResultadoOperacionDto> CambiarMiPasswordAsync(string nombreUsuario, string passwordActual, string passwordNueva)
    {
        if (string.IsNullOrWhiteSpace(passwordNueva) || passwordNueva.Length < 6)
        {
            return new ResultadoOperacionDto { Exito = false, Error = "La contraseña nueva debe tener al menos 6 caracteres." };
        }

        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);
        if (usuario is null)
        {
            return new ResultadoOperacionDto { Exito = false, Error = "Usuario no encontrado." };
        }

        if (!PasswordHasher.Verificar(passwordActual, usuario.PasswordHash))
        {
            return new ResultadoOperacionDto { Exito = false, Error = "La contraseña actual no es correcta." };
        }

        usuario.PasswordHash = PasswordHasher.Hash(passwordNueva);
        await _contexto.SaveChangesAsync();

        return new ResultadoOperacionDto { Exito = true };
    }

    private string GenerarToken(
        string nombreUsuario,
        IEnumerable<string> roles,
        int? direccionId,
        int? departamentoId,
        int? areaId,
        IEnumerable<string> modulos,
        IEnumerable<ModuloUbicacionAsignadaDto> moduloPlantas)
    {
        var jwtConfig = _configuracion.GetSection("Jwt");
        var claveSecreta = jwtConfig["ClaveSecreta"]!;
        var minutosExpiracion = int.Parse(jwtConfig["MinutosExpiracion"] ?? "120");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, nombreUsuario),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        // Solo se agrega el claim de jerarquía cuando aplica al rol del
        // usuario (un CEO, por ejemplo, no tiene ninguno de los tres).
        if (direccionId.HasValue)
        {
            claims.Add(new Claim("direccionId", direccionId.Value.ToString()));
        }

        if (departamentoId.HasValue)
        {
            claims.Add(new Claim("departamentoId", departamentoId.Value.ToString()));
        }

        if (areaId.HasValue)
        {
            claims.Add(new Claim("areaId", areaId.Value.ToString()));
        }

        claims.AddRange(modulos.Select(modulo => new Claim("modulo", modulo)));

        // Nivel de captura por Planta (20/sep/2026): un claim por cada
        // combinación Módulo+Planta a la que el usuario está restringido.
        // Un módulo sin ningún claim "moduloPlanta" = todas las Plantas
        // del Área (ver AutorizacionModuloHelper).
        claims.AddRange(moduloPlantas.Select(mp => new Claim("moduloPlanta", $"{mp.ModuloClave}:{mp.AreaUbicacionId}")));

        var credencialesFirma = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutosExpiracion),
            signingCredentials: credencialesFirma);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
