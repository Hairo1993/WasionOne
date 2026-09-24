namespace WasionOne.API.Models;

// Acceso manual de un usuario a un módulo de captura específico (además
// de su nodo en la jerarquía Dirección/Departamento/Área). ModuloClave es
// una de las constantes de Modulos. Selección manual por usuario, según
// decisión del usuario (14/sep/2026): el nodo de jerarquía no otorga
// acceso automático a los módulos de esa Área.
public class UsuarioModulo
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string ModuloClave { get; set; } = string.Empty;
}
