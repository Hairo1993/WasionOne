namespace WasionOne.API.Models;

// Registro de una revisión de locker. Un registro por revisión realizada.
public class Locker
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones. Opcional: si el archivo no lo
    // trae, cada fila crea un registro nuevo.
    public string? IdOrigen { get; set; }

    public DateTime? Fecha { get; set; }
    public string? NumeroLocker { get; set; }
    public string? NoNomina { get; set; }
    public string? Nombre { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraTermino { get; set; }

    public string Resultado { get; set; } = "Sin novedad";
    public string? Detalles { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
