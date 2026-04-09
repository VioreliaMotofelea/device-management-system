# Database (MS SQL)

This folder contains idempotent SQL scripts for creating and seeding the database required by the backend.

## Files

- `001_create_database.sql`
  - creates `DeviceManagementDb` (if it does not already exist)
  - creates `Users` and `Devices` tables (if they do not already exist)
- `002_seed_data.sql`
  - inserts baseline sample data (if it does not already exist)

## How To Run

Run the scripts in this order using SQL Server Management Studio (SSMS), DataGrip, Azure Data Studio, or `sqlcmd`:

1. Open and execute `001_create_database.sql`.
2. Open and execute `002_seed_data.sql`.

## Notes

- Scripts are idempotent and safe to re-run.
- The backend expects the database name to be `DeviceManagementDb`; point `DefaultConnection` in `backend/DeviceManagement.Api/appsettings.Development.json` at that server.
- The SQL seed script provides baseline idempotent setup data.
- The backend also includes a richer development runtime seeder (`DatabaseSeeder`) for additional realistic local testing data.
