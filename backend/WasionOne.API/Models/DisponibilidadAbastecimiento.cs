namespace WasionOne.API.Models;

// Registro por entrega de material — un registro por evento. El sistema de
// origen no trae ningún "ID" (ni se espera que lo traiga en el futuro), así
// que no hay llave de deduplicación: cada fila importada siempre crea un
// registro nuevo (no hay actualiza-o-crea en este módulo, a diferencia de
// ReunionProveedor/CumplimientoPrograma). Es una simplificación deliberada,
// no un descuido: aquí genuinamente no hay nada sobre lo cual armar una
// llave.
public class DisponibilidadAbastecimiento
{
    public int Id { get; set; }

    public DateTime FechaEntrega { get; set; }

    // Texto libre — NO está ligado al catálogo real de Departamento (puede
    // no coincidir con Recursos Humanos/Seguridad/Proyectos/Administración).
    public string? Departamento { get; set; }

    public string? Material { get; set; }
    public string? Especificar { get; set; }
    public string? Unidad { get; set; }
    public decimal? CantidadEntregada { get; set; }
    public string? Comentarios { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
