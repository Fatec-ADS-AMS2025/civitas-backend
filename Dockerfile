# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

COPY Civitas.WebAPI/Civitas.WebAPI.csproj Civitas.WebAPI/
COPY Civitas.WebAPI.Tests/Civitas.WebAPI.Tests.csproj Civitas.WebAPI.Tests/
RUN dotnet restore Civitas.WebAPI/Civitas.WebAPI.csproj

FROM restore AS build
COPY . .
RUN dotnet build Civitas.WebAPI/Civitas.WebAPI.csproj -c Release --no-restore

FROM build AS publish
RUN dotnet publish Civitas.WebAPI/Civitas.WebAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-build \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

COPY --from=publish /app/publish .

EXPOSE 8080

USER $APP_UID

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl --fail http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "Civitas.WebAPI.dll"]
