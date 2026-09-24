namespace WasionOne.API.Models;

// Registro de un vale de salida de materiales/artículos. Un registro por vale.
public class ValeSalida
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones. Opcional: si el archivo no lo
    // trae, cada fila crea un registro nuevo.
    public string? IdOrigen { get; set; }

    public string? Folio { get; set; }
    public string? Solicitante { get; set; }
    public string? Referencia { get; set; }
    public string? ConceptoMotivo { get; set; }
    public string? DetalleMotivo { get; set; }
    public bool ActivoFijo { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public string Estado { get; set; } = "Abierto";

    public DateTime? FechaVale { get; set; }
    public DateTime? FechaSalida { get; set; }
    public DateTime? FechaEstimadaRetorno { get; set; }
    public DateTime? FechaRealRetorno { get; set; }

    public string? ArticulosMateriales { get; set; }

    public string? RegistradoPor { get; set; }
    public string? CerradoPor { get; set; }

    // "Fecha Registro" del sistema de origen — dato de negocio, distinto
    // de FechaImportacion (control interno de importación, más abajo).
    public DateTime? FechaRegistro { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
