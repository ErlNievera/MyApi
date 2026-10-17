# Docker Lab — ASP.NET Core Web API

## Project Overview

This lab containerizes an ASP.NET Core Web API and runs it with SQL Server, Redis, and Nginx using Docker Compose.

### Environment

- Operating System: Windows
- IDE: Visual Studio 2022
- .NET SDK installed: 9.0.304
- Application container: .NET 8 ASP.NET Core
- Docker Engine: 29.7.2
- Docker Compose: v5.4.0

## Part 0 — Environment Setup

Docker and Docker Compose were installed and verified. The Docker `hello-world` test completed successfully.

## Part 1 — Run Nginx

The public Nginx image was used to test container execution and port mapping.

- Image: `nginx:alpine`
- Container name: `web`
- Port mapping: `8081:80`
- Verification: Nginx welcome page displayed in the browser.
- Cleanup: The test container was stopped and removed.

## Part 2 — Dockerize the ASP.NET Core API

A multi-stage Dockerfile was created using the .NET 8 SDK image for building and the .NET 8 ASP.NET Core runtime image for execution.

- Image: `myapi:1.0` was used during the image-build exercise.
- Dockerfile build stage: `mcr.microsoft.com/dotnet/sdk:8.0`
- Runtime stage: `mcr.microsoft.com/dotnet/aspnet:8.0`
- Application port: `8080`
- Health endpoint: `/api/health`

The API health endpoint returned `Healthy` during earlier testing.

The current Compose build creates the image `myapi-api`.

## Part 3 — SQL Server and Persistent Storage

SQL Server was configured as the `db` service.

- Image: `mcr.microsoft.com/mssql/server:2022-latest`
- Database: `MyApiDb`
- Internal connection hostname: `db`
- Host port: `1433`
- Named volume: `sqlserver_data`
- Health check: SQL Server readiness is checked with `sqlcmd`.
- API dependency: The API waits for SQL Server to become healthy.

The API uses the `ConnectionStrings__DefaultConnection` environment variable to connect to SQL Server.

During earlier testing, `/api/items` returned the `Keyboard` item with a price of `799.99`.

**Persistence verification:** Recheck the item after restarting the stack to confirm that the named volume preserves database data. Do not use `docker compose down -v` during this test.

## Part 4 — Redis, Nginx, and API Scaling

The final Compose configuration defines four services:

| Service | Image or build | Purpose |
|---|---|---|
| `nginx` | `nginx:alpine` | Reverse proxy and entry point |
| `api` | Built from `Dockerfile` | ASP.NET Core API |
| `db` | SQL Server 2022 | Persistent database |
| `cache` | `redis:7` | Internal Redis cache service |

### Networking and ports

- Nginx maps host port `80` to container port `80`.
- API containers listen on port `8080` internally and do not publish their own host ports.
- SQL Server publishes port `1433`.
- Redis uses internal port `6379` and does not publish a host port.
- Nginx forwards requests to the `api` service on port `8080`.

### Scaling command

```powershell
docker compose up -d --build --scale api=3 --force-recreate
```

The command completed successfully after removing the old Redis container that conflicted with the renamed `cache` service.

### Latest container status

The latest `docker compose ps` output showed six containers:

- `myapi-api-1` — running
- `myapi-api-2` — running
- `myapi-api-3` — running
- `myapi-db` — healthy
- `myapi-nginx` — running
- `myapi-redis` — healthy, associated with the `cache` service

The `docker compose config --services` command returned:

```text
db
api
nginx
cache
```

The warning about an implicit project name caused by a symlink did not prevent Compose from starting the stack.

### Endpoint verification

Run these commands after the latest recreation to confirm that the application still works:

```powershell
Invoke-RestMethod http://localhost/api/health
Invoke-RestMethod http://localhost/api/items
```

Expected health response:

```json
{
  "status": "Healthy"
}
```

Expected items response: the database-backed `Keyboard` item with a price of `799.99`, if the record remains in the database.

### Remaining verification checklist

- [ ] Confirm health and items endpoints after the latest recreation.
- [ ] Verify database persistence after a normal `docker compose down` and `docker compose up -d`.
- [ ] Record the final runtime image size.
- [ ] Record the Kestrel startup log.
- [ ] Demonstrate requests reaching multiple API replicas.
- [ ] Capture `docker compose ps` and `docker stats` for the final stack.

## Reflection

Using a multi-stage Dockerfile separates the build environment from the runtime image, avoiding the need to include the full SDK in the final application container. Docker Compose makes it easier to run the API, database, cache, and reverse proxy together. Health checks help distinguish a running container from a service that is ready to accept requests. Named volumes provide persistent database storage, while Nginx provides a single entry point for requests to the scaled API replicas.
