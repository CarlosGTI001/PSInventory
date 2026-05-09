FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish "./PSInventory.Web/PSInventory.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8090

ENTRYPOINT ["dotnet", "PSInventory.Web.dll"]
