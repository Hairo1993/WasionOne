using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IActualizacionEquipoCriticoService
{
    Task<IEnumerable<ActualizacionEquipoCriticoDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<ActualizacionEquipoCriticoDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro para ese Código en esa Fecha
    // de registro (se debe editar el existente, no duplicar).
    Task<ActualizacionEquipoCriticoDto?> CrearRegistroAsync(ActualizacionEquipoCriticoCrearDto dto);

    Task<ActualizacionEquipoCriticoDto?> ActualizarRegistroAsync(int id, ActualizacionEquipoCriticoActualizarDto dto);

    Task<ActualizacionEquipoCriticoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
