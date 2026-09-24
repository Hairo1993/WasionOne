using System.Security.Claims;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Services;

public class UsuarioContexto : IUsuarioContexto
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioContexto(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Usuario => _httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated ?? false;

    // El token firma el nombre de usuario en el claim estándar "sub", que
    // ASP.NET Core mapea automáticamente a ClaimTypes.NameIdentifier.
    public string? NombreUsuario => Usuario?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Rol => Usuario?.FindFirstValue(ClaimTypes.Role);

    public int? DireccionId => ObtenerClaimEntero("direccionId");

    public int? DepartamentoId => ObtenerClaimEntero("departamentoId");

    public int? AreaId => ObtenerClaimEntero("areaId");

    public IEnumerable<string> Modulos => Usuario?.FindAll("modulo").Select(c => c.Value) ?? Enumerable.Empty<string>();

    // Cada claim "moduloPlanta" viene en formato "{moduloClave}:{areaUbicacionId}".
    public IEnumerable<(string ModuloClave, int AreaUbicacionId)> ModuloPlantas =>
        (Usuario?.FindAll("moduloPlanta").Select(c => c.Value) ?? Enumerable.Empty<string>())
            .Select(ParsearModuloPlanta)
            .Where(par => par is not null)
            .Select(par => par!.Value);

    private static (string ModuloClave, int AreaUbicacionId)? ParsearModuloPlanta(string valor)
    {
        var partes = valor.Split(':', 2);
        if (partes.Length != 2 || !int.TryParse(partes[1], out var areaUbicacionId))
        {
            return null;
        }

        return (partes[0], areaUbicacionId);
    }

    private int? ObtenerClaimEntero(string nombreClaim)
    {
        var valor = Usuario?.FindFirstValue(nombreClaim);
        return int.TryParse(valor, out var resultado) ? resultado : null;
    }
}
