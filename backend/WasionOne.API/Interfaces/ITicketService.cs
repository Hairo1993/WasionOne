using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketDto>> ObtenerTicketsAsync(int? areaUbicacionId);

    Task<TicketDto?> ObtenerTicketPorIdAsync(int id);

    Task<TicketDto> CrearTicketAsync(TicketCrearDto dto);

    Task<TicketDto?> ActualizarTicketAsync(int id, TicketActualizarDto dto);

    // Importa/actualiza tickets desde un archivo Excel (.xlsx) exportado
    // del sistema de mesa de ayuda. archivoExcel debe ser un stream
    // legible desde el inicio (ej. IFormFile.OpenReadStream()).
    Task<TicketImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
