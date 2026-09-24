namespace WasionOne.API.Models;

// Registro de hallazgo/seguimiento de un programa de cumplimiento — un
// registro por hallazgo. Mismo patrón que ReunionProveedor: "ID" del
// sistema de origen opcional (si el Excel lo trae, deduplica reimportando;
// si no, cada fila/captura manual crea un registro nuevo).
public class CumplimientoPrograma
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones. Opcional: si el archivo no lo
    // trae, cada fila crea un registro nuevo.
    public int? IdOrigen { get; set; }

    public string? AreaEvaluada { get; set; }

    // "Casa/Planta" en el sistema de origen: ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime FechaReporte { get; set; }

    // Fuente: "Hallazgo / observación".
    public string? Hallazgo { get; set; }

    // Fuente: "Seguimiento / acción realizada".
    public string? Seguimiento { get; set; }

    public DateTime? FechaCierre { get; set; }
    public string? Estatus { get; set; }
    public string? Prioridad { get; set; }
    public string? Responsable { get; set; }

    // Fuente: columna "tipo" (minúscula) del Excel de origen.
    public string? Tipo { get; set; }

    // Porcentaje de cumplimiento por hallazgo, capturado/importado tal
    // cual — a diferencia de otros campos de este lote, NO se calcula en
    // el backend (el sistema de origen ya lo trae calculado por fila).
    public decimal? CumplimientoGeneralPorcentaje { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
