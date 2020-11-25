#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

ENV TZ=Pacific/Auckland
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Services/Ordering/Ordering.BackgroundTasks/Ordering.BackgroundTasks.csproj", "Services/Ordering/Ordering.BackgroundTasks/"]
COPY ["BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/iBookStoreCommon/iBookStoreCommon.csproj", "BuildingBlocks/iBookStoreCommon/"]
RUN dotnet restore "Services/Ordering/Ordering.BackgroundTasks/Ordering.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/Services/Ordering/Ordering.BackgroundTasks"
RUN dotnet build "Ordering.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.BackgroundTasks.dll"]
