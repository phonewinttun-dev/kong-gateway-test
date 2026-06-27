# Kong Gateway & ASP.NET Core Integration Test

This repository demonstrates how to integrate the **Kong API Gateway** (configured in DB-less mode) with an **ASP.NET Core 8.0 Web API**. It showcases proxying requests, configuring routes, and applying gateway-level plugins (such as rate limiting).

---

## Architecture Overview

The repository consists of the following components:

1. **Kong API Gateway**:
   - Runs inside a Docker container (configured via `docker-compose.yml`).
   - Uses **DB-less mode** where the configuration is declaratively defined in `kong.yml`.
   - Listens on standard ports:
     - `8000`: HTTP Proxy
     - `8443`: HTTPS Proxy
     - `8001`: Admin API (HTTP)
     - `8444`: Admin API (HTTPS)
   - Exposes three routes proxying to the ASP.NET Core Web API:
     - `/api/book` (applies the **rate-limiting** plugin: maximum 10 requests per minute).
     - `/scalar` (proxies the interactive Scalar API documentation).
     - `/swagger` (proxies the Swagger OpenAPI documentation).

2. **ASP.NET Core Web API (`Kong-Gateway.WebApi`)**:
   - A standard Web API targeting .NET 8.0.
   - Configured to use an **EF Core In-Memory database** (`BookDb`).
   - Pre-populated on startup with mock book data (`DbInitializer.cs`).
   - Hosts a GET endpoint: `/api/Book`.
   - Listens on ports `5014` (HTTP) and `7179` (HTTPS).

3. **Console Application (`Kong-Gateway.ConsoleApp`)**:
   - A standalone CLI tool targeting .NET 8.0.
   - Demonstrates basic interaction with its own EF Core In-Memory database instance to add and list books.

---

## Prerequisites

Before running the project, ensure you have the following installed on your machine:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker engine running)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

---

## Getting Started

### 1. Clone the Repository

Clone the project to your local machine:

```bash
git clone https://github.com/phonewinttun-dev/kong-gateway-test
cd kong-gateway-test
```

### 2. Run the Project

#### Windows (Quick Start)

A convenience batch file `run.bat` is included in the root directory. Running this script will automatically clean up existing Kong containers, restart the Kong Gateway container, and spin up the ASP.NET Core Web API:

```bash
run.bat
```

#### To run manually

1. **Run the Kong Gateway container**:
   ```bash
   docker compose up -d
   ```
2. **Run the ASP.NET Core Web API**:
   ```bash
   dotnet run --project Kong-Gateway.WebApi/Kong-Gateway.WebApi.csproj --launch-profile https
   ```

---

## Testing & Verification

Once both the Kong container and the Web API are running, you can test the setup.

### 1. Direct Web API Endpoints (Bypassing Gateway)

You can call the Web API directly to verify it is running:

- **HTTP API Endpoint**: `http://localhost:5014/api/book`
- **HTTPS API Endpoint**: `https://localhost:7179/api/book`
- **Interactive Scalar UI**: `https://localhost:7179/scalar`
- **Swagger UI**: `https://localhost:7179/swagger`

### 2. Proxied Endpoints (Through Kong Gateway)

Requests made through Kong Gateway will be proxied to the Web API running on the host machine:

- **HTTP Proxy Endpoint**: [http://localhost:8000/api/book](http://localhost:8000/api/book)
- **HTTPS Proxy Endpoint**: [https://localhost:8443/api/book](https://localhost:8443/api/book) (Note: You may need to ignore/bypass SSL warnings in your client since Kong uses self-signed certificates by default).
- **Proxied Scalar UI**: [http://localhost:8000/scalar](http://localhost:8000/scalar)
- **Proxied Swagger UI**: [http://localhost:8000/swagger](http://localhost:8000/swagger)

### 3. Verify Rate Limiting

To test the rate-limiting plugin defined in `kong.yml`:

1. Send requests to the proxied HTTP endpoint: `http://localhost:8000/api/book`.
2. Notice the response headers returned by Kong:
   ```http
   RateLimit-Limit: 10
   RateLimit-Remaining: 9
   RateLimit-Reset: 58
   ```
3. Send more than **10 requests within a single minute**.
4. The gateway will block subsequent requests with an **HTTP 429 Too Many Requests** response and the message:
   ```json
   {
     "message": "API rate limit exceeded"
   }
   ```

### 4. HTTP Client Integration File

An HTTP file (`Kong-Gateway.WebApi.http` in `Kong-Gateway.WebApi/`) is available if your IDE supports running `.http` requests directly. You can use it to easily test all direct and proxied endpoints.

---

## Running the Console Application

The console application runs independently and demonstrates interacting with the book domain database locally in memory:

```bash
dotnet run --project Kong-Gateway.ConsoleApp/Kong-Gateway.ConsoleApp.csproj
```

Upon execution, it will insert a pre-defined list of books into the console app's in-memory database context and print the list to the console.
