using ClosedXML.Excel;

namespace WasionOne.API.Helpers;

// Utilidades compartidas para generar plantillas de Excel descargables
// (24/sep/2026), a pedido del usuario: cada módulo que ya soporta
// importación masiva (ver ExcelImportUtils) ahora también ofrece un botón
// "Descargar plantilla" con las columnas esperadas + una fila de ejemplo,
// para reducir errores de captura. UN SOLO lugar genera el archivo (este
// helper) — cada módulo solo aporta su lista de columnas, no repite la
// lógica de construir el .xlsx.
public static class ExcelPlantillaUtils
{
    public const string MimeTypeXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    // Una columna de la plantilla: el encabezado tal como debe verse en el
    // Excel (con acentos/espacios — NO el texto normalizado que usa
    // ExcelImportUtils.Normalizar para comparar, que es solo para
    // matching interno) y un valor de ejemplo que ilustra el formato
    // esperado (fecha con el formato correcto, un valor de catálogo
    // válido, "Sí"/"No" para booleanos, etc.).
    public readonly record struct ColumnaPlantilla(string Encabezado, string ValorEjemplo);

    // Genera un libro de un solo hoja: fila 1 = encabezados (negrita, con
    // relleno de color para distinguirlos), fila 2 = valores de ejemplo
    // (en cursiva/gris, para que se note que es solo una guía y no un
    // registro real). Regresa el libro ya armado — quien lo llama decide
    // si lo guarda a un archivo, lo agrega a un libro combinado (ver
    // ImportacionMasivaService) o lo serializa directo a la respuesta
    // HTTP.
    public static XLWorkbook GenerarLibro(string nombreHoja, IReadOnlyList<ColumnaPlantilla> columnas)
    {
        var libro = new XLWorkbook();
        AgregarHoja(libro, nombreHoja, columnas);
        return libro;
    }

    // Agrega la hoja de un módulo a un libro ya existente — usado por
    // ImportacionMasivaService para construir el archivo combinado con
    // una pestaña por indicador. `nombreHoja` ya debe venir saneado a las
    // reglas de Excel (máx. 31 caracteres, sin \ / ? * [ ] : — ver
    // SanearNombreHoja).
    public static IXLWorksheet AgregarHoja(XLWorkbook libro, string nombreHoja, IReadOnlyList<ColumnaPlantilla> columnas)
    {
        var hoja = libro.Worksheets.Add(SanearNombreHoja(nombreHoja));

        for (var i = 0; i < columnas.Count; i++)
        {
            var columna = i + 1;
            var celdaEncabezado = hoja.Cell(1, columna);
            celdaEncabezado.Value = columnas[i].Encabezado;
            celdaEncabezado.Style.Font.Bold = true;
            celdaEncabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E5E9");

            var celdaEjemplo = hoja.Cell(2, columna);
            celdaEjemplo.Value = columnas[i].ValorEjemplo;
            celdaEjemplo.Style.Font.Italic = true;
            celdaEjemplo.Style.Font.FontColor = XLColor.FromHtml("#6B7680");
        }

        hoja.SheetView.FreezeRows(1);
        hoja.Columns(1, columnas.Count).AdjustToContents(1, 2);
        return hoja;
    }

    // Serializa un libro ya armado a bytes, listos para regresar como
    // archivo descargable (File(...) en el Controller) o para reempacar
    // en el archivo combinado.
    public static byte[] GuardarComoBytes(XLWorkbook libro)
    {
        using var memoria = new MemoryStream();
        libro.SaveAs(memoria);
        return memoria.ToArray();
    }

    // Extrae UNA hoja de un libro combinado ya cargado (ver
    // ImportacionMasivaService) como un archivo .xlsx independiente de una
    // sola hoja, en bytes — para poder reutilizar SIN TOCARLO el método
    // `ImportarDesdeExcelAsync(Stream, ...)` que cada uno de los ~31
    // servicios ya tenía antes de esta funcionalidad (todos esperan
    // recibir un archivo con la hoja en la primera posición). Así, la
    // importación combinada no duplica ni un poco de la lógica de lectura
    // de cada módulo — solo le arma un archivo de una sola hoja y le
    // llama exactamente igual que el endpoint individual de "importar" de
    // ese módulo.
    public static byte[] ExtraerHojaComoXlsx(IXLWorksheet hoja)
    {
        using var libroIndividual = new XLWorkbook();
        hoja.CopyTo(libroIndividual, hoja.Name);
        return GuardarComoBytes(libroIndividual);
    }

    // ¿La fila 2 de una hoja todavía trae exactamente los valores de
    // ejemplo con los que se generó la plantilla (nadie la tocó)? Se
    // compara celda por celda, sin importar mayúsculas/espacios extra.
    // Usado por ImportacionMasivaService para no crear un registro real a
    // partir de la fila de ejemplo cuando el usuario la deja tal cual —
    // sin asumir a ciegas que "la fila 2 siempre es el ejemplo", porque el
    // usuario pudo haberla borrado y escrito su primer dato real ahí.
    public static bool EsFilaDeEjemplo(IXLRow fila, IReadOnlyList<ColumnaPlantilla> columnas)
    {
        for (var i = 0; i < columnas.Count; i++)
        {
            var valorCelda = fila.Cell(i + 1).GetString().Trim();
            if (!string.Equals(valorCelda, columnas[i].ValorEjemplo.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    // Igual que ExtraerHojaComoXlsx, pero quitando primero una fila
    // específica (1-based) de la copia — usado para excluir la fila de
    // ejemplo antes de reutilizar la importación individual de un módulo,
    // sin alterar la hoja original del archivo combinado que subió el
    // usuario.
    public static byte[] ExtraerHojaComoXlsxSinFila(IXLWorksheet hoja, int numeroFilaAOmitir)
    {
        using var libroIndividual = new XLWorkbook();
        var copia = hoja.CopyTo(libroIndividual, hoja.Name);
        copia.Row(numeroFilaAOmitir).Delete();
        return GuardarComoBytes(libroIndividual);
    }

    // Nombre de hoja válido para Excel: máximo 31 caracteres, sin
    // \ / ? * [ ] : (se reemplazan por espacio) — usado tanto para la
    // plantilla individual como para cada pestaña del archivo combinado,
    // así ambos usan exactamente el mismo nombre de hoja y
    // ImportacionMasivaService puede reconocer cada pestaña por nombre al
    // leer un archivo combinado ya lleno.
    public static string SanearNombreHoja(string nombre)
    {
        var limpio = nombre;
        foreach (var caracter in new[] { '\\', '/', '?', '*', '[', ']', ':' })
        {
            limpio = limpio.Replace(caracter, ' ');
        }

        return limpio.Length > 31 ? limpio[..31] : limpio;
    }
}
