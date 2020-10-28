#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Web/iBookStoreMVC/iBookStoreMVC.csproj", "Web/iBookStoreMVC/"]
COPY ["BuildingBlocks/iBookStoreCommon/iBookStoreCommon.csproj", "BuildingBlocks/iBookStoreCommon/"]
COPY ["Services/UserManagement/UserManagement.API/UserManagement.API.csproj", "Services/UserManagement/UserManagement.API/"]
COPY ["BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
RUN dotnet restore "Web/iBookStoreMVC/iBookStoreMVC.csproj"
COPY . .
WORKDIR "/src/Web/iBookStoreMVC"
RUN dotnet build "iBookStoreMVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "iBookStoreMVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "iBookStoreMVC.dll"]
