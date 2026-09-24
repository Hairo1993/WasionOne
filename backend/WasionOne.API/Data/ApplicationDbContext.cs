using Microsoft.EntityFrameworkCore;
using WasionOne.API.Models;

namespace WasionOne.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Direccion> Direcciones => Set<Direccion>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<AreaUbicacion> AreaUbicaciones => Set<AreaUbicacion>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<InventarioEquipo> InventarioEquipos => Set<InventarioEquipo>();
    public DbSet<IncidenteCritico> IncidentesCriticos => Set<IncidenteCritico>();
    public DbSet<Respaldo> Respaldos => Set<Respaldo>();
    public DbSet<Platica> Platicas => Set<Platica>();
    public DbSet<AuditoriaEquipo> AuditoriasEquipo => Set<AuditoriaEquipo>();
    public DbSet<DisponibilidadServidor> DisponibilidadServidores => Set<DisponibilidadServidor>();
    public DbSet<DisponibilidadRed> DisponibilidadRedes => Set<DisponibilidadRed>();
    public DbSet<AlmacenamientoServidor> AlmacenamientoServidores => Set<AlmacenamientoServidor>();
    public DbSet<UsuarioModulo> UsuarioModulos => Set<UsuarioModulo>();
    public DbSet<UsuarioModuloUbicacion> UsuarioModuloUbicaciones => Set<UsuarioModuloUbicacion>();
    public DbSet<Recorrido> Recorridos => Set<Recorrido>();
    public DbSet<Credencializacion> Credencializaciones => Set<Credencializacion>();
    public DbSet<TestConsigna> TestConsignas => Set<TestConsigna>();
    public DbSet<Alcoholimetria> Alcoholimetrias => Set<Alcoholimetria>();
    public DbSet<Doping> Dopings => Set<Doping>();
    public DbSet<Locker> Lockers => Set<Locker>();
    public DbSet<ValeSalida> ValesSalida => Set<ValeSalida>();
    public DbSet<Estacionamiento> Estacionamientos => Set<Estacionamiento>();
    public DbSet<ReunionProveedor> ReunionesProveedor => Set<ReunionProveedor>();
    public DbSet<EvaluacionVigilancia> EvaluacionesVigilancia => Set<EvaluacionVigilancia>();
    public DbSet<ActualizacionEquipoCritico> ActualizacionesEquiposCriticos => Set<ActualizacionEquipoCritico>();
    public DbSet<MantenimientoVehicular> MantenimientosVehiculares => Set<MantenimientoVehicular>();
    public DbSet<TiempoRespuestaResolucion> TiemposRespuestaResolucion => Set<TiempoRespuestaResolucion>();
    public DbSet<DisponibilidadAbastecimiento> DisponibilidadesAbastecimiento => Set<DisponibilidadAbastecimiento>();
    public DbSet<CumplimientoDocumentacion> CumplimientosDocumentacion => Set<CumplimientoDocumentacion>();
    public DbSet<CumplimientoPrograma> CumplimientosPrograma => Set<CumplimientoPrograma>();
    public DbSet<ObservacionSeguridad> ObservacionesSeguridad => Set<ObservacionSeguridad>();
    public DbSet<SafetyWalk> SafetyWalks => Set<SafetyWalk>();
    public DbSet<AccidenteTrabajo> AccidentesTrabajo => Set<AccidenteTrabajo>();
    public DbSet<CumplimientoEpp> CumplimientosEpp => Set<CumplimientoEpp>();
    public DbSet<EstatusLegalPlanta> EstatusLegalesPlanta => Set<EstatusLegalPlanta>();
    public DbSet<EvaluacionProveedorSegHigiene> EvaluacionesProveedorSegHigiene => Set<EvaluacionProveedorSegHigiene>();
    public DbSet<Brigada> Brigadas => Set<Brigada>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Direccion>(entidad =>
        {
            entidad.ToTable("Direccion");
            entidad.Property(d => d.Nombre).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<Departamento>(entidad =>
        {
            entidad.ToTable("Departamento");
            entidad.Property(d => d.Nombre).IsRequired().HasMaxLength(150);
            entidad
                .HasOne(d => d.Direccion)
                .WithMany(dir => dir.Departamentos)
                .HasForeignKey(d => d.DireccionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Area>(entidad =>
        {
            entidad.ToTable("Area");
            entidad.Property(a => a.Nombre).IsRequired().HasMaxLength(150);
            entidad
                .HasOne(a => a.Departamento)
                .WithMany(d => d.Areas)
                .HasForeignKey(a => a.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ubicacion>(entidad =>
        {
            entidad.ToTable("Ubicacion");
            entidad.Property(u => u.Nombre).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<AreaUbicacion>(entidad =>
        {
            entidad.ToTable("AreaUbicacion");
            entidad
                .HasOne(au => au.Area)
                .WithMany(a => a.AreaUbicaciones)
                .HasForeignKey(au => au.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad
                .HasOne(au => au.Ubicacion)
                .WithMany(u => u.AreaUbicaciones)
                .HasForeignKey(au => au.UbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad.HasIndex(au => new { au.AreaId, au.UbicacionId }).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.ToTable("Usuario");
            entidad.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(100);
            entidad.HasIndex(u => u.NombreUsuario).IsUnique();
            entidad.Property(u => u.PasswordHash).IsRequired().HasMaxLength(200);
            entidad.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(150);
            entidad.Property(u => u.Rol).IsRequired().HasMaxLength(50);

            entidad
                .HasOne(u => u.Direccion)
                .WithMany()
                .HasForeignKey(u => u.DireccionId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad
                .HasOne(u => u.Departamento)
                .WithMany()
                .HasForeignKey(u => u.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad
                .HasOne(u => u.Area)
                .WithMany()
                .HasForeignKey(u => u.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ticket>(entidad =>
        {
            entidad.ToTable("Ticket");

            entidad.Property(t => t.IdTicketOrigen).HasMaxLength(50);
            // Único solo entre los que sí traen valor (tickets importados);
            // los capturados a mano dejan este campo en null y no chocan
            // entre sí porque SQL Server no aplica unicidad entre NULLs.
            entidad.HasIndex(t => t.IdTicketOrigen).IsUnique();

            entidad.Property(t => t.Asunto).IsRequired().HasMaxLength(300);
            entidad.Property(t => t.Descripcion).IsRequired().HasMaxLength(4000);
            entidad.Property(t => t.Categoria).HasMaxLength(150);
            entidad.Property(t => t.Subcategoria).HasMaxLength(150);
            entidad.Property(t => t.Tipo).HasMaxLength(50);
            entidad.Property(t => t.TipoAsociacion).HasMaxLength(100);
            entidad.Property(t => t.Etiquetas).HasMaxLength(300);

            entidad.Property(t => t.Prioridad).IsRequired().HasMaxLength(20);
            entidad.Property(t => t.Urgencia).HasMaxLength(20);
            entidad.Property(t => t.Impacto).HasMaxLength(20);

            entidad.Property(t => t.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(t => t.EstadoAprobacion).HasMaxLength(30);
            entidad.Property(t => t.EstadoResolucion).HasMaxLength(30);
            entidad.Property(t => t.EstadoPrimeraRespuesta).HasMaxLength(30);

            entidad.Property(t => t.Grupo).HasMaxLength(150);
            entidad.Property(t => t.Agente).HasMaxLength(150);
            entidad.Property(t => t.Origen).HasMaxLength(50);

            entidad.Property(t => t.NombreSolicitante).IsRequired().HasMaxLength(150);
            entidad.Property(t => t.CorreoSolicitante).HasMaxLength(200);
            entidad.Property(t => t.UbicacionSolicitante).HasMaxLength(150);

            entidad.Property(t => t.Elemento).HasMaxLength(200);
            entidad.Property(t => t.AnyDeskEquipo).HasMaxLength(100);
            entidad.Property(t => t.DepartamentoOrigen).HasMaxLength(150);
            entidad.Property(t => t.TiempoInicialRespuesta).HasMaxLength(100);

            entidad.Property(t => t.NotaResolucion).HasMaxLength(4000);
            entidad.Property(t => t.ResultadoEncuesta).HasMaxLength(200);

            entidad
                .HasOne(t => t.AreaUbicacion)
                .WithMany()
                .HasForeignKey(t => t.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventarioEquipo>(entidad =>
        {
            entidad.ToTable("InventarioEquipo");

            entidad.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entidad.HasIndex(e => e.Codigo).IsUnique();

            entidad.Property(e => e.Almacen).HasMaxLength(100);
            entidad.Property(e => e.UbicacionExacta).HasMaxLength(150);

            entidad.Property(e => e.TipoEquipo).IsRequired().HasMaxLength(50);
            entidad.Property(e => e.Hostname).HasMaxLength(100);
            entidad.Property(e => e.Marca).IsRequired().HasMaxLength(100);
            entidad.Property(e => e.Modelo).IsRequired().HasMaxLength(100);

            entidad.Property(e => e.Serial).IsRequired().HasMaxLength(100);
            entidad.HasIndex(e => e.Serial).IsUnique();

            entidad.Property(e => e.Ram).HasMaxLength(50);
            entidad.Property(e => e.Procesador).HasMaxLength(150);
            entidad.Property(e => e.Almacenamiento).HasMaxLength(50);
            entidad.Property(e => e.SistemaOperativo).HasMaxLength(100);
            entidad.Property(e => e.MacWireless).HasMaxLength(50);
            entidad.Property(e => e.MacEthernet).HasMaxLength(50);

            entidad.Property(e => e.UsuarioAsignado).HasMaxLength(150);
            entidad.Property(e => e.Estado).IsRequired().HasMaxLength(20);
            entidad.Property(e => e.Observaciones).HasMaxLength(2000);

            entidad
                .HasOne(e => e.AreaUbicacion)
                .WithMany()
                .HasForeignKey(e => e.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IncidenteCritico>(entidad =>
        {
            entidad.ToTable("IncidenteCritico");

            entidad.Property(i => i.IdFallaOrigen).HasMaxLength(50);
            // Único solo entre los que sí traen valor (importados); los
            // capturados a mano dejan este campo en null.
            entidad.HasIndex(i => i.IdFallaOrigen).IsUnique();

            entidad.Property(i => i.Area).HasMaxLength(150);
            entidad.Property(i => i.Linea).HasMaxLength(150);
            entidad.Property(i => i.Severidad).HasMaxLength(30);
            entidad.Property(i => i.Tipo).HasMaxLength(100);
            entidad.Property(i => i.Descripcion).IsRequired().HasMaxLength(2000);
            entidad.Property(i => i.Responsable).HasMaxLength(150);
            entidad.Property(i => i.Causa).HasMaxLength(2000);
            entidad.Property(i => i.Detalles).HasMaxLength(2000);
            entidad.Property(i => i.Contramedida).HasMaxLength(2000);
            entidad.Property(i => i.Estado).IsRequired().HasMaxLength(30);

            entidad
                .HasOne(i => i.AreaUbicacion)
                .WithMany()
                .HasForeignKey(i => i.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // null = el incidente impacta a todos los departamentos.
            entidad
                .HasOne(i => i.Departamento)
                .WithMany()
                .HasForeignKey(i => i.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Respaldo>(entidad =>
        {
            entidad.ToTable("Respaldo");

            entidad.Property(r => r.IdOrigen).HasMaxLength(50);
            entidad.HasIndex(r => r.IdOrigen).IsUnique();

            entidad.Property(r => r.SistemaAplicacion).IsRequired().HasMaxLength(200);
            entidad.Property(r => r.SoftwareUtilizado).HasMaxLength(200);
            entidad.Property(r => r.TipoRespaldo).HasMaxLength(100);
            entidad.Property(r => r.Responsable).HasMaxLength(150);
            entidad.Property(r => r.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(r => r.Observaciones).HasMaxLength(2000);

            entidad
                .HasOne(r => r.AreaUbicacion)
                .WithMany()
                .HasForeignKey(r => r.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Platica>(entidad =>
        {
            entidad.ToTable("Platica");

            entidad.Property(p => p.IdOrigen).HasMaxLength(50);
            entidad.HasIndex(p => p.IdOrigen).IsUnique();

            entidad.Property(p => p.TemaPolitica).IsRequired().HasMaxLength(300);
            entidad.Property(p => p.ResponsableEnvio).HasMaxLength(150);
            entidad.Property(p => p.MedioDifusion).HasMaxLength(150);

            // null = envío a toda la empresa (sin ubicación específica).
            entidad
                .HasOne(p => p.AreaUbicacion)
                .WithMany()
                .HasForeignKey(p => p.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuditoriaEquipo>(entidad =>
        {
            entidad.ToTable("AuditoriaEquipo");

            entidad.Property(a => a.Folio).HasMaxLength(50);
            entidad.HasIndex(a => a.Folio).IsUnique();

            entidad.Property(a => a.Area).HasMaxLength(150);
            entidad.Property(a => a.Almacen).HasMaxLength(100);
            entidad.Property(a => a.CodigoActivo).HasMaxLength(100);
            entidad.Property(a => a.DescripcionActivo).HasMaxLength(300);
            entidad.Property(a => a.Responsable).HasMaxLength(150);
            entidad.Property(a => a.Tipo).HasMaxLength(100);
            entidad.Property(a => a.Estado).IsRequired().HasMaxLength(30);

            entidad
                .HasOne(a => a.AreaUbicacion)
                .WithMany()
                .HasForeignKey(a => a.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DisponibilidadServidor>(entidad =>
        {
            entidad.ToTable("DisponibilidadServidor");

            entidad.Property(r => r.IdOrigen).HasMaxLength(50);
            // Único solo entre los que sí traen valor (importados); los
            // capturados a mano dejan este campo en null y pueden repetirse
            // libremente — es un registro por evento/revisión, no un
            // acumulado único por día.
            entidad.HasIndex(r => r.IdOrigen).IsUnique();

            entidad.Property(r => r.Servidor).IsRequired().HasMaxLength(150);
            entidad.Property(r => r.Ip).HasMaxLength(50);
            entidad.Property(r => r.Servicio).HasMaxLength(150);
            entidad.Property(r => r.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(r => r.DisponibilidadPorcentaje).HasColumnType("decimal(5,2)");
            entidad.Property(r => r.Responsable).HasMaxLength(150);
            entidad.Property(r => r.Observaciones).HasMaxLength(2000);

            entidad
                .HasOne(r => r.AreaUbicacion)
                .WithMany()
                .HasForeignKey(r => r.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DisponibilidadRed>(entidad =>
        {
            entidad.ToTable("DisponibilidadRed");

            entidad.Property(r => r.IdOrigen).HasMaxLength(50);
            entidad.HasIndex(r => r.IdOrigen).IsUnique();

            entidad.Property(r => r.Dispositivo).IsRequired().HasMaxLength(150);
            entidad.Property(r => r.Ip).HasMaxLength(50);
            entidad.Property(r => r.TipoDispositivo).HasMaxLength(100);
            entidad.Property(r => r.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(r => r.LatenciaMs).HasColumnType("decimal(9,2)");
            entidad.Property(r => r.PerdidaPaquetesPorcentaje).HasColumnType("decimal(5,2)");
            entidad.Property(r => r.DisponibilidadPorcentaje).HasColumnType("decimal(5,2)");
            entidad.Property(r => r.Responsable).HasMaxLength(150);
            entidad.Property(r => r.Observaciones).HasMaxLength(2000);

            entidad
                .HasOne(r => r.AreaUbicacion)
                .WithMany()
                .HasForeignKey(r => r.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AlmacenamientoServidor>(entidad =>
        {
            entidad.ToTable("AlmacenamientoServidor");

            entidad.Property(r => r.IdOrigen).HasMaxLength(50);
            entidad.HasIndex(r => r.IdOrigen).IsUnique();

            entidad.Property(r => r.Servidor).IsRequired().HasMaxLength(150);
            entidad.Property(r => r.Ip).HasMaxLength(50);
            entidad.Property(r => r.Unidad).HasMaxLength(50);
            entidad.Property(r => r.CapacidadTotalGb).HasColumnType("decimal(10,2)");
            entidad.Property(r => r.EspacioUtilizadoGb).HasColumnType("decimal(10,2)");
            entidad.Property(r => r.EspacioDisponibleGb).HasColumnType("decimal(10,2)");
            entidad.Property(r => r.AlmacenamientoUtilizadoPorcentaje).HasColumnType("decimal(5,2)");
            entidad.Property(r => r.Umbral).HasColumnType("decimal(5,2)");
            entidad.Property(r => r.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(r => r.Responsable).HasMaxLength(150);
            entidad.Property(r => r.Observaciones).HasMaxLength(2000);

            entidad
                .HasOne(r => r.AreaUbicacion)
                .WithMany()
                .HasForeignKey(r => r.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Recorrido>(entidad =>
        {
            entidad.ToTable("Recorrido");

            entidad.Property(r => r.IdOrigen).HasMaxLength(50);
            entidad.HasIndex(r => r.IdOrigen).IsUnique();

            entidad.Property(r => r.Operador).HasMaxLength(150);
            entidad.Property(r => r.AreaTipo).HasMaxLength(150);
            entidad.Property(r => r.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(r => r.Hallazgos).HasMaxLength(2000);

            entidad
                .HasOne(r => r.AreaUbicacion)
                .WithMany()
                .HasForeignKey(r => r.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Credencializacion>(entidad =>
        {
            entidad.ToTable("Credencializacion");

            entidad.Property(c => c.IdOrigen).HasMaxLength(50);
            entidad.HasIndex(c => c.IdOrigen).IsUnique();

            entidad.Property(c => c.NombreCompleto).IsRequired().HasMaxLength(200);
            entidad.Property(c => c.Empresa).HasMaxLength(200);
            entidad.Property(c => c.TipoAcceso).HasMaxLength(50);
            entidad.Property(c => c.Identificacion).HasMaxLength(100);
            entidad.Property(c => c.PersonaQueVisita).HasMaxLength(200);
            entidad.Property(c => c.MotivoVisita).HasMaxLength(300);
            entidad.Property(c => c.AreaDeTrabajo).HasMaxLength(150);
            entidad.Property(c => c.Estado).IsRequired().HasMaxLength(30);

            entidad
                .HasOne(c => c.AreaUbicacion)
                .WithMany()
                .HasForeignKey(c => c.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TestConsigna>(entidad =>
        {
            entidad.ToTable("TestConsigna");

            entidad.Property(t => t.IdOrigen).HasMaxLength(50);
            entidad.HasIndex(t => t.IdOrigen).IsUnique();

            entidad.Property(t => t.ResultadoTest).IsRequired().HasMaxLength(30);
            entidad.Property(t => t.AreaInvolucrada).HasMaxLength(150);
            entidad.Property(t => t.ProcedimientoInvolucrado).HasMaxLength(200);
            entidad.Property(t => t.Proveedor).HasMaxLength(150);

            entidad
                .HasOne(t => t.AreaUbicacion)
                .WithMany()
                .HasForeignKey(t => t.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Alcoholimetria>(entidad =>
        {
            entidad.ToTable("Alcoholimetria");

            entidad.Property(a => a.Turno).IsRequired().HasMaxLength(30);

            // No hay "ID" de origen para este registro agregado: la
            // combinación Planta + Fecha + Turno es la llave natural (un
            // registro por turno/planta/día).
            entidad.HasIndex(a => new { a.AreaUbicacionId, a.Fecha, a.Turno }).IsUnique();

            entidad
                .HasOne(a => a.AreaUbicacion)
                .WithMany()
                .HasForeignKey(a => a.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Doping>(entidad =>
        {
            entidad.ToTable("Doping");

            entidad.Property(d => d.IdOrigen).HasMaxLength(50);
            // Índice único filtrado: SQL Server, a diferencia de otros
            // motores, no permite más de un NULL en un índice único normal.
            // Como IdOrigen es opcional (solo viene en registros importados),
            // se filtra para permitir múltiples capturas manuales sin ID.
            entidad.HasIndex(d => d.IdOrigen).IsUnique().HasFilter("[IdOrigen] IS NOT NULL");

            entidad.Property(d => d.Turno).HasMaxLength(30);
            entidad.Property(d => d.NoNomina).HasMaxLength(50);
            entidad.Property(d => d.Nombre).HasMaxLength(200);
            entidad.Property(d => d.Area).HasMaxLength(150);
            entidad.Property(d => d.Resultado).IsRequired().HasMaxLength(30);

            entidad
                .HasOne(d => d.AreaUbicacion)
                .WithMany()
                .HasForeignKey(d => d.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Locker>(entidad =>
        {
            entidad.ToTable("Locker");

            entidad.Property(l => l.IdOrigen).HasMaxLength(50);
            // Mismo motivo que en Doping: índice único filtrado para permitir
            // múltiples capturas manuales sin IdOrigen.
            entidad.HasIndex(l => l.IdOrigen).IsUnique().HasFilter("[IdOrigen] IS NOT NULL");

            entidad.Property(l => l.NumeroLocker).HasMaxLength(50);
            entidad.Property(l => l.NoNomina).HasMaxLength(50);
            entidad.Property(l => l.Nombre).HasMaxLength(200);
            entidad.Property(l => l.Resultado).IsRequired().HasMaxLength(30);
            entidad.Property(l => l.Detalles).HasMaxLength(1000);

            entidad
                .HasOne(l => l.AreaUbicacion)
                .WithMany()
                .HasForeignKey(l => l.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ValeSalida>(entidad =>
        {
            entidad.ToTable("ValeSalida");

            entidad.Property(v => v.IdOrigen).HasMaxLength(50);
            // Índice único filtrado: IdOrigen es opcional (solo viene en
            // registros importados), y SQL Server no permite más de un NULL
            // en un índice único normal.
            entidad.HasIndex(v => v.IdOrigen).IsUnique().HasFilter("[IdOrigen] IS NOT NULL");

            entidad.Property(v => v.Folio).HasMaxLength(50);
            entidad.Property(v => v.Estado).IsRequired().HasMaxLength(30);

            entidad
                .HasOne(v => v.AreaUbicacion)
                .WithMany()
                .HasForeignKey(v => v.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Estacionamiento>(entidad =>
        {
            entidad.ToTable("Estacionamiento");

            entidad.Property(e => e.IdOrigen).HasMaxLength(50);

            // Llave de negocio obligatoria y única: un registro por
            // colaborador (Marbete), se edita en vez de duplicarse. No se
            // filtra porque NoMarbete es requerido (nunca NULL).
            entidad.Property(e => e.NoMarbete).IsRequired().HasMaxLength(50);
            entidad.HasIndex(e => e.NoMarbete).IsUnique();

            entidad
                .HasOne(e => e.AreaUbicacion)
                .WithMany()
                .HasForeignKey(e => e.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReunionProveedor>(entidad =>
        {
            entidad.ToTable("ReunionProveedor");

            entidad.Property(r => r.IdOrigen).HasMaxLength(50);
            // Índice único filtrado: mismo motivo que ValeSalida.IdOrigen.
            entidad.HasIndex(r => r.IdOrigen).IsUnique().HasFilter("[IdOrigen] IS NOT NULL");

            entidad
                .HasOne(r => r.AreaUbicacion)
                .WithMany()
                .HasForeignKey(r => r.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EvaluacionVigilancia>(entidad =>
        {
            entidad.ToTable("EvaluacionVigilancia");

            entidad.Property(e => e.Proveedor).IsRequired().HasMaxLength(200);

            // No hay "ID" de origen en el Excel: la combinación Planta +
            // Proveedor + Fecha es la llave natural (una evaluación por
            // Planta/Proveedor/mes) — mismo criterio que Alcoholimetría.
            entidad.HasIndex(e => new { e.AreaUbicacionId, e.Proveedor, e.Fecha }).IsUnique();

            entidad.Property(e => e.Total).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Cobertura).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Expedientes).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Uniformidad).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Reuniones).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Equipamiento).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Supervision).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.CambiosSolicitados).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Procedimientos).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Capacitacion).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Acuerdos).HasColumnType("decimal(10,2)");

            entidad
                .HasOne(e => e.AreaUbicacion)
                .WithMany()
                .HasForeignKey(e => e.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ActualizacionEquipoCritico>(entidad =>
        {
            entidad.ToTable("ActualizacionEquipoCritico");

            entidad.Property(a => a.Tipo).HasMaxLength(150);
            entidad.Property(a => a.Codigo).IsRequired().HasMaxLength(100);
            entidad.Property(a => a.Estado).HasMaxLength(100);
            entidad.Property(a => a.Responsable).HasMaxLength(150);

            // El "Código" identifica al equipo, no al registro (un mismo
            // equipo tiene varias actualizaciones en fechas distintas), así
            // que la combinación Código + Fecha de registro es la llave
            // natural — mismo criterio que Alcoholimetría/Evaluaciones de
            // vigilancia.
            entidad.HasIndex(a => new { a.Codigo, a.FechaRegistro }).IsUnique();

            entidad
                .HasOne(a => a.AreaUbicacion)
                .WithMany()
                .HasForeignKey(a => a.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MantenimientoVehicular>(entidad =>
        {
            entidad.ToTable("MantenimientoVehicular");

            // Llave de negocio obligatoria y única: un registro por
            // vehículo (VIN), se edita en vez de duplicarse. No se filtra
            // porque Vin es requerido (nunca NULL).
            entidad.Property(m => m.Vin).IsRequired().HasMaxLength(50);
            entidad.HasIndex(m => m.Vin).IsUnique();

            entidad.Property(m => m.VehiculoTipo).HasMaxLength(150);
            entidad.Property(m => m.Estatus).HasMaxLength(100);

            entidad
                .HasOne(m => m.AreaUbicacion)
                .WithMany()
                .HasForeignKey(m => m.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TiempoRespuestaResolucion>(entidad =>
        {
            entidad.ToTable("TiempoRespuestaResolucion");

            // Llave de negocio obligatoria y única: siempre presente en el
            // sistema de origen (a diferencia del patrón IdOrigen opcional
            // de otros módulos).
            entidad.Property(t => t.Folio).IsRequired().HasMaxLength(50);
            entidad.HasIndex(t => t.Folio).IsUnique();

            entidad.Property(t => t.Solicitante).HasMaxLength(200);
            entidad.Property(t => t.ResponsableCompras).HasMaxLength(200);
            entidad.Property(t => t.Tipo).HasMaxLength(100);
            entidad.Property(t => t.Prioridad).HasMaxLength(50);
            entidad.Property(t => t.Estatus).HasMaxLength(50);
            entidad.Property(t => t.Solicitud).HasMaxLength(2000);
            entidad.Property(t => t.Observaciones).HasMaxLength(2000);
            entidad.Property(t => t.SlaCierreHoras).HasColumnType("decimal(10,2)");
            entidad.Property(t => t.TiempoAtencionHoras).HasColumnType("decimal(10,2)");
            entidad.Property(t => t.TiempoAtencionDias).HasColumnType("decimal(10,2)");

            entidad
                .HasOne(t => t.AreaUbicacion)
                .WithMany()
                .HasForeignKey(t => t.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DisponibilidadAbastecimiento>(entidad =>
        {
            entidad.ToTable("DisponibilidadAbastecimiento");

            // Sin llave de negocio ni IdOrigen: el sistema de origen no
            // trae ningún "ID" y no se espera que lo traiga en el futuro,
            // así que no hay índice único de deduplicación en este módulo
            // (ver Models/DisponibilidadAbastecimiento.cs).
            entidad.Property(d => d.Departamento).HasMaxLength(150);
            entidad.Property(d => d.Material).HasMaxLength(200);
            entidad.Property(d => d.Especificar).HasMaxLength(300);
            entidad.Property(d => d.Unidad).HasMaxLength(50);
            entidad.Property(d => d.Comentarios).HasMaxLength(2000);
            entidad.Property(d => d.CantidadEntregada).HasColumnType("decimal(12,2)");

            entidad
                .HasOne(d => d.AreaUbicacion)
                .WithMany()
                .HasForeignKey(d => d.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CumplimientoDocumentacion>(entidad =>
        {
            entidad.ToTable("CumplimientoDocumentacion");

            // Llave de negocio obligatoria y única: un registro por
            // contrato/documento (Code), se edita en vez de duplicarse.
            entidad.Property(c => c.Code).IsRequired().HasMaxLength(50);
            entidad.HasIndex(c => c.Code).IsUnique();

            entidad.Property(c => c.Proveedor).HasMaxLength(200);
            entidad.Property(c => c.TipoContrato).HasMaxLength(100);
            entidad.Property(c => c.Area).HasMaxLength(150);
            entidad.Property(c => c.Responsable).HasMaxLength(150);
            entidad.Property(c => c.Moneda).HasMaxLength(20);
            entidad.Property(c => c.Renovacion).HasMaxLength(500);
            entidad.Property(c => c.Estatus).HasMaxLength(50);
            entidad.Property(c => c.Carpeta).HasMaxLength(500);
            entidad.Property(c => c.MontoIvaIncluido).HasColumnType("decimal(14,2)");

            entidad
                .HasOne(c => c.AreaUbicacion)
                .WithMany()
                .HasForeignKey(c => c.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CumplimientoPrograma>(entidad =>
        {
            entidad.ToTable("CumplimientoPrograma");

            // Índice único filtrado: mismo motivo que ReunionProveedor.IdOrigen
            // (IdOrigen es opcional, SQL Server no permite más de un NULL en
            // un índice único normal).
            entidad.HasIndex(c => c.IdOrigen).IsUnique().HasFilter("[IdOrigen] IS NOT NULL");

            entidad.Property(c => c.AreaEvaluada).HasMaxLength(200);
            entidad.Property(c => c.Hallazgo).HasMaxLength(2000);
            entidad.Property(c => c.Seguimiento).HasMaxLength(2000);
            entidad.Property(c => c.Estatus).HasMaxLength(50);
            entidad.Property(c => c.Prioridad).HasMaxLength(50);
            entidad.Property(c => c.Responsable).HasMaxLength(150);
            entidad.Property(c => c.Tipo).HasMaxLength(100);
            entidad.Property(c => c.CumplimientoGeneralPorcentaje).HasColumnType("decimal(5,2)");

            entidad
                .HasOne(c => c.AreaUbicacion)
                .WithMany()
                .HasForeignKey(c => c.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Seguridad e Higiene (Fase 3.6, 22/sep/2026) ---

        modelBuilder.Entity<ObservacionSeguridad>(entidad =>
        {
            entidad.ToTable("ObservacionSeguridad");

            entidad.Property(o => o.Folio).IsRequired().HasMaxLength(50);
            entidad.HasIndex(o => o.Folio).IsUnique();

            entidad.Property(o => o.Usuario).IsRequired().HasMaxLength(150);
            entidad.Property(o => o.NNomina).HasMaxLength(50);
            entidad.Property(o => o.PersonaObservada).IsRequired().HasMaxLength(200);
            entidad.Property(o => o.Empresa).IsRequired().HasMaxLength(200);
            entidad.Property(o => o.Area).HasMaxLength(150);
            entidad.Property(o => o.Tipo).IsRequired().HasMaxLength(50);
            entidad.Property(o => o.Categorias).IsRequired().HasMaxLength(60);
            entidad.Property(o => o.Estado).IsRequired().HasMaxLength(30);

            entidad
                .HasOne(o => o.AreaUbicacion)
                .WithMany()
                .HasForeignKey(o => o.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SafetyWalk>(entidad =>
        {
            entidad.ToTable("SafetyWalk");

            entidad.Property(s => s.Area).HasMaxLength(150);
            entidad.Property(s => s.Cumplimiento).HasColumnType("decimal(5,2)");

            // Llave de negocio compuesta: no hay Folio, es (Fecha exacta, Planta).
            entidad.HasIndex(s => new { s.Fecha, s.AreaUbicacionId }).IsUnique();

            entidad
                .HasOne(s => s.AreaUbicacion)
                .WithMany()
                .HasForeignKey(s => s.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AccidenteTrabajo>(entidad =>
        {
            entidad.ToTable("AccidenteTrabajo");

            entidad.Property(a => a.Folio).IsRequired().HasMaxLength(50);
            entidad.HasIndex(a => a.Folio).IsUnique();

            entidad.Property(a => a.Area).HasMaxLength(150);
            entidad.Property(a => a.Nombre).IsRequired().HasMaxLength(200);
            entidad.Property(a => a.Compania).IsRequired().HasMaxLength(200);
            entidad.Property(a => a.TipoIncidenteAccidente).IsRequired().HasMaxLength(60);
            entidad.Property(a => a.Lesion).IsRequired().HasMaxLength(60);
            entidad.Property(a => a.ParteLesionada).IsRequired().HasMaxLength(60);
            entidad.Property(a => a.CausaRaiz).IsRequired().HasMaxLength(60);
            entidad.Property(a => a.EstatusAccion1).IsRequired().HasMaxLength(30);
            entidad.Property(a => a.EstatusAccion2).IsRequired().HasMaxLength(30);

            entidad
                .HasOne(a => a.AreaUbicacion)
                .WithMany()
                .HasForeignKey(a => a.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CumplimientoEpp>(entidad =>
        {
            entidad.ToTable("CumplimientoEpp");

            entidad.Property(c => c.IdEpp).IsRequired().HasMaxLength(100);
            entidad.Property(c => c.Descripcion).IsRequired().HasMaxLength(300);
            entidad.Property(c => c.Unidad).HasMaxLength(50);
            entidad.Property(c => c.Talla).HasMaxLength(50);
            entidad.Property(c => c.VidaUtilUnidad).HasMaxLength(50);

            // IdEpp identifica al EPP, no al registro — la llave natural es
            // IdEpp + Fecha de registro (mismo criterio que
            // ActualizacionEquipoCritico: Codigo + FechaRegistro).
            entidad.HasIndex(c => new { c.IdEpp, c.FechaRegistro }).IsUnique();

            entidad
                .HasOne(c => c.AreaUbicacion)
                .WithMany()
                .HasForeignKey(c => c.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstatusLegalPlanta>(entidad =>
        {
            entidad.ToTable("EstatusLegalPlanta");

            entidad.Property(e => e.RequerimientoLegal).IsRequired().HasMaxLength(300);
            entidad.Property(e => e.Autoridad).HasMaxLength(200);
            entidad.Property(e => e.Frecuencia).HasMaxLength(100);
            entidad.Property(e => e.Estatus).HasMaxLength(50);

            // Llave compuesta: Planta + Requerimiento legal + Última fecha
            // de realización (un mismo requerimiento se re-evalúa
            // periódicamente).
            entidad.HasIndex(e => new { e.AreaUbicacionId, e.RequerimientoLegal, e.UltimaFechaRealizacion }).IsUnique();

            entidad
                .HasOne(e => e.AreaUbicacion)
                .WithMany()
                .HasForeignKey(e => e.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EvaluacionProveedorSegHigiene>(entidad =>
        {
            entidad.ToTable("EvaluacionProveedorSegHigiene");

            entidad.Property(e => e.Proveedor).IsRequired().HasMaxLength(200);
            entidad.Property(e => e.Especialidad).HasMaxLength(150);
            entidad.Property(e => e.Kpi).HasColumnType("decimal(10,2)");
            entidad.Property(e => e.Cumplimiento).HasColumnType("decimal(5,2)");

            // Llave compuesta: un proveedor no puede tener 2 evaluaciones el
            // mismo mes en la misma Planta (mismo criterio que EvaluacionVigilancia).
            entidad.HasIndex(e => new { e.Proveedor, e.AreaUbicacionId, e.Mes }).IsUnique();

            entidad
                .HasOne(e => e.AreaUbicacion)
                .WithMany()
                .HasForeignKey(e => e.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Brigada>(entidad =>
        {
            // PROPUESTO — pendiente de confirmar con el usuario.
            entidad.ToTable("Brigada");

            entidad.Property(b => b.Folio).IsRequired().HasMaxLength(50);
            entidad.HasIndex(b => b.Folio).IsUnique();

            entidad.Property(b => b.TipoBrigada).IsRequired().HasMaxLength(60);
            entidad.Property(b => b.NombreBrigadista).IsRequired().HasMaxLength(200);
            entidad.Property(b => b.PuestoBrigada).IsRequired().HasMaxLength(60);
            entidad.Property(b => b.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(b => b.Observaciones).HasMaxLength(2000);

            entidad
                .HasOne(b => b.AreaUbicacion)
                .WithMany()
                .HasForeignKey(b => b.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UsuarioModulo>(entidad =>
        {
            entidad.ToTable("UsuarioModulo");

            entidad.Property(m => m.ModuloClave).IsRequired().HasMaxLength(50);
            entidad.HasIndex(m => new { m.UsuarioId, m.ModuloClave }).IsUnique();

            // A diferencia de los demás catálogos (Restrict), aquí sí se
            // usa Cascade: UsuarioModulo es una lista de detalle propia del
            // Usuario (sus permisos), no un catálogo compartido — al
            // eliminar un Usuario tiene sentido borrar también sus accesos.
            entidad
                .HasOne(m => m.Usuario)
                .WithMany(u => u.ModulosAsignados)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UsuarioModuloUbicacion>(entidad =>
        {
            entidad.ToTable("UsuarioModuloUbicacion");

            entidad.Property(m => m.ModuloClave).IsRequired().HasMaxLength(50);
            entidad.HasIndex(m => new { m.UsuarioId, m.ModuloClave, m.AreaUbicacionId }).IsUnique();

            // Igual que UsuarioModulo: es una lista de detalle propia del
            // Usuario (sus permisos), no un catálogo compartido — Cascade
            // hacia Usuario. Hacia AreaUbicacion sí es Restrict (no se debe
            // poder borrar una Planta que un usuario tiene asignada como
            // restricción de captura).
            entidad
                .HasOne(m => m.Usuario)
                .WithMany(u => u.ModuloUbicacionesAsignadas)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entidad
                .HasOne(m => m.AreaUbicacion)
                .WithMany()
                .HasForeignKey(m => m.AreaUbicacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Datos semilla: estructura organizacional conocida hasta ahora ---
        // (ver requerimientos-dashboard-general.md en el proyecto)

        modelBuilder.Entity<Direccion>().HasData(
            new Direccion { Id = 1, Nombre = "Administración Centralizada" },
            new Direccion { Id = 2, Nombre = "Operaciones" }
        );

        modelBuilder.Entity<Departamento>().HasData(
            new Departamento { Id = 1, Nombre = "Seguridad", DireccionId = 1 },
            new Departamento { Id = 2, Nombre = "Recursos Humanos", DireccionId = 1 }
        );

        modelBuilder.Entity<Area>().HasData(
            new Area { Id = 1, Nombre = "Seguridad Patrimonial", DepartamentoId = 1 },
            new Area { Id = 2, Nombre = "EHS", DepartamentoId = 1 },
            new Area { Id = 3, Nombre = "IT", DepartamentoId = 1 },
            new Area { Id = 4, Nombre = "RH1", DepartamentoId = 2 },
            new Area { Id = 5, Nombre = "RH2", DepartamentoId = 2 }
        );

        // "CDMX - Oficina de Ventas" (Id 4) se renombró a "CDMX" (14/sep/2026)
        // para que coincida exactamente con el texto que trae el sistema de
        // mesa de ayuda en la columna "Ubicación" de los tickets exportados.
        // "Monterrey" (Id 6) y "Planta 5" (Id 7) se agregaron por la misma
        // razón: aparecían en tickets reales y no existían en el catálogo.
        modelBuilder.Entity<Ubicacion>().HasData(
            new Ubicacion { Id = 1, Nombre = "Planta 1" },
            new Ubicacion { Id = 2, Nombre = "Planta 2" },
            new Ubicacion { Id = 3, Nombre = "Planta 3" },
            new Ubicacion { Id = 4, Nombre = "CDMX" },
            new Ubicacion { Id = 5, Nombre = "Acapulco - Soporte Técnico" },
            new Ubicacion { Id = 6, Nombre = "Monterrey" },
            new Ubicacion { Id = 7, Nombre = "Planta 5" }
        );

        // IT (AreaId 3) es la única área confirmada en estas ubicaciones.
        // El resto de combinaciones (Seguridad Patrimonial, EHS, RH1, RH2 por
        // ubicación) todavía no está definido, así que no se siembra aquí:
        // se dará de alta desde la pantalla de administración de catálogos
        // cuando se precise (para eso es un catálogo dinámico).
        modelBuilder.Entity<AreaUbicacion>().HasData(
            new AreaUbicacion { Id = 1, AreaId = 3, UbicacionId = 1 },
            new AreaUbicacion { Id = 2, AreaId = 3, UbicacionId = 2 },
            new AreaUbicacion { Id = 3, AreaId = 3, UbicacionId = 3 },
            new AreaUbicacion { Id = 4, AreaId = 3, UbicacionId = 4 },
            new AreaUbicacion { Id = 5, AreaId = 3, UbicacionId = 5 },
            new AreaUbicacion { Id = 6, AreaId = 3, UbicacionId = 6 },
            new AreaUbicacion { Id = 7, AreaId = 3, UbicacionId = 7 }
        );

        // Usuario administrador inicial (Fase 1). Contraseña: "admin123"
        // (mismo valor que se usaba en el login temporal de la Fase 0, para
        // no romper la credencial de prueba que el usuario ya conoce). El
        // hash fue generado fuera de línea con el mismo algoritmo que usa
        // PasswordHasher (PBKDF2-SHA256, 100000 iteraciones, salt de 16
        // bytes, hash de 32 bytes) para poder sembrarlo como dato fijo.
        //
        // 19/sep/2026: este usuario cambió de Rol = CEO a Rol = Superadmin.
        // Superadmin es un rol de sistema (administra Usuarios y Catálogos)
        // y no participa en la jerarquía de negocio, por eso no tiene
        // Dirección/Departamento/Área asignada (quedan en null, igual que
        // antes). Si ya tenías esta fila en tu base de datos con Rol="CEO",
        // la migración que actualice este seed la sobrescribirá a
        // "Superadmin" — no hace falta tocar nada a mano.
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                NombreUsuario = "admin",
                PasswordHash = "100000.eE4Fsv4smUeDOysC5huSKw==.IyukRhjeKxflKPAplF7FfngTa87nIAZrBQTHXpL313w=",
                NombreCompleto = "Administrador General",
                Rol = Roles.Superadmin,
                Activo = true,
            }
        );

        // El admin semilla ya NO recibe módulos de captura de IT: al ser
        // Superadmin no interactúa con las áreas/direcciones de negocio, solo
        // administra Usuarios y Catálogos. Si en algún momento se necesita
        // que además capture algo de IT, se le puede dar de alta el módulo a
        // mano desde la pantalla de administración de Usuarios.
    }
}
