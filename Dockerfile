FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish "./PSInventory.Web/PSInventory.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Crear el directorio de datos y asegurar permisos para SQLite
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
RUN mkdir -p /var/psinventory && chmod 777 /var/psinventory

COPY --from=build /app/publish .

EXPOSE 9000

ENV ASPNETCORE_HTTP_PORTS=9000

# Healthcheck para monitoreo (útil para Coolify/Docker)
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:9000/ || exit 1

# Forzamos la ruta que espera el código C# en Linux
ENV ConnectionStrings__DefaultConnection="Data Source=/var/psinventory/psinventory.db"

# Declaramos el volumen para persistencia
VOLUME ["/var/psinventory"]

ENTRYPOINT ["dotnet", "PSInventory.Web.dll"]
