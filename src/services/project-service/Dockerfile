# Stage 1 — Runtime base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
USER $APP_UID

# Stage 2 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

COPY *.sln .
COPY src/services/project-service/project-service.csproj   ./src/services/project-service/
COPY src/shared/shared.csproj                               ./src/shared/
COPY src/shared.messaging/shared.messaging.csproj           ./src/shared.messaging/

RUN dotnet restore src/services/project-service/project-service.csproj

COPY src/services/project-service/    ./src/services/project-service/
COPY src/shared/                      ./src/shared/
COPY src/shared.messaging/            ./src/shared.messaging/


RUN dotnet publish src/services/project-service/project-service.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 3 — Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "project-service.dll"]