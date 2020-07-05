# ** Build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY SolarTimeProvider SolarTimeProvider

RUN dotnet publish SolarTimeProvider -o SolarTimeProvider/artifacts -c Release

# ** Run

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS run
WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=build /src/SolarTimeProvider/artifacts ./

ENTRYPOINT ["dotnet", "SolarTimeProvider.dll"]