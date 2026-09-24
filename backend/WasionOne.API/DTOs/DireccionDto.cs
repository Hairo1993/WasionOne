namespace WasionOne.API.DTOs;

public class DireccionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class DireccionCrearDto
{
    public string Nombre { get; set; } = string.Empty;
}

public class DireccionActualizarDto
{
    public string Nombre { get; set; } = string.Empty;
}
