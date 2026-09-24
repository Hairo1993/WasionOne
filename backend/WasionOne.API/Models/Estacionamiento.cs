namespace WasionOne.API.Models;

// Registro único por colaborador con acceso a estacionamiento — se EDITA
// conforme cambian sus datos (vehículo, vigencias), no es un histórico de
// versiones. Mismo patrón que InventarioEquipo (llave de negocio única).
public class Estacionamiento
{
    public int Id { get; set; }

    // "ID" del sistema de origen, solo informativo (no es la llave de
    // deduplicación de este módulo — ver NoMarbete).
    public string? IdOrigen { get; set; }

    // Llave de negocio: identifica al colaborador de forma única. Un
    // registro por Marbete, que se actualiza en vez de duplicarse.
    public string NoMarbete { get; set; } = string.Empty;

    public string? Colaborador { get; set; }
    public string? Area { get; set; }

    // "Planta Base": ubicación física principal del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public bool MultiPlanta { get; set; }
    // Plantas adicionales cuando MultiPlanta = true — texto libre (no hay
    // catálogo de selección múltiple para esto todavía).
    public string? PlantasAdicionales { get; set; }

    public string? MarcaVehiculo1 { get; set; }
    public string? SubmarcaVehiculo1 { get; set; }
    public string? PlacasVehiculo1 { get; set; }
    public string? MarcaVehiculo2 { get; set; }
    public string? SubmarcaVehiculo2 { get; set; }
    public string? PlacasVehiculo2 { get; set; }

    public string? EstatusDocumentacion { get; set; }
    public string? Licencia { get; set; }
    public DateTime? VencimientoLicencia { get; set; }
    public string? TarjetaCirculacion { get; set; }
    public string? Seguro { get; set; }
    public DateTime? VencimientoSeguro { get; set; }

    public string? RegistradoPor { get; set; }

    // "Fecha Registro" del sistema de origen — dato de negocio, distinto
    // de FechaImportacion (control interno de importación, más abajo).
    public DateTime? FechaRegistro { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
