namespace WasionOne.API.DTOs;

public class CumplimientoEppDto
{
    public int Id { get; set; }
    public string IdEpp { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public string? Talla { get; set; }
    public int? VidaUtilCantidad { get; set; }
    public string? VidaUtilUnidad { get; set; }
    public int? Minimo { get; set; }
    public int? Maximo { get; set; }
    public int? Existencias { get; set; }
    public int? Solicitud { get; set; }
    public int? AbastecimientoStock { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class CumplimientoEppCrearDto
{
    public string IdEpp { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public string? Talla { get; set; }
    public int? VidaUtilCantidad { get; set; }
    public string? VidaUtilUnidad { get; set; }
    public int? Minimo { get; set; }
    public int? Maximo { get; set; }
    public int? Existencias { get; set; }
    public int? Solicitud { get; set; }
    public int? AbastecimientoStock { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime FechaRegistro { get; set; }
}

// No incluye IdEpp/FechaRegistro (la llave) ni AreaUbicacionId (la Planta
// donde ocurrió el registro histórico) — mismo criterio que
// ActualizacionEquipoCriticoActualizarDto: una vez creado el registro de
// una fecha, sólo se corrigen los datos propios de esa versión del EPP.
public class CumplimientoEppActualizarDto
{
    public string Descripcion { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public string? Talla { get; set; }
    public int? VidaUtilCantidad { get; set; }
    public string? VidaUtilUnidad { get; set; }
    public int? Minimo { get; set; }
    public int? Maximo { get; set; }
    public int? Existencias { get; set; }
    public int? Solicitud { get; set; }
    public int? AbastecimientoStock { get; set; }
}

public class CumplimientoEppImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
