namespace WasionOne.API.DTOs;

public class DepartamentoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DireccionId { get; set; }
}

public class DepartamentoCrearDto
{
    public string Nombre { get; set; } = string.Empty;
    public int DireccionId { get; set; }
}

public class DepartamentoActualizarDto
{
    public string Nombre { get; set; } = string.Empty;
    public int DireccionId { get; set; }
}
