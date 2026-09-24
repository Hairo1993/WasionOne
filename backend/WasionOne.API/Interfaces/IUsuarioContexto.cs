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
    IEnumerable<string> Modulos { get; }

    // Nivel de captura por Planta dentro de cada módulo (20/sep/2026): un
    // par (ModuloClave, AreaUbicacionId) por cada Planta a la que el
    // usuario está restringido en ese módulo. Un módulo que no aparece
    // aquí en ningún par = todas las Plantas del Área (sin restricción).
    // Ver AutorizacionModuloHelper para cómo se consulta en la práctica.
    IEnumerable<(string ModuloClave, int AreaUbicacionId)> ModuloPlantas { get; }
}
