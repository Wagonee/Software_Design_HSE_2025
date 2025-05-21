FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FileStoringService.csproj", "."]
RUN dotnet restore "./FileStoringService.csproj"
COPY . .
RUN dotnet build "FileStoringService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FileStoringService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/storage && chown app:app /app/storage
USER app

ENTRYPOINT ["dotnet", "FileStoringService.dll"]