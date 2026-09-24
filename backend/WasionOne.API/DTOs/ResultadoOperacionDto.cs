namespace WasionOne.API.DTOs;

// Resultado de una operación de eliminación. Exito=false + Error=null
// significa "no existe" (404); Exito=false + Error con texto significa
// "existe pero no se puede eliminar" (409, por ejemplo por registros
// dependientes) — el Controller distingue ambos casos con ese criterio.
public class ResultadoOperacionDto
{
    public bool Exito { get; set; }
    public string? Error { get; set; }
}
