FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY PublisherService/PublisherService.csproj PublisherService/
COPY Monitor/Monitor.csproj Monitor/

RUN dotnet restore PublisherService/PublisherService.csproj

COPY PublisherService/ PublisherService/
COPY Monitor/ Monitor/

WORKDIR /src/PublisherService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PublisherService.dll"]
