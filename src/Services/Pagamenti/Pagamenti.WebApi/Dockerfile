# syntax=docker/dockerfile:1

# ---------- STAGE 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=${GITHUB_TOKEN}

WORKDIR /src

COPY nuget.config ./
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Pagamenti/ src/Services/Pagamenti/

RUN dotnet restore src/Services/Pagamenti/Pagamenti.WebApi/Pagamenti.WebApi.csproj
RUN dotnet publish src/Services/Pagamenti/Pagamenti.WebApi/Pagamenti.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- STAGE 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Pagamenti.WebApi.dll"]
