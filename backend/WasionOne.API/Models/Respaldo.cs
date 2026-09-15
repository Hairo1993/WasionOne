namespace WasionOne.API.Models;

// Registro de respaldo (backup) de un sistema/aplicación de IT. Vive
// dentro del nodo operativo AreaUbicacion (la Ubicación física del
// catálogo donde vive ese respaldo), igual que Tickets/Inventario.
public class Respaldo
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones.
    public string? IdOrigen { get; set; }

    // "Ubicación del Respaldo": coincide con el catálogo de Ubicaciones
    // (el usuario confirmó que es una ubicación física real, no una
    // descripción del medio de respaldo).
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime FechaRespaldo { get; set; } = DateTime.UtcNow.Date;
    public string SistemaAplicacion { get; set; } = string.Empty;
    public string? SoftwareUtilizado { get; set; }
    public string? TipoRespaldo { get; set; }
    public string? Responsable { get; set; }

    // Valores esperados: "Completado", "Fallido", "Pendiente".
    public string Estado { get; set; } = "Completado";

    public string? Observaciones { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
