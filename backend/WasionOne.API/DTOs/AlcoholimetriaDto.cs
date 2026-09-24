namespace WasionOne.API.DTOs;

public class AlcoholimetriaDto
{
    public int Id { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public string Turno { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int Positivo { get; set; }
    public int Negativo { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class AlcoholimetriaCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public string Turno { get; set; } = string.Empty;
    public int? Cantidad { get; set; }
    public int Positivo { get; set; }
    public int Negativo { get; set; }
}

public class AlcoholimetriaActualizarDto
{
    public int? Cantidad { get; set; }
    public int Positivo { get; set; }
    public int Negativo { get; set; }
}

public class AlcoholimetriaImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
