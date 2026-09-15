namespace WasionOne.API.Models;

public class Ubicacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<AreaUbicacion> AreaUbicaciones { get; set; } = new List<AreaUbicacion>();
}