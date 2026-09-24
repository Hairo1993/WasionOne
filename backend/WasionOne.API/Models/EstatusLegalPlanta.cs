namespace WasionOne.API.Models;

// Bitácora histórica de Estatus legal de planta — Seguridad e Higiene. Un
// mismo requerimiento legal de una Planta se re-evalúa periódicamente
// (según su Frecuencia), así que la llave de unicidad es la combinación
// Planta + Requerimiento legal + Última fecha de realización (mismo
// criterio de bitácora histórica que Cumplimiento de EPP/Actualizaciones
// de equipos críticos), no la Planta+Requerimiento por sí solos.
public class EstatusLegalPlanta
{
    public int Id { get; set; }

    // "Planta": ubicación física del catálogo. Obligatoria y parte de la llave.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Texto libre.
    public string RequerimientoLegal { get; set; } = string.Empty;

    // Texto libre.
    public string Autoridad { get; set; } = string.Empty;

    // Texto libre.
    public string Frecuencia { get; set; } = string.Empty;

    // Fecha del evento/registro — parte de la llave.
    public DateTime UltimaFechaRealizacion { get; set; }

    // Texto libre (ej. "Cumple", "Vencido", "Pendiente") — no es catálogo
    // fijo, el usuario pidió texto libre para este campo específicamente.
    public string Estatus { get; set; } = string.Empty;

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
