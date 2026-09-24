namespace WasionOne.API.Models;

// Catálogo fijo de módulos de captura del sistema (mismo patrón que
// Roles.cs). Se agrega una entrada aquí cada vez que se construye una
// pantalla nueva. El administrador usa estas claves para dar acceso
// módulo por módulo a cada usuario (selección manual, además de su nodo
// en la jerarquía Dirección/Departamento/Área).
public static class Modulos
{
    public const string ItTickets = "it.tickets";
    public const string ItInventario = "it.inventario";
    public const string ItIncidentes = "it.incidentes";
    public const string ItRespaldos = "it.respaldos";
    public const string ItPlaticas = "it.platicas";
    public const string ItAuditorias = "it.auditorias";
    public const string ItDispServidores = "it.dispServidores";
    public const string ItDispRed = "it.dispRed";
    public const string ItAlmacenamiento = "it.almacenamiento";
    public const string ItActualizacionesEquipos = "it.actualizacionesEquipos";
    public const string SegRecorridos = "seguridad.recorridos";
    public const string SegCredencializacion = "seguridad.credencializacion";
    public const string SegTestConsignas = "seguridad.testConsignas";
    public const string SegAlcoholimetria = "seguridad.alcoholimetria";
    public const string SegDopings = "seguridad.dopings";
    public const string SegLockers = "seguridad.lockers";
    public const string SegValesSalida = "seguridad.valesSalida";
    public const string SegEstacionamiento = "seguridad.estacionamiento";
    public const string SegReunionesProveedor = "seguridad.reunionesProveedor";
    public const string SegEvaluacionesVigilancia = "seguridad.evaluacionesVigilancia";
    public const string AdmMantenimientoVehicular = "administracion.mantenimientoVehicular";
    public const string AdmTiempoRespuestaResolucion = "administracion.tiempoRespuestaResolucion";
    public const string AdmDisponibilidadAbastecimiento = "administracion.disponibilidadAbastecimiento";
    public const string AdmCumplimientoDocumentacion = "administracion.cumplimientoDocumentacion";
    public const string AdmCumplimientoPrograma = "administracion.cumplimientoPrograma";

    // --- Seguridad e Higiene (Fase 3.6, 22/sep/2026) ---
    public const string SegHigObservaciones = "seguridadHigiene.observaciones";
    public const string SegHigSafetyWalks = "seguridadHigiene.safetyWalks";
    public const string SegHigCumplimientoEpp = "seguridadHigiene.cumplimientoEpp";
    public const string SegHigEstatusLegal = "seguridadHigiene.estatusLegal";
    public const string SegHigBrigadas = "seguridadHigiene.brigadas";
    public const string SegHigEvaluacionesProveedores = "seguridadHigiene.evaluacionesProveedores";
    public const string SegHigAccidentes = "seguridadHigiene.accidentes";

    // AreaId: a qué Área pertenece cada módulo (IT = 3, Seguridad
    // Patrimonial = 1, Administración = 11) — agregado 20/sep/2026 para que
    // la pantalla de Usuarios sepa qué lista de Plantas mostrar al
    // configurar el nivel de captura por Planta de cada módulo (ver
    // UsuarioModuloUbicacion).
    public record Definicion(string Clave, string Nombre, int AreaId);

    private const int AREA_ID_IT = 3;
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    // AreaId real confirmado en la base de datos (22/sep/2026): Área
    // "Administración" dentro del Departamento "Administración", creada por
    // Actualizar-CatalogoAdministracionCentralizada.ps1 → Id = 11.
    private const int AREA_ID_ADMINISTRACION = 11;

    // AreaId real confirmado por el usuario en la base de datos (23/sep/2026):
    // Área "Seguridad e Higiene" dentro del Departamento Seguridad (junto a
    // Seguridad Patrimonial = 1 e IT = 3) → Id = 2. ÚNICO lugar donde vive
    // este valor — todos los Services y componentes de Seguridad e Higiene lo
    // toman de aquí (o de su espejo en el frontend,
    // shared/constants/modulos.ts) en vez de declarar su propia constante
    // local, precisamente para no repetir el problema detectado el
    // 22/sep/2026 con AREA_ID_ADMINISTRACION (mismo valor duplicado en 11
    // archivos distintos, corregido uno por uno).
    public const int AreaIdSeguridadHigiene = 2;

    public static readonly IReadOnlyList<Definicion> Todos = new List<Definicion>
    {
        new(ItTickets, "IT — Tickets de soporte", AREA_ID_IT),
        new(ItInventario, "IT — Inventario electrónico", AREA_ID_IT),
        new(ItIncidentes, "IT — Incidentes críticos", AREA_ID_IT),
        new(ItRespaldos, "IT — Respaldos", AREA_ID_IT),
        new(ItPlaticas, "IT — Envío de pláticas de ciberseguridad", AREA_ID_IT),
        new(ItAuditorias, "IT — Auditorías de equipos electrónicos", AREA_ID_IT),
        new(ItDispServidores, "IT — Disponibilidad de servidores", AREA_ID_IT),
        new(ItDispRed, "IT — Disponibilidad de red", AREA_ID_IT),
        new(ItAlmacenamiento, "IT — % de almacenamiento", AREA_ID_IT),
        new(ItActualizacionesEquipos, "IT — Actualizaciones de equipos críticos", AREA_ID_IT),
        new(SegRecorridos, "Seguridad Patrimonial — Recorridos", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegCredencializacion, "Seguridad Patrimonial — Credencialización", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegTestConsignas, "Seguridad Patrimonial — Test de Consignas", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegAlcoholimetria, "Seguridad Patrimonial — Alcoholimetría", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegDopings, "Seguridad Patrimonial — Dopings", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegLockers, "Seguridad Patrimonial — Lockers", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegValesSalida, "Seguridad Patrimonial — Vales de salida", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegEstacionamiento, "Seguridad Patrimonial — Estacionamiento", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegReunionesProveedor, "Seguridad Patrimonial — Reuniones con proveedor", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(SegEvaluacionesVigilancia, "Seguridad Patrimonial — Evaluaciones de vigilancia", AREA_ID_SEGURIDAD_PATRIMONIAL),
        new(AdmMantenimientoVehicular, "Administración — Mantenimiento Vehicular", AREA_ID_ADMINISTRACION),
        new(AdmTiempoRespuestaResolucion, "Administración — Tiempo de Respuesta y Resolución", AREA_ID_ADMINISTRACION),
        new(AdmDisponibilidadAbastecimiento, "Administración — Disponibilidad y Abastecimiento", AREA_ID_ADMINISTRACION),
        new(AdmCumplimientoDocumentacion, "Administración — Cumplimiento de la Documentación", AREA_ID_ADMINISTRACION),
        new(AdmCumplimientoPrograma, "Administración — Cumplimiento con el Programa", AREA_ID_ADMINISTRACION),
        new(SegHigObservaciones, "Seguridad e Higiene — Observaciones de seguridad", AreaIdSeguridadHigiene),
        new(SegHigSafetyWalks, "Seguridad e Higiene — Safety Walks", AreaIdSeguridadHigiene),
        new(SegHigCumplimientoEpp, "Seguridad e Higiene — Cumplimiento de EPP", AreaIdSeguridadHigiene),
        new(SegHigEstatusLegal, "Seguridad e Higiene — Estatus legal de planta", AreaIdSeguridadHigiene),
        new(SegHigBrigadas, "Seguridad e Higiene — Brigadas", AreaIdSeguridadHigiene),
        new(SegHigEvaluacionesProveedores, "Seguridad e Higiene — Evaluaciones a Proveedores", AreaIdSeguridadHigiene),
        new(SegHigAccidentes, "Seguridad e Higiene — Accidentes de trabajo", AreaIdSeguridadHigiene),
    };

    public static bool EsValido(string clave) => Todos.Any(m => m.Clave == clave);
}
