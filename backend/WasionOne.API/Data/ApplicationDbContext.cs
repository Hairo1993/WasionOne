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
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                NombreUsuario = "admin",
                PasswordHash = "100000.eE4Fsv4smUeDOysC5huSKw==.IyukRhjeKxflKPAplF7FfngTa87nIAZrBQTHXpL313w=",
                NombreCompleto = "Administrador General",
                Rol = Roles.Ceo,
                Activo = true,
            }
        );
    }
}
