namespace WasionOne.API.DTOs;

public class AreaUbicacionDto
{
    public int Id { get; set; }
    public int AreaId { get; set; }
    public int UbicacionId { get; set; }
}

public class AreaUbicacionCrearDto
{
    public int AreaId { get; set; }
    public int UbicacionId { get; set; }
}