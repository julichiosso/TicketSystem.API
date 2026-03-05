using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using TicketSystem.API.Middleware;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Aplicacion.Servicios;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Infraestructura.Datos;
using TicketSystem.Infraestructura.Repositorios;
using TicketSystem.Infraestructura.Seed;

// ─── SERILOG ──────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/ticketsystem-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Iniciando TicketSystem API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // ─── CORS ────────────────────────────────────────────────────────────────────
    var allowedOrigins = builder.Configuration
        .GetSection("AppSettings:AllowedCorsOrigins")
        .Get<string[]>() ?? [];

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            if (builder.Environment.IsDevelopment() && allowedOrigins.Length == 0)
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
            else
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        });
    });

    // ─── CONTROLLERS ─────────────────────────────────────────────────────────────
    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // ─── DATABASE ─────────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<TicketSystemDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    // ─── REPOSITORIES & SERVICES ─────────────────────────────────────────────────
    builder.Services.AddScoped<IRepositorioTickets, RepositorioTickets>();
    builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
    builder.Services.AddScoped<IServicioTickets, ServicioTickets>();
    builder.Services.AddScoped<IServicioUsuarios, ServicioUsuarios>();
    builder.Services.AddScoped<IServicioEmail, TicketSystem.Infraestructura.Servicios.ServicioEmail>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

    // ─── JWT ──────────────────────────────────────────────────────────────────────
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var jwtKey = jwtSection["Key"]
        ?? throw new InvalidOperationException("JWT Key is not configured.");

    if (jwtKey.Length < 32)
        throw new InvalidOperationException($"JWT Key must be at least 32 characters. Current: {jwtKey.Length}");

    var key = Encoding.UTF8.GetBytes(jwtKey);
    var jwtIssuer = jwtSection["Issuer"] ?? "TicketSystemAPI";
    var jwtAudience = jwtSection["Audience"] ?? "TicketSystemFrontend";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

    // ─── RATE LIMITING ────────────────────────────────────────────────────────────
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("AuthPolicy", limiter =>
        {
            limiter.PermitLimit = 10;
            limiter.Window = TimeSpan.FromMinutes(1);
            limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiter.QueueLimit = 0;
        });
        options.RejectionStatusCode = 429;
    });

    // ─── HEALTH CHECKS ────────────────────────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<TicketSystemDbContext>("database");

    // ─── SWAGGER ──────────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "TicketSystem API",
            Version = "v1",
            Description = "Sistema de tickets de soporte técnico"
        });

        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Ingrese SOLO el token JWT"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // ─────────────────────────────────────────────────────────────────────────────
    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowFrontend");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health");
    app.MapControllers();

    // ─── DATABASE MIGRATION & SEED ───────────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketSystemDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        await context.Database.MigrateAsync();
        await DataSeeder.SeedAsync(context, passwordHasher);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar.");
}
finally
{
    Log.CloseAndFlush();
}