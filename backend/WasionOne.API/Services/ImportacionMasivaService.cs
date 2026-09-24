using ClosedXML.Excel;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

// Importación masiva combinada (24/sep/2026): "quiero que también pueda
// subirse un excel con todas las plantillas" — un solo archivo con una
// pestaña por indicador, en vez de entrar módulo por módulo. Este
// servicio NO reimplementa la lógica de importación de cada módulo — la
// reutiliza tal cual (inyecta los ~31 I*Service existentes) extrayendo la
// pestaña de cada módulo como un archivo de una sola hoja (ver
// ExcelPlantillaUtils.ExtraerHojaComoXlsx) y llamando a su
// ImportarDesdeExcelAsync ya existente, sin tocarlo.
//
// El nombre de hoja de cada módulo es EXACTAMENTE el mismo que usa su
// endpoint individual de "plantilla" (ver Descriptores abajo) — así una
// plantilla descargada módulo por módulo y una descargada combinada usan
// el mismo nombre de pestaña, y el archivo combinado se puede reconocer
// pestaña por pestaña sin ambigüedad.
public class ImportacionMasivaService : IImportacionMasivaService
{
    // Un módulo participante: su clave (Models/Modulos.cs, fuente única de
    // verdad para el nombre visible), el nombre de hoja (debe coincidir
    // con el que usa GenerarPlantillaExcel() de ese módulo) y los 2
    // puntos de entrada que ya existían en su propio servicio, sin
    // duplicar nada de su lógica.
    private sealed record DescriptorModulo(
        string Clave,
        string NombreHoja,
        Func<IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla>> ObtenerColumnas,
        Func<Stream, IReadOnlySet<int>?, Task<IResultadoImportacion>> ImportarAsync);

    private readonly IUsuarioContexto _usuarioContexto;
    private readonly IReadOnlyList<DescriptorModulo> _descriptores;

    public ImportacionMasivaService(
        IUsuarioContexto usuarioContexto,
        ITicketService ticketService,
        IIncidenteCriticoService incidenteCriticoService,
        IRespaldoService respaldoService,
        IPlaticaService platicaService,
        IAuditoriaEquipoService auditoriaEquipoService,
        IDisponibilidadServidorService disponibilidadServidorService,
        IDisponibilidadRedService disponibilidadRedService,
        IAlmacenamientoServidorService almacenamientoServidorService,
        IActualizacionEquipoCriticoService actualizacionEquipoCriticoService,
        IRecorridoService recorridoService,
        ICredencializacionService credencializacionService,
        ITestConsignaService testConsignaService,
        IAlcoholimetriaService alcoholimetriaService,
        IDopingService dopingService,
        ILockerService lockerService,
        IValeSalidaService valeSalidaService,
        IEstacionamientoService estacionamientoService,
        IReunionProveedorService reunionProveedorService,
        IEvaluacionVigilanciaService evaluacionVigilanciaService,
        IMantenimientoVehicularService mantenimientoVehicularService,
        ITiempoRespuestaResolucionService tiempoRespuestaResolucionService,
        IDisponibilidadAbastecimientoService disponibilidadAbastecimientoService,
        ICumplimientoDocumentacionService cumplimientoDocumentacionService,
        ICumplimientoProgramaService cumplimientoProgramaService,
        IObservacionSeguridadService observacionSeguridadService,
        ISafetyWalkService safetyWalkService,
        ICumplimientoEppService cumplimientoEppService,
        IEstatusLegalPlantaService estatusLegalPlantaService,
        IBrigadaService brigadaService,
        IEvaluacionProveedorSegHigieneService evaluacionProveedorSegHigieneService,
        IAccidenteTrabajoService accidenteTrabajoService)
    {
        _usuarioContexto = usuarioContexto;

        _descriptores = new List<DescriptorModulo>
        {
            new(Modulos.ItTickets, "IT Tickets", ticketService.ObtenerColumnasPlantilla,
                async (s, p) => await ticketService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItIncidentes, "IT Incidentes", incidenteCriticoService.ObtenerColumnasPlantilla,
                async (s, p) => await incidenteCriticoService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItRespaldos, "IT Respaldos", respaldoService.ObtenerColumnasPlantilla,
                async (s, p) => await respaldoService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItPlaticas, "IT Pláticas", platicaService.ObtenerColumnasPlantilla,
                async (s, p) => await platicaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItAuditorias, "IT Auditorias", auditoriaEquipoService.ObtenerColumnasPlantilla,
                async (s, p) => await auditoriaEquipoService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItDispServidores, "IT Disponibilidad Servidores", disponibilidadServidorService.ObtenerColumnasPlantilla,
                async (s, p) => await disponibilidadServidorService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItDispRed, "IT Disponibilidad Red", disponibilidadRedService.ObtenerColumnasPlantilla,
                async (s, p) => await disponibilidadRedService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItAlmacenamiento, "IT Almacenamiento", almacenamientoServidorService.ObtenerColumnasPlantilla,
                async (s, p) => await almacenamientoServidorService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.ItActualizacionesEquipos, "IT Actualizaciones Equipos", actualizacionEquipoCriticoService.ObtenerColumnasPlantilla,
                async (s, p) => await actualizacionEquipoCriticoService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegRecorridos, "Seg Recorridos", recorridoService.ObtenerColumnasPlantilla,
                async (s, p) => await recorridoService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegCredencializacion, "Seg Credencialización", credencializacionService.ObtenerColumnasPlantilla,
                async (s, p) => await credencializacionService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegTestConsignas, "Seg Test Consignas", testConsignaService.ObtenerColumnasPlantilla,
                async (s, p) => await testConsignaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegAlcoholimetria, "Seg Alcoholimetría", alcoholimetriaService.ObtenerColumnasPlantilla,
                async (s, p) => await alcoholimetriaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegDopings, "Seg Dopings", dopingService.ObtenerColumnasPlantilla,
                async (s, p) => await dopingService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegLockers, "Seg Lockers", lockerService.ObtenerColumnasPlantilla,
                async (s, p) => await lockerService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegValesSalida, "Seg Vales de Salida", valeSalidaService.ObtenerColumnasPlantilla,
                async (s, p) => await valeSalidaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegEstacionamiento, "Seg Estacionamiento", estacionamientoService.ObtenerColumnasPlantilla,
                async (s, p) => await estacionamientoService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegReunionesProveedor, "Seg Reuniones Proveedor", reunionProveedorService.ObtenerColumnasPlantilla,
                async (s, p) => await reunionProveedorService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegEvaluacionesVigilancia, "Seg Eval Vigilancia", evaluacionVigilanciaService.ObtenerColumnasPlantilla,
                async (s, p) => await evaluacionVigilanciaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.AdmMantenimientoVehicular, "Adm Mantenimiento Vehicular", mantenimientoVehicularService.ObtenerColumnasPlantilla,
                async (s, p) => await mantenimientoVehicularService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.AdmTiempoRespuestaResolucion, "Adm Tiempo Respuesta Resolucion", tiempoRespuestaResolucionService.ObtenerColumnasPlantilla,
                async (s, p) => await tiempoRespuestaResolucionService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.AdmDisponibilidadAbastecimiento, "Adm Disponibilidad Abastecimiento", disponibilidadAbastecimientoService.ObtenerColumnasPlantilla,
                async (s, p) => await disponibilidadAbastecimientoService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.AdmCumplimientoDocumentacion, "Adm Cumplimiento Documentacion", cumplimientoDocumentacionService.ObtenerColumnasPlantilla,
                async (s, p) => await cumplimientoDocumentacionService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.AdmCumplimientoPrograma, "Adm Cumplimiento Programa", cumplimientoProgramaService.ObtenerColumnasPlantilla,
                async (s, p) => await cumplimientoProgramaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegHigObservaciones, "Observaciones SegHig", observacionSeguridadService.ObtenerColumnasPlantilla,
                async (s, p) => await observacionSeguridadService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegHigSafetyWalks, "SegHig Safety Walks", safetyWalkService.ObtenerColumnasPlantilla,
                async (s, p) => await safetyWalkService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegHigCumplimientoEpp, "SegHig Cumplimiento EPP", cumplimientoEppService.ObtenerColumnasPlantilla,
                async (s, p) => await cumplimientoEppService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegHigEstatusLegal, "SegHig Estatus Legal", estatusLegalPlantaService.ObtenerColumnasPlantilla,
                async (s, p) => await estatusLegalPlantaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegHigBrigadas, "SegHig Brigadas", brigadaService.ObtenerColumnasPlantilla,
                async (s, p) => await brigadaService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegHigEvaluacionesProveedores, "SegHig Eval Proveedores", evaluacionProveedorSegHigieneService.ObtenerColumnasPlantilla,
                async (s, p) => await evaluacionProveedorSegHigieneService.ImportarDesdeExcelAsync(s, p)),
            new(Modulos.SegHigAccidentes, "Seg Hig Accidentes", accidenteTrabajoService.ObtenerColumnasPlantilla,
                async (s, p) => await accidenteTrabajoService.ImportarDesdeExcelAsync(s, p)),
        };
    }

    // Módulos asignados al usuario actual, en el mismo orden que
    // Models/Modulos.cs (para que la lista siempre salga en un orden
    // estable y predecible).
    private IEnumerable<DescriptorModulo> DescriptoresAsignados()
    {
        var asignados = _usuarioContexto.Modulos.ToHashSet();
        return _descriptores.Where(d => asignados.Contains(d.Clave));
    }

    public IReadOnlyList<ModuloDisponibleDto> ObtenerModulosDisponibles()
    {
        return DescriptoresAsignados()
            .Select(d => new ModuloDisponibleDto
            {
                Clave = d.Clave,
                Nombre = Modulos.Todos.First(m => m.Clave == d.Clave).Nombre,
                NombreHoja = ExcelPlantillaUtils.SanearNombreHoja(d.NombreHoja),
            })
            .ToList();
    }

    public byte[] GenerarPlantillaCombinada()
    {
        var asignados = DescriptoresAsignados().ToList();

        using var libro = new XLWorkbook();

        if (asignados.Count == 0)
        {
            // Nunca se regresa un archivo vacío/corrupto: si el usuario no
            // tiene ningún módulo de captura asignado, el archivo trae una
            // sola hoja que lo explica.
            var hojaVacia = libro.Worksheets.Add("Sin módulos asignados");
            hojaVacia.Cell(1, 1).Value = "Tu usuario no tiene asignado ningún indicador con importación por Excel.";
            hojaVacia.Cell(2, 1).Value = "Pide a un Superadmin que te asigne los módulos correspondientes desde Administración de Usuarios.";
            hojaVacia.Columns(1, 1).AdjustToContents();
            return ExcelPlantillaUtils.GuardarComoBytes(libro);
        }

        foreach (var descriptor in asignados)
        {
            ExcelPlantillaUtils.AgregarHoja(libro, descriptor.NombreHoja, descriptor.ObtenerColumnas());
        }

        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<ImportacionMasivaResultadoDto> ImportarCombinadoAsync(Stream archivoExcel)
    {
        var resultado = new ImportacionMasivaResultadoDto();
        var asignados = DescriptoresAsignados().ToList();

        // Índice por nombre de hoja SANEADO (el mismo saneo que se le
        // aplicó al nombre al generar la plantilla) — así reconoce la
        // pestaña aunque Excel le haya recortado o limpiado el nombre al
        // guardarla.
        var descriptoresPorHoja = asignados
            .ToDictionary(d => ExcelPlantillaUtils.SanearNombreHoja(d.NombreHoja), d => d, StringComparer.OrdinalIgnoreCase);

        using var libro = new XLWorkbook(archivoExcel);

        foreach (var hoja in libro.Worksheets)
        {
            if (!descriptoresPorHoja.TryGetValue(hoja.Name, out var descriptor))
            {
                // O el nombre de la pestaña no corresponde a ningún módulo
                // conocido, o corresponde a un módulo que este usuario no
                // tiene asignado (en ese caso tampoco se procesa, para no
                // dejar que alguien importe datos de un módulo que no le
                // pertenece con solo renombrar/agregar una pestaña).
                if (hoja.RowsUsed().Skip(1).Any())
                {
                    resultado.PestanasOmitidas.Add(hoja.Name);
                }

                continue;
            }

            var filasUsadas = hoja.RowsUsed().ToList();
            if (filasUsadas.Count <= 1)
            {
                // Solo trae el encabezado (o ni eso) — pestaña que el
                // usuario nunca llenó, se ignora en silencio: no se
                // reporta como error ni como omitida, porque el archivo
                // combinado trae una pestaña por CADA módulo asignado
                // aunque el usuario solo quiera llenar unas cuantas.
                continue;
            }

            // ¿La fila 2 sigue siendo exactamente la fila de ejemplo con
            // la que se generó la plantilla? Si es así, se excluye antes
            // de importar — no se crea un registro real a partir de ella.
            // Si el usuario la modificó o la borró (su primer dato real
            // quedó en la fila 2), se importa tal cual.
            var segundaFila = filasUsadas.Count >= 2 ? filasUsadas[1] : null;
            var filaDeEjemploIntacta = segundaFila is not null
                && segundaFila.RowNumber() == 2
                && ExcelPlantillaUtils.EsFilaDeEjemplo(segundaFila, descriptor.ObtenerColumnas());

            if (filaDeEjemploIntacta && filasUsadas.Count == 2)
            {
                // Solo encabezado + fila de ejemplo intacta, nada más —
                // tampoco hay nada que importar.
                continue;
            }

            var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, descriptor.Clave);

            var bytesHoja = filaDeEjemploIntacta
                ? ExcelPlantillaUtils.ExtraerHojaComoXlsxSinFila(hoja, 2)
                : ExcelPlantillaUtils.ExtraerHojaComoXlsx(hoja);
            using var flujoHoja = new MemoryStream(bytesHoja);
            var resultadoModulo = await descriptor.ImportarAsync(flujoHoja, plantasPermitidas);

            resultado.Modulos.Add(new ResumenModuloImportadoDto
            {
                Clave = descriptor.Clave,
                Nombre = Modulos.Todos.First(m => m.Clave == descriptor.Clave).Nombre,
                NombreHoja = hoja.Name,
                TotalFilas = resultadoModulo.TotalFilas,
                Creados = resultadoModulo.Creados,
                Actualizados = resultadoModulo.Actualizados,
                Omitidos = resultadoModulo.Omitidos,
                Errores = resultadoModulo.Errores,
            });
        }

        return resultado;
    }
}
