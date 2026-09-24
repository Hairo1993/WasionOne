namespace WasionOne.API.DTOs;

public class DisponibilidadAbastecimientoDto
{
    public int Id { get; set; }
    public DateTime FechaEntrega { get; set; }
    public string? Departamento { get; set; }
    public string? Material { get; set; }
    public string? Especificar { get; set; }
    public string? Unidad { get; set; }
    public decimal? CantidadEntregada { get; set; }
    public string? Comentarios { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class DisponibilidadAbastecimientoCrearDto
{
    public DateTime FechaEntrega { get; set; }
    public string? Departamento { get; set; }
    public string? Material { get; set; }
    public string? Especificar { get; set; }
    public string? Unidad { get; set; }
    public decimal? CantidadEntregada { get; set; }
    public string? Comentarios { get; set; }
    public int AreaUbicacionId { get; set; }
}

// Este módulo no tiene llave de negocio ni IdOrigen (ver Models/
// DisponibilidadAbastecimiento.cs), así que no hay un campo "identidad" que
// excluir aquí: se puede editar cualquier campo del registro.
public class DisponibilidadAbastecimientoActualizarDto
{
    public DateTime FechaEntrega { get; set; }
    public string? Departamento { get; set; }
    public string? Material { get; set; }
    public string? Especificar { get; set; }
    public string? Unidad { get; set; }
    public decimal? CantidadEntregada { get; set; }
    public string? Comentarios { get; set; }
    public int AreaUbicacionId { get; set; }
}

public class DisponibilidadAbastecimientoImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
