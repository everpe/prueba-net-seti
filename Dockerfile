# ─── Etapa 1: Build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias (cache layer)
COPY AcmePedidosAPI.csproj ./
RUN dotnet restore --packages /root/.nuget/packages

# Copiar el resto del código y compilar
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ─── Etapa 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Copiar artefactos publicados
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AcmePedidosAPI.dll"]
