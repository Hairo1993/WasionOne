using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IImportacionMasivaService
{
    // Módulos (de los ~31 con importación) que el usuario de la petición
    // actual tiene asignados — el frontend los lista antes de descargar,
    // para que el usuario sepa qué pestañas va a traer el archivo.
    IReadOnlyList<ModuloDisponibleDto> ObtenerModulosDisponibles();

    // Un solo libro de Excel con una pestaña por cada módulo asignado al
    // usuario actual (mismas columnas + fila de ejemplo que su plantilla
    // individual). Si el usuario no tiene ningún módulo de captura con
    // importación asignado, regresa igual un archivo válido con una sola
    // hoja explicándolo (nunca un archivo vacío/corrupto).
    byte[] GenerarPlantillaCombinada();

    // Lee un archivo combinado ya lleno. Por cada pestaña cuyo nombre
    // coincide con la de un módulo Y el usuario actual tiene ese módulo
    // asignado, reutiliza — sin duplicarla — la importación individual de
    // ese módulo. Pestañas que no coinciden con ningún módulo conocido, o
    // que pertenecen a un módulo no asignado al usuario, se reportan como
    // omitidas y no tocan la base de datos.
    Task<ImportacionMasivaResultadoDto> ImportarCombinadoAsync(Stream archivoExcel);
}
