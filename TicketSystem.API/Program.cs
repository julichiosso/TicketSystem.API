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
using TicketSystem.API.Hubs;
using TicketSystem.API.Middleware;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Aplicacion.Servicios;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Infraestructura.Datos;
using TicketSystem.Infraestructura.Repositorios;
using TicketSystem.Infraestructura.Seed;
using TicketSystem.Infraestructura.Servicios;

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
    Log.Information("Starting TicketSystem API...");

    var builder = WebApplication.CreateBuilder(args);

    var port = Environment.GetEnvironmentVariable("PORT");
    if (port != null)
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    builder.Host.UseSerilog();

    var allowedOrigins = builder.Configuration
        .GetSection("AppSettings:AllowedCorsOrigins")
        .Get<string[]>() ?? [];

    if (allowedOrigins.Length == 0)
    {
        var originsEnv = Environment.GetEnvironmentVariable("ALLOWED_CORS_ORIGINS") ?? "";
        allowedOrigins = originsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }

    Log.Information("CORS origins configured: {Origins}", string.Join(", ", allowedOrigins));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            if (builder.Environment.IsDevelopment() && allowedOrigins.Length == 0)
            {
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
            else
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
        });
    });

    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgres"))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Prefer;Trust Server Certificate=true";
    }

    builder.Services.AddDbContext<TicketSystemDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddScoped<ITicketRepository, TicketRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddScoped<ITicketService, TicketService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

    builder.Services.AddSignalR();

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
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });

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

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<TicketSystemDbContext>("database");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "TicketSystem API",
            Version = "v1",
            Description = "IT Support Ticketing System"
        });

        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter JWT token ONLY"
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

    var app = builder.Build();

    app.UseCors("AllowFrontend");
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseStaticFiles();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health");
    app.MapControllers();
    app.MapHub<TicketHub>("/hubs/tickets");

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketSystemDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P07")
        {
            Log.Warning("Migration conflict detected (tables already exist). Marking pending migrations as applied...");

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            foreach (var migration in pendingMigrations)
            {
                await context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ({0}, {1}) ON CONFLICT DO NOTHING",
                    migration,
                    "8.0.0");
                Log.Information("Marked migration {Migration} as applied.", migration);
            }
        }

        await DataSeeder.SeedAsync(context, passwordHasher);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
