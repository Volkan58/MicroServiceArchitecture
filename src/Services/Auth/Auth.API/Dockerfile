FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["Auth.API/Auth.API.csproj", "Auth.API/"]
COPY ["Auth.Infrastructure/Auth.Infrastructure.csproj", "Auth.Infrastructure/"]
COPY ["Auth.Application/Auth.Application.csproj", "Auth.Application/"]
COPY ["Auth.Domain/Auth.Domain.csproj", "Auth.Domain/"]

RUN dotnet restore "Auth.API/Auth.API.csproj"

COPY . .

RUN dotnet build "Auth.API/Auth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Auth.API/Auth.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.API.dll"]
