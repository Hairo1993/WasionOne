namespace WasionOne.API.Models;

public class Direccion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}