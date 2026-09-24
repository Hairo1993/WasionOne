namespace WasionOne.API.DTOs;

public class MantenimientoVehicularDto
{
    public int Id { get; set; }
    public string Vin { get; set; } = string.Empty;
    public DateTime? Fecha { get; set; }
    public string? VehiculoTipo { get; set; }
    public int? KilometrajeUltimoServicio { get; set; }
    public int? KilometrajeActual { get; set; }
    public int? ProximoServicio { get; set; }
    public string? Estatus { get; set; }
    public int AreaUbicacionId { get; set; }

    // Calculado: ProximoServicio - KilometrajeActual. Nunca se persiste ni
    // se acepta del cliente/Excel — se calcula en cada lectura.
    public int? KmRestantes { get; set; }

    public DateTime? FechaImportacion { get; set; }
}

public class MantenimientoVehicularCrearDto
{
    public string Vin { get; set; } = string.Empty;
    public DateTime? Fecha { get; set; }
    public string? VehiculoTipo { get; set; }
    public int? KilometrajeUltimoServicio { get; set; }
    public int? KilometrajeActual { get; set; }
    public int? ProximoServicio { get; set; }
    public string? Estatus { get; set; }
    public int AreaUbicacionId { get; set; }
}

// No incluye Vin — es la llave del registro y no se edita después de creado.
public class MantenimientoVehicularActualizarDto
{
    public DateTime? Fecha { get; set; }
    public string? VehiculoTipo { get; set; }
    public int? KilometrajeUltimoServicio { get; set; }
    public int? KilometrajeActual { get; set; }
    public int? ProximoServicio { get; set; }
    public string? Estatus { get; set; }
    public int AreaUbicacionId { get; set; }
}

public class MantenimientoVehicularImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
