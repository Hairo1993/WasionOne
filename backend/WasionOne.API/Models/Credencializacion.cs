namespace WasionOne.API.Models;

// Bitácora de acceso/credencialización de visitantes, contratistas y
// proveedores. Un registro por evento de entrada (con su salida
// registrada cuando aplica), no un agregado.
public class Credencializacion
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones.
    public string? IdOrigen { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;
    public string? Empresa { get; set; }

    // Texto libre con sugerencias en el formulario (Visitante/Contratista/
    // Proveedor/Empleado/Otro), mismo criterio que otros catálogos "fijos
    // en código" del proyecto.
    public string? TipoAcceso { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public string? Identificacion { get; set; }
    public string? PersonaQueVisita { get; set; }
    public string? MotivoVisita { get; set; }
    public string? AreaDeTrabajo { get; set; }

    public DateTime? FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }

    public string Estado { get; set; } = "Dentro de instalaciones";

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
