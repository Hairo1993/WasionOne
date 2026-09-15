namespace WasionOne.API.Models;

// Tercer nivel de la jerarquía: Dirección -> Departamento -> Área -> Ubicación.
// (Antes se llamaba "Sub-área"; se renombró a "Área" para que coincida con
// la terminología de negocio.)
public class Area
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public int DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }

    public ICollection<AreaUbicacion> AreaUbicaciones { get; set; } = new List<AreaUbicacion>();
}
