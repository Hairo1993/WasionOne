namespace WasionOne.API.Models;

// Resumen agregado de pruebas de alcoholimetría por Turno/Planta/día
// (no identifica a cada persona individualmente, a diferencia de
// Dopings). Un registro por combinación Planta+Fecha+Turno.
public class Alcoholimetria
{
    public int Id { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime Fecha { get; set; }
    public string Turno { get; set; } = string.Empty;

    public int Cantidad { get; set; }
    public int Positivo { get; set; }
    public int Negativo { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
