FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ArticleService/ArticleService.csproj ArticleService/
COPY Monitor/Monitor.csproj Monitor/

RUN dotnet restore ArticleService/ArticleService.csproj

COPY ArticleService/ ArticleService/
COPY Monitor/ Monitor/

WORKDIR /src/ArticleService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ArticleService.dll"]
