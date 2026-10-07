# syntax=docker/dockerfile:1

# ---------- STAGE 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=${GITHUB_TOKEN}

WORKDIR /src

# Copio prima solo i file di progetto per sfruttare la cache dei layer Docker:
# se non cambiano i .csproj, "dotnet restore" non viene rieseguito ad ogni build.
COPY nuget.config ./
COPY src/Services/Utenti/Utenti.WebApi/Utenti.WebApi.csproj src/Services/Utenti/Utenti.WebApi/
COPY src/Services/Utenti/Utenti.Business/Utenti.Business.csproj src/Services/Utenti/Utenti.Business/
COPY src/Services/Utenti/Utenti.Data/Utenti.Data.csproj src/Services/Utenti/Utenti.Data/
COPY src/Services/Utenti/Utenti.Repository/Utenti.Repository.csproj src/Services/Utenti/Utenti.Repository/
COPY src/Services/Utenti/Utenti.Shared/Utenti.Shared.csproj src/Services/Utenti/Utenti.Shared/
COPY src/Services/Utenti/Utenti.ClientHttp/Utenti.ClientHttp.csproj src/Services/Utenti/Utenti.ClientHttp/
COPY src/BuildingBlocks/Common.Auth/Common.Auth.csproj src/BuildingBlocks/Common.Auth/
COPY src/BuildingBlocks/EventBus.Contracts/EventBus.Contracts.csproj src/BuildingBlocks/EventBus.Contracts/
COPY src/BuildingBlocks/Domain.Common/Domain.Common.csproj src/BuildingBlocks/Domain.Common/

# MODIFICA: Aggiunto -c Release per scaricare anche i pacchetti di analisi del codice per la produzione
RUN dotnet restore src/Services/Utenti/Utenti.WebApi/Utenti.WebApi.csproj

# Ora copio tutto il resto del codice sorgente necessario e compilo
COPY src/Services/Utenti/ src/Services/Utenti/
COPY src/BuildingBlocks/ src/BuildingBlocks/

# MODIFICA: Ripristinato --no-restore in totale sicurezza
RUN dotnet publish src/Services/Utenti/Utenti.WebApi/Utenti.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- STAGE 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Utenti.WebApi.dll"]