namespace WasionOne.API.DTOs;

public class LockerDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public DateTime? Fecha { get; set; }
    public string? NumeroLocker { get; set; }
    public string? NoNomina { get; set; }
    public string? Nombre { get; set; }
    public int AreaUbicacionId { get; set; }
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraTermino { get; set; }
    public string Resultado { get; set; } = string.Empty;
    public string? Detalles { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class LockerCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime? Fecha { get; set; }
    public string? NumeroLocker { get; set; }
    public string? NoNomina { get; set; }
    public string? Nombre { get; set; }
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraTermino { get; set; }
    public string Resultado { get; set; } = "Sin novedad";
    public string? Detalles { get; set; }
}

public class LockerActualizarDto
{
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraTermino { get; set; }
    public string Resultado { get; set; } = string.Empty;
    public string? Detalles { get; set; }
}

public class LockerImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
