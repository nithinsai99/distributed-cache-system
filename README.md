# High-Performance Distributed Cache

This is a small cache service built with ASP.NET Core, Redis, Serilog, and Docker. The code is split into three parts so it stays easy to follow:

- `Cache.Api` handles HTTP requests.
- `Cache.Core` defines the cache contract.
- `Cache.Infrastructure` talks to Redis.

## What this app does

In simple terms, the API lets you save, read, and delete values by key. Anything you send is stored in Redis as JSON and turned back into an object when you read it again.

The app also includes a few practical extras:

- request throttling so one client cannot spam the API,
- Serilog logging with Seq,
- Docker Compose for running everything locally,
- Redis Commander so you can look at Redis data in a browser.

## Quick start

Run the whole stack with Docker Compose:

```bash
docker-compose up --build
```

When it is up, these are the useful URLs:

- API: http://localhost:5001
- Swagger UI: http://localhost:5001/swagger
- Seq: http://localhost:5341
- Redis Commander: http://localhost:8081

## What runs in Docker Compose

The compose file starts four services:

- `redis` stores the cached data.
- `seq` collects and shows logs.
- `redis-commander` gives you a browser UI for Redis.
- `api` is the ASP.NET Core cache API.

## API endpoints

The cache controller is small on purpose. It exposes these routes:

- `GET /api/cache/{key}` reads a value.
- `POST /api/cache/{key}` stores a value.
- `DELETE /api/cache/{key}` removes a value.

Example POST request:

```bash
curl -X POST http://localhost:5001/api/cache/user:1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Alice","status":"active"}'
```

Example GET request:

```bash
curl http://localhost:5001/api/cache/user:1
```

## How caching works

The `RedisCacheService` stores values in Redis as JSON strings. When the API reads a key, it pulls the string back out and deserializes it into the requested type.

The cache contract lives in [src/Cache.Core/Interfaces/ICacheService.cs](src/Cache.Core/Interfaces/ICacheService.cs).

## Request throttling

`RequestThrottlingMiddleware` keeps the API from being hammered by too many requests from the same IP.

It is set to:

- allow 100 requests,
- count them over a 1 second window,
- return HTTP 429 `Too many requests` once the limit is reached.

To see it in action, run:

```bash
bash tests/hit_rate_limit.sh http://localhost:5001/api/cache/testkey 200
```

If you want some sample Redis data to browse, run:

```bash
bash tests/populate_redis.sh
```

## Logging with Seq

The API uses Serilog for structured logs and sends them to Seq when `Seq__Url` is set.

In Docker Compose, Seq is started with:

- `ACCEPT_EULA=Y`
- `SEQ_FIRSTRUN_ADMINPASSWORD`

That lets the container initialize cleanly and accept log events from the API.

## Redis Commander

Redis Commander is the easiest way to inspect Redis data from the browser.

Open it here:

```text
http://localhost:8081
```

It is handy when you want to:

- check what keys are stored,
- look at JSON values,
- delete test data,
- confirm that POST requests really landed in Redis.

## Files worth knowing about

- `docker-compose.yml` starts Redis, Seq, Redis Commander, and the API.
- `src/Cache.Api/Program.cs` wires up logging, Swagger, Redis, and middleware.
- `src/Cache.Api/Controllers/CacheController.cs` contains the cache endpoints.
- `src/Cache.Api/Middleware/RequestThrottlingMiddleware.cs` handles rate limiting.
- `src/Cache.Infrastructure/Services/RedisCacheService.cs` is the Redis-backed cache implementation.

## Troubleshooting

- If Swagger does not open, make sure the API is running in Development mode.
- If Seq stays empty, check that `Seq__Url` is set and restart the API container.
- If Redis Commander shows nothing, run `bash tests/populate_redis.sh` again and refresh the page.
- If port 5001 is already in use, change the host port mapping in `docker-compose.yml`.

## Running locally without Docker

You can also run the API directly from the source project:

```bash
cd src/Cache.Api
dotnet restore
dotnet run
```

## Test scripts

- `tests/hit_rate_limit.sh` sends many POST requests quickly to trigger the throttle.
- `tests/populate_redis.sh` sends sample POST requests so you can verify data in Redis Commander.

