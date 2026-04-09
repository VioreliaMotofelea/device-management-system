# Backend (`.NET 8`)

This folder contains the complete backend implementation for the Device Management System.

## Projects

- `DeviceManagement.Api`: ASP.NET Core Web API (controllers, auth, middleware, Swagger).
- `DeviceManagement.Application`: contracts and business-level components (DTOs, interfaces, validators, options).
- `DeviceManagement.Infrastructure`: EF Core, repositories, service implementations, dependency wiring, development seeding.
- `DeviceManagement.Domain`: domain entities and constants.
- `DeviceManagement.Tests.Unit`: unit tests for services, validators/parsers, middleware, and security helpers.
- `DeviceManagement.Tests.Integration`: API and infrastructure integration tests using `WebApplicationFactory`.

## Main Features Covered

- Devices: full CRUD via the API and UI.
- Authentication (register/login) with JWT.
- User records: secured `/api/users` endpoints support list, get-by-id, create, update, and delete (API/Swagger); the Angular UI focuses on devices, not a full user-admin console.
- Device assignment and unassignment flows.
- AI device description generation with OpenAI integration and deterministic template fallback when no API key is configured.
- Bonus ranked free-text device search (normalized and deterministic).

## Prerequisites

- .NET 8 SDK
- SQL Server (local Windows instance or Docker on macOS)
- An existing connection string configured for the API

## Database

Before running the API, create and seed the database using the SQL scripts from the repository root:

- `database/001_create_database.sql`
- `database/002_seed_data.sql`

Run them using SQL Server Management Studio, DataGrip, or another SQL Server client.

## Configuration

Configure the development connection string in:

- `DeviceManagement.Api/appsettings.Development.json`

**Windows vs macOS:** on Windows, `DefaultConnection` usually targets LocalDB or a local SQL Server instance. On macOS or Linux, SQL Server often runs in Docker—use the published host/port (typically `Server=localhost,1433;...`) and the SQL credentials you set on the container so they match how you connect when running the SQL scripts.

For secrets such as OpenAI API keys, use user secrets or environment variables.

## Swagger

Swagger UI is available in development after startup, typically at:

- `http://localhost:5103/swagger`

## Run Locally

From this directory:

```bash
dotnet restore
dotnet run --project DeviceManagement.Api
```

API defaults to `http://localhost:5103` in development (see launch settings).

## Run Tests

```bash
dotnet test DeviceManagement.Tests.Unit/DeviceManagement.Tests.Unit.csproj
dotnet test DeviceManagement.Tests.Integration/DeviceManagement.Tests.Integration.csproj
```

## Notes

- In development, `DatabaseSeeder` runs at startup to populate realistic sample users/devices.
- Configure OpenAI key using user secrets or environment variables (do not commit secrets).
