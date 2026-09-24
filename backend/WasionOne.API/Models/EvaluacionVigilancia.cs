namespace WasionOne.API.Models;

// Evaluación mensual de vigilancia — un registro por combinación
// Planta + Proveedor + Fecha (mes evaluado). Igual que Alcoholimetría, no
// hay un "ID" de origen natural en el Excel, así que esa combinación hace
// las veces de llave de deduplicación (reimportar el mismo mes/proveedor
// actualiza en vez de duplicar; el alta manual duplicada se rechaza).
public class EvaluacionVigilancia
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }
    public string Proveedor { get; set; } = string.Empty;

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Los 10 criterios de evaluación, calificados numéricamente. Nulables
    // porque no siempre se califican los 10 en cada evaluación.
    public decimal? Cobertura { get; set; }
    public decimal? Expedientes { get; set; }
    public decimal? Uniformidad { get; set; }
    public decimal? Reuniones { get; set; }
    public decimal? Equipamiento { get; set; }
    public decimal? Supervision { get; set; }
    public decimal? CambiosSolicitados { get; set; }
    public decimal? Procedimientos { get; set; }
    public decimal? Capacitacion { get; set; }
    public decimal? Acuerdos { get; set; }

    // Calculado por el sistema como la suma de los 10 criterios (tratando
    // los que no se calificaron como 0) — nunca se acepta del cliente ni
    // del Excel de origen, aunque el Excel traiga su propia columna Total.
    public decimal Total { get; set; }

    public string? Observaciones { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
