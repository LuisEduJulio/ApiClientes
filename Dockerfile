FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# Instalar wget para healthcheck
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiClientes.csproj", "./"]
RUN dotnet restore "ApiClientes.csproj"
COPY . .
RUN dotnet build "ApiClientes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiClientes.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Criar diretório para SQLite se necessário
RUN mkdir -p /app/data && chmod 777 /app/data
ENTRYPOINT ["dotnet", "ApiClientes.dll"]
