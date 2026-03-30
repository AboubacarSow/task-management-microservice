# Stage 1 — Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
USER $APP_UID

# Stage 2 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

# Copy solution + project files
COPY *.sln .
COPY src/services/dispatcher-service/dispatcher-service.csproj ./src/services/dispatcher-service/
COPY src/shared/shared.csproj                                            ./src/shared/

# Restore dependencies
RUN dotnet restore src/services/dispatcher-service/dispatcher-service.csproj

# Copy full source
COPY src/services/dispatcher-service/ ./src/services/dispatcher-service/
COPY src/shared/                             ./src/shared/


# Build + publish
RUN dotnet publish src/services/dispatcher-service/dispatcher-service.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 3 — Final image
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

# Ocelot will listen on this port
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "dispatcher-service.dll"]