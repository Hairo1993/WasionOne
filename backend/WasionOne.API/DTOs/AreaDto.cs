namespace WasionOne.API.DTOs;

public class AreaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
}

public class AreaCrearDto
{
    public string Nombre { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
}