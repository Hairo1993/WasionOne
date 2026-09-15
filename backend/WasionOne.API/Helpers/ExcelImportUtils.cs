using System.Globalization;
using System.Text;
using ClosedXML.Excel;

namespace WasionOne.API.Helpers;

// Utilidades compartidas para leer archivos Excel (.xlsx) exportados de
// otros sistemas, usadas por todos los módulos de IT que soportan
// importación masiva (Tickets, Incidentes críticos, Respaldos, Pláticas
// de ciberseguridad, Auditorías de equipos electrónicos).
//
// Nota: TicketService.cs tiene su propia copia privada de Normalizar()
// (se escribió antes de que existiera este helper compartido) — no se
// tocó para no arriesgar una regresión en un módulo ya verificado en
// producción. Los módulos nuevos sí usan este helper.
public static class ExcelImportUtils
{
    // Normaliza texto para comparar encabezados/valores sin importar
    // acentos, mayúsculas ni espacios/paréntesis/puntuación (ej. "ID de
    // Falla" -> "iddefalla").
    public static string Normalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        var descompuesto = texto.Normalize(NormalizationForm.FormD);
        var sinAcentos = new string(descompuesto
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        var soloAlfanumerico = new string(sinAcentos.Where(char.IsLetterOrDigit).ToArray());
        return soloAlfanumerico.ToLowerInvariant();
    }

    // Lee la fila de encabezados de una hoja y regresa un mapa de
    // encabezado normalizado -> número de columna.
    public static Dictionary<string, int> LeerIndicePorEncabezado(IXLRow filaEncabezado)
    {
        var indice = new Dictionary<string, int>();
        foreach (var celda in filaEncabezado.CellsUsed())
        {
            indice[Normalizar(celda.GetString())] = celda.Address.ColumnNumber;
        }

        return indice;
    }
}

// Envuelve una fila de Excel + el mapa de columnas del módulo, para leer
// valores por "nombre lógico" (ej. "Descripcion") con conversión
// tolerante, sin repetir la lógica de búsqueda de columna en cada
// servicio.
public class ExcelFilaLectora
{
    private readonly IXLRow _fila;
    private readonly IReadOnlyDictionary<string, int> _indicePorEncabezado;
    private readonly IReadOnlyDictionary<string, string> _columnas;

    // columnas: nombre lógico (ej. "IdFalla") -> encabezado normalizado
    // esperado en el archivo (ej. "iddefalla", ya pasado por Normalizar).
    public ExcelFilaLectora(
        IXLRow fila,
        IReadOnlyDictionary<string, int> indicePorEncabezado,
        IReadOnlyDictionary<string, string> columnas)
    {
        _fila = fila;
        _indicePorEncabezado = indicePorEncabezado;
        _columnas = columnas;
    }

    private int? IndiceDe(string claveLogica)
    {
        if (!_columnas.TryGetValue(claveLogica, out var encabezadoEsperado))
        {
            throw new KeyNotFoundException($"'{claveLogica}' no está definida en el mapa de columnas de este módulo.");
        }

        return _indicePorEncabezado.TryGetValue(encabezadoEsperado, out var indice) ? indice : (int?)null;
    }

    public string? Texto(string claveLogica)
    {
        var indice = IndiceDe(claveLogica);
        if (indice is null)
        {
            return null;
        }

        var valor = _fila.Cell(indice.Value).GetString().Trim();
        return string.IsNullOrWhiteSpace(valor) ? null : valor;
    }

    public DateTime? Fecha(string claveLogica)
    {
        var indice = IndiceDe(claveLogica);
        if (indice is null)
        {
            return null;
        }

        var celda = _fila.Cell(indice.Value);
        if (celda.TryGetValue(out DateTime valorFecha))
        {
            return valorFecha;
        }

        var texto = celda.GetString().Trim();
        if (string.IsNullOrWhiteSpace(texto))
        {
            return null;
        }

        return DateTime.TryParse(texto, CultureInfo.GetCultureInfo("es-MX"), DateTimeStyles.None, out var parseado)
            ? parseado
            : null;
    }

    // Para columnas de solo hora del día (ej. "Hora de Inicio"), que en
    // Excel pueden venir como hora pura, como fecha+hora completa, o como
    // texto "HH:mm".
    public TimeSpan? Hora(string claveLogica)
    {
        var indice = IndiceDe(claveLogica);
        if (indice is null)
        {
            return null;
        }

        var celda = _fila.Cell(indice.Value);
        if (celda.TryGetValue(out TimeSpan valorHora))
        {
            return valorHora;
        }

        if (celda.TryGetValue(out DateTime valorFechaHora))
        {
            return valorFechaHora.TimeOfDay;
        }

        var texto = celda.GetString().Trim();
        if (string.IsNullOrWhiteSpace(texto))
        {
            return null;
        }

        return TimeSpan.TryParse(texto, CultureInfo.GetCultureInfo("es-MX"), out var parseado) ? parseado : null;
    }

    public decimal? Numero(string claveLogica)
    {
        var indice = IndiceDe(claveLogica);
        if (indice is null)
        {
            return null;
        }

        var celda = _fila.Cell(indice.Value);
        if (celda.TryGetValue(out double valorNumero))
        {
            return (decimal)valorNumero;
        }

        var texto = celda.GetString().Trim();
        if (string.IsNullOrWhiteSpace(texto))
        {
            return null;
        }

        return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.GetCultureInfo("es-MX"), out var parseado)
            ? parseado
            : null;
    }

    public int? Entero(string claveLogica)
    {
        var valor = Numero(claveLogica);
        return valor.HasValue ? (int)valor.Value : null;
    }

    public bool Booleano(string claveLogica)
    {
        var texto = Texto(claveLogica)?.ToLowerInvariant();
        return texto is "si" or "sí" or "yes" or "true" or "1" or "verdadero";
    }
}
