namespace WasionOne.API.DTOs;

public class UbicacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class UbicacionCrearDto
{
    public string Nombre { get; set; } = string.Empty;
}

public class UbicacionActualizarDto
{
    public string Nombre { get; set; } = string.Empty;
}
