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
        var token = GenerarToken(usuario.NombreUsuario, roles, usuario.DireccionId, usuario.DepartamentoId, usuario.AreaId);

        return new LoginRespuestaDto
        {
            Token = token,
            NombreUsuario = usuario.NombreUsuario,
            Roles = roles,
        };
    }

    private string GenerarToken(
        string nombreUsuario,
        IEnumerable<string> roles,
        int? direccionId,
        int? departamentoId,
        int? areaId)
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