# API de Clientes

API para cadastro e gerenciamento de clientes desenvolvida em .NET 8 com Clean Architecture, DDD e Repository Pattern.

## Como Executar

### Com Docker

```bash
cd ApiClientes
docker-compose up --build
```

A API estará disponível em `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- Health: `http://localhost:8080/health`

### Localmente

```bash
cd ApiClientes
dotnet restore

# Instalar EF Core Tools (se necessário)
# Se houver erro de autenticação, use apenas nuget.org:
dotnet tool install --global dotnet-ef --add-source https://api.nuget.org/v3/index.json

# Criar migrations (já existe InitialCreate, mas para novas migrations):
dotnet ef migrations add NomeDaMigration --project . --startup-project .

# Aplicar migrations:
dotnet ef database update --project . --startup-project .

dotnet run
```

A API estará disponível em `http://localhost:5000` ou `http://localhost:5001`

## Endpoints

- `POST /clientes` - Cadastrar cliente
- `GET /clientes` - Listar clientes
- `GET /clientes/{id}` - Obter cliente por ID
- `GET /health` - Health check

## Tecnologias

- .NET 8.0
- Entity Framework Core
- SQLite
- Redis
- Docker
