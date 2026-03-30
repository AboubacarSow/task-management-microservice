# Stage 1 — Runtime base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

USER root

RUN mkdir -p ./keys \
&& chmod 777 ./keys 

USER $APP_UID

# Stage 2 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

COPY *.sln .
COPY src/services/authentication-service/authentication-service.csproj  ./src/services/authentication-service/
COPY src/shared/shared.csproj                                            ./src/shared/

RUN dotnet restore src/services/authentication-service/authentication-service.csproj

COPY src/services/authentication-service/    ./src/services/authentication-service/
COPY src/shared/                             ./src/shared/

RUN dotnet publish src/services/authentication-service/authentication-service.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 3 — Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5004

ENTRYPOINT ["dotnet", "authentication-service.dll"]