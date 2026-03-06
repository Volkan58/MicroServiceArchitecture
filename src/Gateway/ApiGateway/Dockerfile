FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["src/Gateway/ApiGateway/ApiGateway.csproj", "src/Gateway/ApiGateway/"]

RUN dotnet restore "src/Gateway/ApiGateway/ApiGateway.csproj"

COPY . .

RUN dotnet build "src/Gateway/ApiGateway/ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Gateway/ApiGateway/ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
