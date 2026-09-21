FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DraftService/DraftService.csproj DraftService/
COPY Monitor/Monitor.csproj Monitor/

RUN dotnet restore DraftService/DraftService.csproj

COPY DraftService/ DraftService/
COPY Monitor/ Monitor/

WORKDIR /src/DraftService

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "DraftService.dll"]
