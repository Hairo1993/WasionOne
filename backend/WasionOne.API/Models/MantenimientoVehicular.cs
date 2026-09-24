namespace WasionOne.API.Models;

// Registro único por vehículo — se EDITA conforme cambian sus datos
// (kilometraje, estatus), no es un histórico de versiones. Mismo patrón
// que Estacionamiento (llave de negocio única, aquí el VIN).
public class MantenimientoVehicular
{
    public int Id { get; set; }

    // Llave de negocio: identifica al vehículo de forma única. Un
    // registro por VIN, que se actualiza en vez de duplicarse.
    public string Vin { get; set; } = string.Empty;

    // Fecha del último registro/actualización.
    public DateTime? Fecha { get; set; }

    public string? VehiculoTipo { get; set; }

    public int? KilometrajeUltimoServicio { get; set; }
    public int? KilometrajeActual { get; set; }

    // Kilometraje al que toca el próximo servicio.
    public int? ProximoServicio { get; set; }

    public string? Estatus { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
