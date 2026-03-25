# Weather Forecast API

A .NET 8 Web API built with **Clean Architecture** principles, featuring full CRUD operations and random weather generation. Uses **Mapster** for DTO mapping and **Entity Framework Core** with an in-memory database.

## Architecture

```
src/
├── WeatherApi.Domain          # Entities, interfaces (no dependencies)
├── WeatherApi.Application     # DTOs, services, Mapster mapping config
├── WeatherApi.Infrastructure  # EF Core DbContext, repositories
└── WeatherApi.API             # Controllers, DI wiring, Program.cs
```

### Dependency Flow

```
API → Application → Domain
API → Infrastructure → Application → Domain
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run

```bash
cd src/WeatherApi.API
dotnet run
```

Navigate to `http://localhost:5000` (or the port shown in console) to view the Swagger UI.

## API Endpoints

| Method | Route                                | Description                        |
|--------|--------------------------------------|------------------------------------|
| GET    | `/api/weatherforecast`               | Get all forecasts                  |
| GET    | `/api/weatherforecast/{id}`          | Get a forecast by ID               |
| GET    | `/api/weatherforecast/city/{city}`   | Get forecasts by city              |
| POST   | `/api/weatherforecast`               | Create a new forecast              |
| PUT    | `/api/weatherforecast/{id}`          | Update an existing forecast        |
| DELETE | `/api/weatherforecast/{id}`          | Delete a forecast                  |
| POST   | `/api/weatherforecast/random`        | Generate a random forecast         |
