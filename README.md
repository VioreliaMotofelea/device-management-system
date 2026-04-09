# Device Management System

Device inventory system for managing users and devices, including authentication, assignment flows, AI-assisted descriptions, and ranked search.

## Implemented Scope

- Phase 1: DB scripts + backend logic + API for device management, authentication, and secured user-record endpoints
- Phase 2: Angular UI for listing, viewing details, creating, updating, deleting devices
- Phase 3: Authentication + authorization, including device assignment and unassignment
- Phase 4: AI device description generator (OpenAI + template fallback)
- Bonus: ranked free-text search for devices (non-AI)

## Tech Stack

- Backend: C#, ASP.NET Core Web API (.NET 8), EF Core, SQL Server
- Frontend: Angular
- Auth: JWT bearer tokens
- Testing: xUnit unit and integration tests

## Architecture

The backend follows a layered Repository-Service-Controller architecture with separate API, Application, Infrastructure, and Domain projects.

## Repository Structure

- `backend/` - API, application/infrastructure/domain layers, tests
- `frontend/` - Angular application
- `database/` - idempotent SQL scripts for DB setup and baseline seed

## Prerequisites

- .NET SDK 8+
- Node.js + npm
- SQL Server (local instance or Docker)

## Database Setup

Point the API at the same SQL Server where you ran the scripts: edit `ConnectionStrings:DefaultConnection` in `backend/DeviceManagement.Api/appsettings.Development.json`. On **Windows**, use your local instance (for example LocalDB or a named SQL Server instance). On **macOS** (or Linux), SQL Server is often **Docker**—use the host and port you published (commonly `localhost,1433`) and the SQL login you configured for the container.

Option A (scripts):

1. Run `database/001_create_database.sql`
2. Run `database/002_seed_data.sql`

Option B (development seeding):

- In Development, the backend can seed additional sample data at startup.
- SQL scripts remain the recommended baseline setup.

## Run Backend

From `backend/`:

```bash
dotnet restore
dotnet run --project DeviceManagement.Api
```

API default development URL:

- `http://localhost:5103`

Swagger:

- `http://localhost:5103/swagger`

## Run Frontend

From `frontend/device-management-ui`:

```bash
npm install
ng serve
```

Open:

- `http://localhost:4200`

The frontend expects the backend API to be running locally.

## How To Review The Project

1. Set up the database
2. Start backend API from `backend/`
3. Open Swagger at `http://localhost:5103/swagger` and verify API endpoints
4. Start frontend from `frontend/device-management-ui`
5. Open `http://localhost:4200`
6. Register a user or log in and test device flows (list/detail/create/update/delete, assignment/unassignment, AI description generation, search)

## AI Description Configuration

OpenAI settings are under `OpenAi` in API config.

- Leave `ApiKey` empty for template-only mode.
- Use user secrets or environment variables for real keys:
  - `OpenAi__ApiKey`

## Tests

From `backend/`:

```bash
dotnet test DeviceManagement.Tests.Unit/DeviceManagement.Tests.Unit.csproj
dotnet test DeviceManagement.Tests.Integration/DeviceManagement.Tests.Integration.csproj
```

Current backend test status:

- Unit tests: 46 passing
- Integration tests: 28 passing

## Coverage (optional)

```bash
dotnet test DeviceManagement.Tests.Unit/DeviceManagement.Tests.Unit.csproj --collect:"XPlat Code Coverage"
dotnet test DeviceManagement.Tests.Integration/DeviceManagement.Tests.Integration.csproj --collect:"XPlat Code Coverage"
```

## Notes

- `TestResults/` is generated output and should not be committed.
- Project-level details are documented in:
  - `backend/README.md`
  - `frontend/device-management-ui/README.md`
  - `database/README.md`
