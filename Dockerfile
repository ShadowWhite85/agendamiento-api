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

# Sin privilegios de root: usuario 'app' que traen las imágenes de .NET (APP_UID).
# Necesita escribir en /app porque ahí se crea la base SQLite.
RUN chown -R $APP_UID:$APP_UID /app
USER $APP_UID

# Puerto interno estándar de contenedores .NET
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Agendamiento.Api.dll"]
