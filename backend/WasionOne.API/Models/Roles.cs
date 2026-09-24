namespace WasionOne.API.Models;

// Roles definidos en el documento de requerimientos (sección 3):
// CEO/Presidencia ve todo; Director por Dirección; Responsable de
// Departamento; Responsable de Área.
//
// Superadmin (agregado 19/sep/2026) es un rol aparte, de sistema: no
// participa en la jerarquía de negocio (no tiene Dirección/Departamento/
// Área asignada) y es el único rol con acceso a las pantallas de
// administración (Usuarios y Catálogos). Se asigna al usuario semilla
// "admin".
public static class Roles
{
    public const string Ceo = "CEO";
    public const string Director = "Director";
    public const string ResponsableDepartamento = "ResponsableDepartamento";
    public const string ResponsableArea = "ResponsableArea";
    public const string Superadmin = "Superadmin";
}
