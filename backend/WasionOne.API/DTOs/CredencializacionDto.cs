namespace WasionOne.API.DTOs;

public class CredencializacionDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Empresa { get; set; }
    public string? TipoAcceso { get; set; }
    public int AreaUbicacionId { get; set; }
    public string? Identificacion { get; set; }
    public string? PersonaQueVisita { get; set; }
    public string? MotivoVisita { get; set; }
    public string? AreaDeTrabajo { get; set; }
    public DateTime? FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaImportacion { get; set; }
}

public class CredencializacionCrearDto
{
    public int AreaUbicacionId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Empresa { get; set; }
    public string? TipoAcceso { get; set; }
    public string? Identificacion { get; set; }
    public string? PersonaQueVisita { get; set; }
    public string? MotivoVisita { get; set; }
    public string? AreaDeTrabajo { get; set; }
    public DateTime? FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
    public string Estado { get; set; } = "Dentro de instalaciones";
}

public class CredencializacionActualizarDto
{
    public DateTime? FechaHoraSalida { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class CredencializacionImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
