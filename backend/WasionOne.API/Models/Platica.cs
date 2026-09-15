namespace WasionOne.API.Models;

// Registro de envío de plática/capacitación de ciberseguridad. La mayoría
// de los envíos son para toda la empresa (AreaUbicacionId = null); si se
// dirigió a una ubicación específica del catálogo de IT, se guarda aquí.
// El Excel de origen no trae una columna de ubicación, así que todo lo
// importado entra como "toda la empresa"; asignar una ubicación específica
// es una opción solo para captura manual.
public class Platica
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones.
    public string? IdOrigen { get; set; }

    // null = toda la empresa (sin ubicación específica).
    public int? AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow.Date;
    public string TemaPolitica { get; set; } = string.Empty;
    public string? ResponsableEnvio { get; set; }
    public string? MedioDifusion { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
