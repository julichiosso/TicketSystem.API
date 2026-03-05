# ── Etapa 1: build ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todos los .csproj y restaurar dependencias primero (cache de capas)
COPY TicketSystem.Dominio/TicketSystem.Dominio.csproj           TicketSystem.Dominio/
COPY TicketSystem.Aplicacion/TicketSystem.Aplicacion.csproj     TicketSystem.Aplicacion/
COPY TicketSystem.Infraestructura/TicketSystem.Infraestructura.csproj TicketSystem.Infraestructura/
COPY TicketSystem.API/TicketSystem.API.csproj                   TicketSystem.API/

RUN dotnet restore TicketSystem.API/TicketSystem.API.csproj

# Copiar el resto del código
COPY TicketSystem.Dominio/           TicketSystem.Dominio/
COPY TicketSystem.Aplicacion/        TicketSystem.Aplicacion/
COPY TicketSystem.Infraestructura/   TicketSystem.Infraestructura/
COPY TicketSystem.API/               TicketSystem.API/

# Publicar en modo Release
RUN dotnet publish TicketSystem.API/TicketSystem.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Etapa 2: runtime ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Crear carpetas necesarias
RUN mkdir -p /app/logs /app/wwwroot/uploads

COPY --from=build /app/publish .

# Render usa el puerto 10000 por defecto
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "TicketSystem.API.dll"]
