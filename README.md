# High-Performance Distributed Cache (ASP.NET Core, Redis, Docker)

This workspace contains a minimal scaffold for a distributed cache microservice using .NET 8, Redis, Serilog, and Docker. The layout follows SOLID principles with separate `Core`, `Infrastructure`, and `Api` projects.

Quick start (requires Docker and .NET 8 SDK):

```bash
# build & run services
docker-compose up --build
```

API is available at http://localhost:5001 (Swagger at /swagger when in Development).

Detailed behavior
-----------------

- Request throttling: The API includes `RequestThrottlingMiddleware` (`src/Cache.Api/Middleware/RequestThrottlingMiddleware.cs`). It limits requests per client IP to 100 requests per 1 second window. When the limit is exceeded the middleware returns HTTP 429 `Too many requests` and does not pass the request to application code.

- Seq: The local logging service uses the `datalust/seq` image. To allow Seq to initialize automatically the compose file sets `ACCEPT_EULA=Y` and `SEQ_FIRSTRUN_ADMINPASSWORD`. The stack brings up `seq` on host port `5341` and the API is configured to send logs to `http://seq:5341` inside Docker.

How to reproduce the rate-limit (quick test)
------------------------------------------

1. Start the stack:

```bash
docker-compose up --build
```

2. The API runs in Development mode and serves Swagger at `http://localhost:5001/swagger`.

3. To trigger the throttling, run the provided script which will rapidly issue many requests from your host IP and report response codes. The middleware counts requests per IP, so running all requests from your machine will hit the limit.

Example (run the test script):

```bash
# send 200 rapid requests to GET /api/cache/testkey
bash tests/hit_rate_limit.sh http://localhost:5001/api/cache/testkey 200
```

The script prints each response code and a short summary of how many `429` responses were returned.

Files added
- `tests/hit_rate_limit.sh`: simple bash script that issues many parallel curl requests to the API endpoint and tallies HTTP status codes.

If you want, I can (A) change the throttling parameters, (B) add a small C# integration test that asserts the 429 behavior, or (C) run the script now and share the results.

Notes:
- `ICacheService` in `src/Cache.Core` defines the cache contract.
- `RedisCacheService` in `src/Cache.Infrastructure` implements Redis-backed caching (StackExchange.Redis).
- `RequestThrottlingMiddleware` in `src/Cache.Api` demonstrates a simple rate limiter.
- CI workflow at `.github/workflows/ci.yml` builds and runs tests on push/PR.

Next steps you can ask me to run:
- Run `dotnet restore`/`dotnet build` locally and fix compilation issues.
- Run tests and a sample request against the running containerized API.
- Harden throttling, add authentication, or implement Redis clustering/high-availability.
