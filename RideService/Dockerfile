ARG BUILD_CONFIGURATION=Release
ARG PROJECT_NAME=RideService
ARG PROJECT_PATH=${PROJECT_NAME}/${PROJECT_NAME}.csproj
ARG APP_DLL=${PROJECT_NAME}.dll

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION
ARG PROJECT_NAME
ARG PROJECT_PATH
WORKDIR /src

COPY ${PROJECT_PATH} ${PROJECT_PATH}
RUN dotnet restore ${PROJECT_PATH}

COPY . .

WORKDIR /src/${PROJECT_NAME}

RUN dotnet build ${PROJECT_NAME}.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION
ARG PROJECT_NAME
WORKDIR /src/${PROJECT_NAME}

RUN dotnet publish ${PROJECT_NAME}.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG PROJECT_NAME
ARG APP_DLL
LABEL maintainer="BackendFans83@github.com" \
      version="10.0" \
      description="ASP.NET Core application"

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "${APP_DLL}"]
