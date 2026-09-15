namespace WasionOne.API.Models;

// Incidente crítico de IT: registro aparte y más detallado que un Ticket
// normal (causa raíz, contramedida, etc.). Vive dentro del nodo operativo
// AreaUbicacion (la Planta donde ocurrió), igual que Tickets/Inventario.
public class IncidenteCritico
{
    public int Id { get; set; }

    // "ID de Falla" del sistema de origen, si el registro fue importado.
    // Llave para no duplicar en reimportaciones.
    public string? IdFallaOrigen { get; set; }

    // "Planta": coincide con el catálogo de Ubicaciones.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow.Date;
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }
    public decimal? DuracionHoras { get; set; }

    // "Departamento": SÍ se liga al catálogo organizacional (a diferencia
    // de Tickets, donde ese campo es texto libre). Null significa que el
    // incidente impacta a TODOS los departamentos (el usuario confirmó que
    // el sistema de origen manda un valor tipo "Todos" para esos casos).
    public int? DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }

    // "Área" y "Línea": clasificación de planta/producción, independiente
    // del catálogo organizacional (Dirección > Departamento > Área) — por
    // eso quedan como texto libre y no como FK.
    public string? Area { get; set; }
    public string? Linea { get; set; }

    public string? Severidad { get; set; }
    public string? Tipo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Causa { get; set; }
    public string? Detalles { get; set; }
    public string? Contramedida { get; set; }

    // Valores esperados: "Abierto", "EnProceso", "Cerrado".
    public string Estado { get; set; } = "Abierto";

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
