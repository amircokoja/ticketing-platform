# Ticketing Platform

Learning project for an event ticketing system. Built as part of a
12-week backend curriculum focused on production database skills.

## Stack

- **.NET 10** Web API (`backend/Ticketing.Api`)
- **EF Core 10** with **Npgsql** for Postgres
- **Postgres 17** (Dockerized)
- Console seeder for realistic test data (`backend/Ticketing.Seeder`)

## Repo layout
ticketing-platform/
├── backend/
│   ├── Ticketing.Domain/    # Entities + DbContext (shared)
│   ├── Ticketing.Api/       # Web API
│   └── Ticketing.Seeder/    # Console app that populates the DB
├── frontend/                 # (placeholder, not built yet)
├── sql/                      # Ad-hoc queries for EXPLAIN exercises
└── docker-compose.yml

## Prerequisites

- .NET 10 SDK
- Postgres running on `localhost:5432` (any container or local install)
- A database called `ticketing` (create it manually)

## First-time setup

1. Update the connection string in:
   - `backend/Ticketing.Api/appsettings.Development.json`
   - `backend/Ticketing.Seeder/appsettings.json`

2. Apply migrations:
```bash
   cd backend/Ticketing.Api
   dotnet ef database update --project ../Ticketing.Domain
```

3. Seed the database (~13 seconds, ~170k seats + 92k tickets):
```bash
   cd ../Ticketing.Seeder
   dotnet run
```

4. Run the API:
```bash
   cd ../Ticketing.Api
   dotnet run
```

   OpenAPI spec at `http://localhost:<port>/openapi/v1.json`.

## Reseeding

The seeder is idempotent — it TRUNCATEs all tables and starts fresh.
Just re-run `dotnet run` in `Ticketing.Seeder/`.

## Curriculum notes

This week's focus is the storage layer and query plans. The seeded
data shape is intentional:
- Variable seats per event (1,500-5,000) to create selectivity skew
- Variable sold-rate per event (20-95%) so different events have
  different "available seats" cardinalities
- Three seat statuses (`available`, `held`, `sold`) with realistic
  distribution to make `WHERE status = ?` queries interesting

The "slow query of the week" lives in `sql/01_available_seats_by_event.sql`.