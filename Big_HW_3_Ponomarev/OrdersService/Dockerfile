﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OrdersService/OrdersService.csproj ./OrdersService/
RUN dotnet restore ./OrdersService/OrdersService.csproj

COPY OrdersService/. ./OrdersService/
WORKDIR /src/OrdersService
RUN dotnet publish OrdersService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrdersService.dll"]
