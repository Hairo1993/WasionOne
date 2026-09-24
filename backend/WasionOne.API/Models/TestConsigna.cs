namespace WasionOne.API.Models;

// Registro de un test de consignas (evaluación de conocimiento de
// procedimientos) aplicado al personal de vigilancia. Un registro por
// test aplicado.
public class TestConsigna
{
    public int Id { get; set; }

    // "No." del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones.
    public string? IdOrigen { get; set; }

    public DateTime? Fecha { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public string ResultadoTest { get; set; } = "Aprobado";

    public string? AreaInvolucrada { get; set; }
    public string? ProcedimientoInvolucrado { get; set; }
    public string? Proveedor { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
