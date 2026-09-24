namespace WasionOne.API.DTOs;

public class EstacionamientoDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public string NoMarbete { get; set; } = string.Empty;
    public string? Colaborador { get; set; }
    public string? Area { get; set; }
    public int AreaUbicacionId { get; set; }
    public bool MultiPlanta { get; set; }
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
    public DateTime? FechaRegistro { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class EstacionamientoCrearDto
{
    public string NoMarbete { get; set; } = string.Empty;
    public string? Colaborador { get; set; }
    public string? Area { get; set; }
    public int AreaUbicacionId { get; set; }
    public bool MultiPlanta { get; set; }
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
}

public class EstacionamientoActualizarDto
{
    public string? Colaborador { get; set; }
    public string? Area { get; set; }
    public int AreaUbicacionId { get; set; }
    public bool MultiPlanta { get; set; }
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
}

public class EstacionamientoImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
