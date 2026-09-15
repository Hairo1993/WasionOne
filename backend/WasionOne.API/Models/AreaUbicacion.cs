namespace WasionOne.API.Models;

// Nodo hoja de la jerarquía Dirección -> Departamento -> Área -> Ubicación.
// Representa la instancia operativa real de un Área en una Ubicación
// (ej. "IT" en "Planta 1"), que es donde se registrará la información
// de cada pantalla de gestión (Fase 2 en adelante).
// (Antes se llamaba "SubAreaUbicacion", renombrado junto con Área.)
public class AreaUbicacion
{
    public int Id { get; set; }

    public int AreaId { get; set; }
    public Area? Area { get; set; }

    public int UbicacionId { get; set; }
    public Ubicacion? Ubicacion { get; set; }
}