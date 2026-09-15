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

    public string? NombreUsuario => Usuario?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Rol => Usuario?.FindFirstValue(ClaimTypes.Role);

    public int? DireccionId => ObtenerClaimEntero("direccionId");

    public int? DepartamentoId => ObtenerClaimEntero("departamentoId");

    public int? AreaId => ObtenerClaimEntero("areaId");

    private int? ObtenerClaimEntero(string nombreClaim)
    {
        var valor = Usuario?.FindFirstValue(nombreClaim);
        return int.TryParse(valor, out var resultado) ? resultado : null;
    }
}