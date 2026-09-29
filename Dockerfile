# Etapa 1: build — SDK .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restauración por separado (mejora caché de capas Docker)
COPY NuGet.config ./
COPY Agendamiento.Api/Agendamiento.Api.csproj Agendamiento.Api/
RUN dotnet restore Agendamiento.Api/Agendamiento.Api.csproj

# Compilación y publicación en Release
COPY Agendamiento.Api/ Agendamiento.Api/
RUN dotnet publish Agendamiento.Api/Agendamiento.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: runtime — solo ASP.NET (imagen ligera)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Puerto interno estándar de contenedores .NET
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Agendamiento.Api.dll"]
