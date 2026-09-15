namespace WasionOne.API.Models;

// Roles definidos en el documento de requerimientos (sección 3):
// CEO/Presidencia ve todo; Director por Dirección; Responsable de
// Departamento; Responsable de Área.
public static class Roles
{
    public const string Ceo = "CEO";
    public const string Director = "Director";
    public const string ResponsableDepartamento = "ResponsableDepartamento";
    public const string ResponsableArea = "ResponsableArea";
}