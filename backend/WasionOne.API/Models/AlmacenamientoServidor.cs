namespace WasionOne.API.Models;

// Registro por evento/revisión de % de almacenamiento usado en una unidad
// de un servidor, por Planta. Mismo criterio que los otros dos: una fila
// por checada, no un acumulado diario.
public class AlmacenamientoServidor
{
    public int Id { get; set; }

    public string? IdOrigen { get; set; }

    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow.Date;
    public TimeSpan Hora { get; set; }

    public string Servidor { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public string? Unidad { get; set; }

    public decimal? CapacidadTotalGb { get; set; }
    public decimal? EspacioUtilizadoGb { get; set; }
    public decimal? EspacioDisponibleGb { get; set; }
    public decimal AlmacenamientoUtilizadoPorcentaje { get; set; }

    // Umbral de alerta configurado para esta unidad/servidor (%).
    public decimal? Umbral { get; set; }

    // Valores esperados: "Normal", "Alerta", "Crítico".
    public string Estado { get; set; } = "Normal";

    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }

    public DateTime? FechaImportacion { get; set; }
}
