FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY NewsletterService/NewsletterService.csproj NewsletterService/
COPY Monitor/Monitor.csproj Monitor/

RUN dotnet restore NewsletterService/NewsletterService.csproj

COPY NewsletterService/ NewsletterService/
COPY Monitor/ Monitor/

WORKDIR /src/NewsletterService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NewsletterService.dll"]
