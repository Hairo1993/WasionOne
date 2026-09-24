using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WasionOne.API.Data;
using WasionOne.API.Interfaces;
using WasionOne.API.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICatalogosService, CatalogosService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioContexto, UsuarioContexto>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IIncidenteCriticoService, IncidenteCriticoService>();
builder.Services.AddScoped<IRespaldoService, RespaldoService>();
builder.Services.AddScoped<IPlaticaService, PlaticaService>();
builder.Services.AddScoped<IAuditoriaEquipoService, AuditoriaEquipoService>();
builder.Services.AddScoped<IDisponibilidadServidorService, DisponibilidadServidorService>();
builder.Services.AddScoped<IDisponibilidadRedService, DisponibilidadRedService>();
builder.Services.AddScoped<IAlmacenamientoServidorService, AlmacenamientoServidorService>();
builder.Services.AddScoped<IRecorridoService, RecorridoService>();
builder.Services.AddScoped<ICredencializacionService, CredencializacionService>();
builder.Services.AddScoped<ITestConsignaService, TestConsignaService>();
builder.Services.AddScoped<IAlcoholimetriaService, AlcoholimetriaService>();
builder.Services.AddScoped<IDopingService, DopingService>();
builder.Services.AddScoped<ILockerService, LockerService>();
builder.Services.AddScoped<IValeSalidaService, ValeSalidaService>();
builder.Services.AddScoped<IEstacionamientoService, EstacionamientoService>();
builder.Services.AddScoped<IReunionProveedorService, ReunionProveedorService>();
builder.Services.AddScoped<IEvaluacionVigilanciaService, EvaluacionVigilanciaService>();
builder.Services.AddScoped<IActualizacionEquipoCriticoService, ActualizacionEquipoCriticoService>();
builder.Services.AddScoped<IMantenimientoVehicularService, MantenimientoVehicularService>();
builder.Services.AddScoped<ITiempoRespuestaResolucionService, TiempoRespuestaResolucionService>();
builder.Services.AddScoped<IDisponibilidadAbastecimientoService, DisponibilidadAbastecimientoService>();
builder.Services.AddScoped<ICumplimientoDocumentacionService, CumplimientoDocumentacionService>();
builder.Services.AddScoped<ICumplimientoProgramaService, CumplimientoProgramaService>();
builder.Services.AddScoped<IObservacionSeguridadService, ObservacionSeguridadService>();
builder.Services.AddScoped<ISafetyWalkService, SafetyWalkService>();
builder.Services.AddScoped<IAccidenteTrabajoService, AccidenteTrabajoService>();
builder.Services.AddScoped<ICumplimientoEppService, CumplimientoEppService>();
builder.Services.AddScoped<IEstatusLegalPlantaService, EstatusLegalPlantaService>();
builder.Services.AddScoped<IEvaluacionProveedorSegHigieneService, EvaluacionProveedorSegHigieneService>();
builder.Services.AddScoped<IBrigadaService, BrigadaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
// Importación masiva combinada (24/sep/2026) — agrega, sin duplicar su
// lógica, todos los I*Service de arriba que ya soportan importación.
builder.Services.AddScoped<IImportacionMasivaService, ImportacionMasivaService>();

var jwtConfig = builder.Configuration.GetSection("Jwt");
var claveSecreta = jwtConfig["ClaveSecreta"]!;

builder.Services
    .AddAuthentication(opciones =>
    {
        opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opciones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta)),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("FrontendAngular", politica =>
    {
        politica
            .WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- Middleware pipeline ---
app.UseHttpsRedirection();
app.UseCors("FrontendAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
