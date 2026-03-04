FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "BuildingBlocks/EventBus.RabbitMQ/"]
COPY ["Services/Product/Product.API/Product.API.csproj", "Services/Product/Product.API/"]
COPY ["Services/Product/Product.Infrastructure/Product.Infrastructure.csproj", "Services/Product/Product.Infrastructure/"]
COPY ["Services/Product/Product.Application/Product.Application.csproj", "Services/Product/Product.Application/"]
COPY ["Services/Product/Product.Domain/Product.Domain.csproj", "Services/Product/Product.Domain/"]

RUN dotnet restore "Services/Product/Product.API/Product.API.csproj"

COPY . .

RUN dotnet build "Services/Product/Product.API/Product.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Services/Product/Product.API/Product.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.API.dll"]
