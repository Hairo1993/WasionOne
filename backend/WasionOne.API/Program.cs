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
