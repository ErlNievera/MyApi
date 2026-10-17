# Docker Lab Log — MyApi

## Part 0: Environment Setup

- Operating System: Windows
- IDE: Visual Studio 2022
- .NET SDK: 9.0.304
- Docker Engine: 29.7.2
- Docker Compose: 5.4.0
- Docker verification: `hello-world` ran successfully.

## Part 1: Nginx

- Image: `nginx:alpine`
- Configuration file: `nginx.conf`
- Result: Nginx successfully started after correcting the UTF-8 encoding of the configuration file.

## Part 2: Dockerize the API

- Created a multi-stage Dockerfile using the .NET 8 SDK and ASP.NET runtime images.
- Configured the application to listen on port 8080.
- Verification: `GET http://localhost:8080/api/health`
- Observed response: `{"status":"Healthy"}`

## Part 3: Docker Compose and SQL Server

- Services: API and SQL Server.
- Added the named volume `sqlserver_data` for database persistence.
- Configured a SQL Server health check.
- Configured the API connection string to use the Docker service hostname `db`.
- Verification: `GET http://localhost:8080/api/items`
- Observed result: The API returned a database item named `Keyboard`, priced at `799.99`.

## Part 4: Redis, Nginx Reverse Proxy, and API Scaling

### Redis

- Added Redis using the `redis:7-alpine` image.
- Redis health check command: `docker compose exec redis redis-cli ping`
- Observed output: `PONG`
- Redis container status: Healthy.

### Nginx Reverse Proxy

- Added Nginx as a reverse proxy to the API.
- Configuration file: `nginx.conf`
- Exposed Nginx on port 8081.
- Verification: `GET http://localhost:8081/api/health`
- Observed response: `{"status":"Healthy"}`
- Verification: `GET http://localhost:8081/api/items`
- Observed result: The API returned the database-backed `Keyboard` item successfully through Nginx.

### API Scaling

- Scaling command: `docker compose up -d --build --scale api=3`
- Result: Three API replicas started successfully.
- Verified containers: `myapi-api-1`, `myapi-api-2`, and `myapi-api-3`.
- Verification: `GET http://localhost:8081/api/health`
- Observed response: `{"status":"Healthy"}`
- Verification: `GET http://localhost:8081/api/items`
- Observed result: The API returned the `Keyboard` item priced at `799.99`.
- SQL Server status: Healthy.
- Redis status: Healthy.
- Nginx status: Running and forwarding requests successfully with three API replicas configured.

## Reflection

Multi-stage builds separate the build environment from the final runtime image. This reduces unnecessary build tools in the final image and helps keep the deployed application smaller and more focused. Health checks help Docker determine whether a service is ready to respond rather than relying only on whether its container process is running. In this lab, the SQL Server health check helped Docker Compose wait for the database before starting the API. Together, these practices improve deployment reliability and make the container setup easier to maintain.
