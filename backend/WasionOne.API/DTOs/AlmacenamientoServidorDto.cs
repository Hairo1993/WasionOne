namespace WasionOne.API.DTOs;

public class AlmacenamientoServidorDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Servidor { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public string? Unidad { get; set; }
    public decimal? CapacidadTotalGb { get; set; }
    public decimal? EspacioUtilizadoGb { get; set; }
    public decimal? EspacioDisponibleGb { get; set; }
    public decimal AlmacenamientoUtilizadoPorcentaje { get; set; }
    public decimal? Umbral { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class AlmacenamientoServidorCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Servidor { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public string? Unidad { get; set; }
    public decimal? CapacidadTotalGb { get; set; }
    public decimal? EspacioUtilizadoGb { get; set; }
    public decimal? EspacioDisponibleGb { get; set; }
    public decimal AlmacenamientoUtilizadoPorcentaje { get; set; }
    public decimal? Umbral { get; set; }
    public string? Estado { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class AlmacenamientoServidorActualizarDto
{
    public decimal? EspacioUtilizadoGb { get; set; }
    public decimal? EspacioDisponibleGb { get; set; }
    public decimal AlmacenamientoUtilizadoPorcentaje { get; set; }
    public decimal? Umbral { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class AlmacenamientoServidorImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
