FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "src/BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["src/Services/Log/Log.API/Log.API.csproj", "src/Services/Log/Log.API/"]
COPY ["src/Services/Log/Log.Infrastructure/Log.Infrastructure.csproj", "src/Services/Log/Log.Infrastructure/"]
COPY ["src/Services/Log/Log.Application/Log.Application.csproj", "src/Services/Log/Log.Application/"]
COPY ["src/Services/Log/Log.Domain/Log.Domain.csproj", "src/Services/Log/Log.Domain/"]

RUN dotnet restore "src/Services/Log/Log.API/Log.API.csproj"

COPY . .

RUN dotnet build "src/Services/Log/Log.API/Log.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Services/Log/Log.API/Log.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Log.API.dll"]
