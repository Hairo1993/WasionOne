namespace WasionOne.API.Models;

// Ticket de soporte de IT. Vive dentro del nodo operativo AreaUbicacion
// (el Área IT en una Ubicación específica, ej. "IT en Planta 1").
//
// La estructura de campos replica el reporte que exporta el sistema de
// mesa de ayuda que ya usa la empresa, para poder importar el histórico
// de tickets desde un archivo Excel (ver TicketService.ImportarDesdeExcelAsync).
// Los campos de catálogo (Categoría, Prioridad, Estado, etc.) se guardan
// como texto libre a propósito: así un ticket importado nunca falla por
// traer un valor que no esté en las listas fijas que ofrece la pantalla
// de captura manual (ver TicketsComponent en el frontend).
public class Ticket
{
    public int Id { get; set; }

    // Identificador del ticket en el sistema de origen ("ID del Ticket").
    // Es la llave para no importar el mismo ticket dos veces y para poder
    // reimportar un archivo actualizado sin duplicar. Null en tickets
    // capturados aquí directamente (no importados).
    public string? IdTicketOrigen { get; set; }

    // Ubicación real del ticket dentro del catálogo (Ubicación = Planta 1,
    // CDMX, etc., igual que en Inventario). En tickets importados se
    // resuelve buscando el texto de la columna "Ubicación" contra el
    // catálogo de Ubicaciones.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // --- Identificación / clasificación ---
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? Subcategoria { get; set; }
    public string? Tipo { get; set; } // Incidente / Solicitud de servicio / Problema / Cambio
    public string? TipoAsociacion { get; set; }
    public string? Etiquetas { get; set; }

    // --- Prioridad / clasificación de urgencia ---
    public string Prioridad { get; set; } = string.Empty; // Baja/Media/Alta/Urgente
    public string? Urgencia { get; set; }
    public string? Impacto { get; set; }

    // --- Estado / flujo ---
    public string Estado { get; set; } = "Abierto";
    public string? EstadoAprobacion { get; set; }
    public string? EstadoResolucion { get; set; }
    public string? EstadoPrimeraRespuesta { get; set; }

    // --- Asignación ---
    public string? Grupo { get; set; }
    public string? Agente { get; set; }
    public string? Origen { get; set; } // Email/Teléfono/Portal/Chat

    // --- Solicitante ---
    public string NombreSolicitante { get; set; } = string.Empty;
    public string? CorreoSolicitante { get; set; }

    // Texto libre: puede no coincidir con el catálogo interno (el
    // solicitante puede estar en una ubicación distinta a la que atiende
    // el ticket).
    public string? UbicacionSolicitante { get; set; }
    public bool SolicitanteVip { get; set; }

    // --- Elemento / equipo afectado (texto libre, no ligado al inventario) ---
    public string? Elemento { get; set; }
    public string? AnyDeskEquipo { get; set; }

    // Departamento tal como viene del sistema de origen (texto libre: no
    // necesariamente coincide con el catálogo interno de Departamentos).
    public string? DepartamentoOrigen { get; set; }

    // --- Fechas / SLA ---
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaVencimiento { get; set; }
    public DateTime? FechaCierre { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }

    // Guardado como texto porque el sistema de origen no deja claro si es
    // una duración o una marca de tiempo; se conserva tal cual venga.
    public string? TiempoInicialRespuesta { get; set; }

    // --- Métricas de tiempo (en horas) ---
    public decimal? TiempoPrimeraRespuestaHoras { get; set; }
    public decimal? TiempoResolucionHoras { get; set; }
    public decimal? RegistroTiempo { get; set; }

    // --- Interacciones ---
    public int? InteraccionesCliente { get; set; }
    public int? InteraccionesAgente { get; set; }

    // --- Resolución / encuesta ---
    public string? NotaResolucion { get; set; }
    public string? ResultadoEncuesta { get; set; }

    // --- Control de importación ---
    // Null si el ticket se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
