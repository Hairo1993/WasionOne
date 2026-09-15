namespace WasionOne.API.Interfaces;

// Expone quién es el usuario autenticado en la petición actual (leído de los
// claims del JWT) y en qué nivel de la jerarquía está posicionado, para que
// los servicios puedan restringir/filtrar información según su rol más
// adelante (Fase 2 en adelante: visibilidad heredada hacia abajo).
public interface IUsuarioContexto
{
    bool EstaAutenticado { get; }
    string? NombreUsuario { get; }
    string? Rol { get; }
    int? DireccionId { get; }
    int? DepartamentoId { get; }
    int? AreaId { get; }
}