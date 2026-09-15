namespace WasionOne.API.Models;

// Segundo nivel de la jerarquía: Dirección -> Departamento -> Área -> Ubicación.
// (Antes se llamaba "Área"; se renombró a "Departamento" para que coincida
// con la terminología de negocio.)
public class Departamento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public int DireccionId { get; set; }
    public Direccion? Direccion { get; set; }

    public ICollection<Area> Areas { get; set; } = new List<Area>();
}