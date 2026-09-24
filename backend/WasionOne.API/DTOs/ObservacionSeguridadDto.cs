namespace WasionOne.API.DTOs;

public class ObservacionSeguridadDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string? NNomina { get; set; }
    public string PersonaObservada { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Categorias { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class ObservacionSeguridadCrearDto
{
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string? NNomina { get; set; }
    public string PersonaObservada { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Categorias { get; set; } = string.Empty;
    public string Estado { get; set; } = "Abierto";
    public int AreaUbicacionId { get; set; }
}

// No incluye Folio — es la llave del registro y no se edita después de creado.
public class ObservacionSeguridadActualizarDto
{
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string? NNomina { get; set; }
    public string PersonaObservada { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Categorias { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
}

public class ObservacionSeguridadImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
