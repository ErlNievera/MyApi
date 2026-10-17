Docker Lab Log — MyApi

Part 0: Environment Setup

Operating System: Windows

IDE: Visual Studio 2022

.NET SDK: 9.0.304

Docker Engine: 29.7.2

Docker Compose: 5.4.0

Docker verification: hello-world ran successfully.

Part 1: Nginx

Image: nginx:alpine

Configuration file: nginx.conf

Result: Nginx successfully started after correcting the UTF-8 encoding of the configuration file.

Part 2: Dockerize the API

Created a multi-stage Dockerfile using the .NET 8 SDK and ASP.NET runtime images.

Configured the application to listen on port 8080.

Verification: GET http://localhost:8080/api/health

Expected response:
{"status":"Healthy"}

Part 3: Docker Compose and SQL Server

Services: API and SQL Server.

Added the named volume sqlserver_data for database persistence.

Configured a SQL Server health check.

Configured the API connection string to use the Docker service hostname db.

Verification: GET http://localhost:8080/api/items

Observed result: The API returned a database item:
Keyboard, priced at 799.99.

Part 4: Redis and Nginx Reverse Proxy

Added Redis using redis:7-alpine.

Redis health check: docker compose exec redis redis-cli ping

Observed output: PONG

Added Nginx as a reverse proxy to the API.

Verification: GET http://localhost:8081/api/health

Observed response:
{"status":"Healthy"}

Verification: GET http://localhost:8081/api/items

Observed result: The API returned the database-backed item successfully through Nginx.

Reflection

Multi-stage builds separate the build environment from the final runtime image. This reduces unnecessary build tools in the final image and helps keep the deployed application smaller and more focused. 
Health checks help Docker determine whether a service is ready to respond, rather than relying only on whether its container process is running. In this lab, 
the SQL Server health check helped Compose wait for the database before starting the API. 
Together, these practices improve deployment reliability and make the container setup easier to maintain.
