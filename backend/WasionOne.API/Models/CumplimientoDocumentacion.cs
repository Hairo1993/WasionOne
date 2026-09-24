namespace WasionOne.API.Models;

// Registro único por contrato/documento — se EDITA conforme avanza su
// proceso de firmas y vigencia, no es un histórico de versiones. Mismo
// patrón que Estacionamiento/MantenimientoVehicular (llave de negocio
// única, aquí el Code).
public class CumplimientoDocumentacion
{
    public int Id { get; set; }

    // Llave de negocio: identifica el contrato/documento de forma única.
    public string Code { get; set; } = string.Empty;

    public string? Proveedor { get; set; }
    public string? TipoContrato { get; set; }

    // Texto libre — etiqueta de área de negocio (ej. "Legal"/"Operaciones"),
    // NO está ligada al catálogo real de Área (puede no coincidir).
    public string? Area { get; set; }

    // Fuente: columna "Resp" del Excel de origen.
    public string? Responsable { get; set; }

    public decimal? MontoIvaIncluido { get; set; }
    public string? Moneda { get; set; }

    public bool FirmaDireccion { get; set; }
    public bool FirmaLegal { get; set; }
    public bool FirmaFinanzas { get; set; }

    // Fuente: columna "Aprobado Lenin /Li" del Excel de origen — un paso de
    // aprobación específico y nombrado así en el sistema de origen.
    public bool AprobadoLeninLi { get; set; }

    public bool Firmado { get; set; }

    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }

    // Texto libre, no booleano: puede traer notas sobre condiciones de renovación.
    public string? Renovacion { get; set; }

    public string? Estatus { get; set; }

    // Referencia/ruta de carpeta — texto libre, no es un archivo adjunto.
    public string? Carpeta { get; set; }

    // "Planta": ubicación física del catálogo. Se agrega por decisión
    // explícita aunque el Excel de origen no la traiga.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
