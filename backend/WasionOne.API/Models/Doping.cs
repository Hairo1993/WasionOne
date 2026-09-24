namespace WasionOne.API.Models;

// Registro de una prueba de doping individual. Un registro por persona
// probada (a diferencia de Alcoholimetría, que es un agregado).
public class Doping
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones. Opcional: si el archivo no lo
    // trae, cada fila crea un registro nuevo.
    public string? IdOrigen { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime? Fecha { get; set; }
    public string? Turno { get; set; }
    public string? NoNomina { get; set; }
    public string? Nombre { get; set; }

    // Área/departamento del empleado — texto libre (no hay catálogo para esto).
    public string? Area { get; set; }

    public string Resultado { get; set; } = "Negativo";

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
