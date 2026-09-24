namespace WasionOne.API.DTOs;

// Interfaz compartida (24/sep/2026) que implementan los ~31
// "XImportarResultadoDto" de cada módulo de captura — todos ya tenían,
// de forma independiente, exactamente estas 5 propiedades (verificado
// antes de agregar esta interfaz, no se adivinó). Sirve para que
// ImportacionMasivaService pueda tratar el resultado de cualquier módulo
// de forma uniforme al construir el reporte combinado, sin tener que
// conocer el tipo concreto de cada uno.
public interface IResultadoImportacion
{
    int TotalFilas { get; set; }
    int Creados { get; set; }
    int Actualizados { get; set; }
    int Omitidos { get; set; }
    List<string> Errores { get; set; }
}
