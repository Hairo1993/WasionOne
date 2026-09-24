namespace WasionOne.API.Models;

// Registro de una reunión mensual con proveedor de vigilancia. Un registro
// por reunión.
public class ReunionProveedor
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones. Opcional: si el archivo no lo
    // trae, cada fila crea un registro nuevo.
    public string? IdOrigen { get; set; }

    public DateTime? Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public string? Proveedor { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public string? AsuntoMotivo { get; set; }
    public string? Asistentes { get; set; }
    public string? MinutaAcuerdos { get; set; }

    public string? RegistradoPor { get; set; }

    // "Fecha Registro" del sistema de origen — dato de negocio, distinto
    // de FechaImportacion (control interno de importación, más abajo).
    public DateTime? FechaRegistro { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
