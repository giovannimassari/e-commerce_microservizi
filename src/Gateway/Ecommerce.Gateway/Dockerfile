# syntax=docker/dockerfile:1

# ---------- STAGE 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Gateway/Ecommerce.Gateway/ src/Gateway/Ecommerce.Gateway/

RUN dotnet restore src/Gateway/Ecommerce.Gateway/Ecommerce.Gateway.csproj
RUN dotnet publish src/Gateway/Ecommerce.Gateway/Ecommerce.Gateway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- STAGE 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Ecommerce.Gateway.dll"]
