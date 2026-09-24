namespace WasionOne.API.Models;

// Nivel de captura por Planta dentro de un módulo ya asignado a un usuario
// (20/sep/2026). Un usuario puede tener un módulo asignado (ver
// UsuarioModulo) sin ninguna fila aquí para ese módulo — eso significa
// "todas las Plantas del Área" (comportamiento histórico, sin cambios
// para los usuarios ya existentes). Si en cambio existen filas aquí para
// ese Usuario+Módulo, el usuario queda restringido a capturar/ver
// únicamente esas Plantas en ese módulo. La restricción es por
// combinación Usuario+Módulo, no por usuario en general: la misma
// persona puede tener un módulo sin restricción y otro restringido a una
// sola Planta.
public class UsuarioModuloUbicacion
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string ModuloClave { get; set; } = string.Empty;

    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }
}
