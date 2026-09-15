namespace WasionOne.API.Models;

// Registro de auditoría física de un equipo electrónico del inventario.
// "Código de activo" es texto libre (no se vincula al catálogo de
// InventarioEquipo), siguiendo el mismo criterio que "Elemento" en Tickets:
// el dato de origen no siempre coincide exactamente con el inventario.
public class AuditoriaEquipo
{
    public int Id { get; set; }

    // Folio del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones.
    public string? Folio { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaRealizada { get; set; }

    public string? Area { get; set; }
    public string? Almacen { get; set; }
    public string? CodigoActivo { get; set; }
    public string? DescripcionActivo { get; set; }
    public string? Responsable { get; set; }
    public string? Tipo { get; set; }

    // Cantidad de elementos/piezas revisadas.
    public int? Revisados { get; set; }

    public string Estado { get; set; } = "Programada";

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
