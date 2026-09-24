namespace WasionOne.API.DTOs;

// Importación masiva combinada (24/sep/2026): un solo archivo de Excel con
// una pestaña por indicador, para no tener que entrar módulo por módulo.
// Ver ImportacionMasivaService — reutiliza sin duplicar la lógica de
// importación que cada uno de los ~31 módulos ya tenía.

// Un módulo que el usuario actual puede incluir en su archivo combinado
// (los que tiene asignados). Se expone para que el frontend le diga al
// usuario, antes de descargar, qué pestañas va a traer el archivo.
public class ModuloDisponibleDto
{
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string NombreHoja { get; set; } = string.Empty;
}

// Resultado de UN módulo dentro del archivo combinado — mismos 4 números
// que ya regresaba cada módulo individualmente (Creados/Actualizados/
// Omitidos/Errores), con el nombre del módulo agregado para el reporte.
public class ResumenModuloImportadoDto
{
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string NombreHoja { get; set; } = string.Empty;
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}

public class ImportacionMasivaResultadoDto
{
    public List<ResumenModuloImportadoDto> Modulos { get; set; } = new();

    // Pestañas del archivo que no se reconocieron como ningún módulo (el
    // nombre no coincide con ninguna hoja de plantilla conocida) o que
    // pertenecen a un módulo que el usuario actual no tiene asignado —
    // ninguna de estas tocó la base de datos.
    public List<string> PestanasOmitidas { get; set; } = new();
}
