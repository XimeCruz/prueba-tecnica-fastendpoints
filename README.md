# TaskManager Microservice

Microservicio de gestión de tareas construido con .NET 8, FastEndpoints y Clean Architecture.

## Arquitectura

Clean Architecture en 4 capas:

- **Domain** – Entidades e interfaces (TaskItem, ITaskRepository)
- **Application** – DTOs y validaciones (FluentValidation)
- **Infrastructure** – EF Core + PostgreSQL, implementación del repositorio
- **API** – Endpoints con FastEndpoints, Swagger

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | /tasks | Crear tarea |
| GET | /tasks | Listar con paginación por headers |
| PUT | /tasks/{id} | Actualizar estado |
| DELETE | /tasks/{id} | Eliminar tarea |
| GET | /metrics | Estadísticas generales |

### Paginación
Enviar headers en el GET:
- `X-Page` – página actual
- `X-PageSize` – cantidad por página

Responde con:
- `X-TotalCount` – total de registros
- `X-TotalPages` – total de páginas

## Stack

- .NET 8
- FastEndpoints 5.31
- Entity Framework Core + Npgsql (PostgreSQL)
- FluentValidation
- XUnit + WebApplicationFactory + FluentAssertions

## Cómo correr

### Requisitos
- .NET 8 SDK
- PostgreSQL corriendo en localhost

### Pasos
```bash
git clone <repo>
cd prueba-tecnica-fastendpoints
dotnet restore TaskManager.slnx


# Aplicar migraciones
cd TaskManager.API
dotnet ef database update

# Correr la API
dotnet run
```

Swagger disponible en: `http://localhost:5066/swagger`

### Tests
```bash
cd TaskManager.Tests
dotnet test
```

> Los tests usan una base de datos separada `taskmanager_test` que se crea y destruye automáticamente.

## Estructura
```
TaskManager/
├── TaskManager.Domain/         # Entidades e interfaces
├── TaskManager.Application/    # DTOs y validaciones
├── TaskManager.Infrastructure/ # EF Core + repositorios
├── TaskManager.API/            # Endpoints + Program.cs
└── TaskManager.Tests/          # Pruebas de integración
```
