namespace WasionOne.API.DTOs;

public class TicketDto
{
    public int Id { get; set; }
    public string? IdTicketOrigen { get; set; }
    public int AreaUbicacionId { get; set; }

    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? Subcategoria { get; set; }
    public string? Tipo { get; set; }
    public string? TipoAsociacion { get; set; }
    public string? Etiquetas { get; set; }

    public string Prioridad { get; set; } = string.Empty;
    public string? Urgencia { get; set; }
    public string? Impacto { get; set; }

    public string Estado { get; set; } = string.Empty;
    public string? EstadoAprobacion { get; set; }
    public string? EstadoResolucion { get; set; }
    public string? EstadoPrimeraRespuesta { get; set; }

    public string? Grupo { get; set; }
    public string? Agente { get; set; }
    public string? Origen { get; set; }

    public string NombreSolicitante { get; set; } = string.Empty;
    public string? CorreoSolicitante { get; set; }
    public string? UbicacionSolicitante { get; set; }
    public bool SolicitanteVip { get; set; }

    public string? Elemento { get; set; }
    public string? AnyDeskEquipo { get; set; }
    public string? DepartamentoOrigen { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public DateTime? FechaCierre { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }
    public string? TiempoInicialRespuesta { get; set; }

    public decimal? TiempoPrimeraRespuestaHoras { get; set; }
    public decimal? TiempoResolucionHoras { get; set; }
    public decimal? RegistroTiempo { get; set; }

    public int? InteraccionesCliente { get; set; }
    public int? InteraccionesAgente { get; set; }

    public string? NotaResolucion { get; set; }
    public string? ResultadoEncuesta { get; set; }

    public DateTime? FechaImportacion { get; set; }
}

// DTO para captura manual desde la pantalla (no para lo importado). Solo
// pide los campos que tiene sentido llenar al abrir un ticket nuevo; el
// resto se va completando conforme el agente lo trabaja (ver
// TicketActualizarDto) o llega ya lleno desde la importación.
public class TicketCrearDto
{
    public int AreaUbicacionId { get; set; }

    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? Subcategoria { get; set; }
    public string? Tipo { get; set; }
    public string? Etiquetas { get; set; }

    public string Prioridad { get; set; } = string.Empty;
    public string? Urgencia { get; set; }
    public string? Impacto { get; set; }

    public string? Grupo { get; set; }
    public string? Agente { get; set; }
    public string? Origen { get; set; }

    public string NombreSolicitante { get; set; } = string.Empty;
    public string? CorreoSolicitante { get; set; }
    public string? UbicacionSolicitante { get; set; }
    public bool SolicitanteVip { get; set; }

    public string? Elemento { get; set; }
    public string? AnyDeskEquipo { get; set; }

    public DateTime? FechaVencimiento { get; set; }
}

// DTO para actualizar un ticket ya existente (cambio de estado, avance,
// cierre). Se usa tanto para capturados a mano como para importados.
public class TicketActualizarDto
{
    public string Estado { get; set; } = string.Empty;
    public string? EstadoResolucion { get; set; }
    public string? EstadoAprobacion { get; set; }
    public string? Grupo { get; set; }
    public string? Agente { get; set; }
    public string? NotaResolucion { get; set; }
    public string? ResultadoEncuesta { get; set; }
}

// Resultado de una importación masiva desde Excel.
public class TicketImportarResultadoDto
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
