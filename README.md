# EShop Modular Monoliths — Detailed README

This README explains the EShop Modular Monoliths solution in detail: architecture, technologies, module boundaries, HTTP endpoints, messaging, persistence, patterns used (Repository + UnitOfWork + Outbox + Caching), how to run the project locally (with and without Docker), how to run migrations, and where to find the important files in the repository.

The goal is that a developer who opens this README should know exactly what technologies are used, where the code for each concern lives, and how to operate or extend the system without first reading the entire codebase.

---

Table of contents
- Project Summary
- Technologies & Libraries (at-a-glance)
- Architecture Overview
- Module Breakdown (Catalog, Basket, Ordering)
  - Purpose
  - Key types and file locations
  - HTTP endpoints (path, method, purpose, request/response shapes)
- Messaging / Events
- Persistence and Migrations
- Patterns used (Repository, UnitOfWork, Outbox, Cache decorator)
- Security (Keycloak integration) & Authorization
- Logging and Observability (Serilog + Seq)
- Running locally (cmd.exe) and with Docker Compose
- Environment variables and configuration
- Adding a new module / extending the system
- Troubleshooting and common errors
- Where to look in the code (quick map)

---

Project summary
---------------
EShop Modular Monoliths is an ASP.NET Core modular-monolith sample implementing a small ecommerce backend split into feature modules:
- Catalog — products and product catalog features
- Basket — shopping cart (with Redis cache + cached decorator)
- Ordering — orders, order aggregation

Cross-cutting functionality (Shared) contains DDD primitives, Contracts, Messaging events, and DI helpers. The system uses a clean separation between modules and exposes HTTP endpoints via Carter modules.

This repository targets .NET 8 (preview/rc in the local environment) and uses EF Core 9.x for persistence.

Technologies & libraries
------------------------
- Runtime / Framework
  - .NET 8 (net8.0)
  - C# 12 language features
- Web / HTTP
  - ASP.NET Core (minimal APIs / Carter for modular endpoints)
  - Carter (ICarterModule based endpoints)
- Dependency Injection & composition
  - Microsoft.Extensions.DependencyInjection
  - Scrutor (for assembly scanning)
- Persistence / Data
  - Entity Framework Core 9.x (Microsoft.EntityFrameworkCore)
  - Npgsql / Npgsql.EntityFrameworkCore.PostgreSQL (Postgres provider)
- Caching
  - Microsoft.Extensions.Caching.StackExchangeRedis (Redis distributed cache)
- Messaging / Integration
  - MassTransit + RabbitMQ (message broker)
  - Outbox pattern (application-level OutboxMessage entity + OutboxProcessor)
- Logging / Observability
  - Serilog + Seq
- Mapping / DTOs
  - Mapster
- CQRS & Mediation
  - MediatR
- Validation
  - FluentValidation
- Misc
  - Carter.Analyzers, Mapster, Scrutor, etc.

Architecture overview
---------------------
- Modular-monolith structure: each module contains its own domain, data, and HTTP endpoints.
- Shared layer contains cross-cutting libraries: DDD base classes, repository/UoW base types, messaging/event types, and helpers.
- HTTP endpoints use Carter modules which register routes and delegate to MediatR handlers.
- Each module uses its own DbContext where required. EF Core migrations are per-module.
- Repository + UnitOfWork patterns are implemented in `Shared` and applied in each module so handlers and services rely on interfaces (IRepository<T>, IUnitOfWork) instead of DbContext directly.
- Caching: Basket uses a `CachedBasketRepository` that wraps `IBasketRepository`. The decorator updates/invalidate the Redis cache on writes and reads from cache on reads.
- Outbox: Basket uses an Outbox entity + repository to store integration events, processed by `OutboxProcessor` hosted service.

Repo / solution layout (high level)
----------------------------------
Root solution folders (relevant):

- `Bootstrapper/Api` — the ASP.NET Core host app (Program.cs, Swagger, auth wiring)
- `Modules/` — feature modules
  - `Catalog/` (module project: Catalog) — product related features
  - `Basket/` (module project: Basket) — shopping cart + cache + outbox
  - `Ordering/` (module project: Ordering) — orders management
- `Shared/` — libraries for cross-cutting concerns
  - `Shared` (domain primitives, repository base types)
  - `Shared.Contracts` (contracts and CQRS shared types)
  - `Shared.Messaging` (integration events definitions)
- `docker-compose.yml` and `docker-compose.override.yml`

File locations for key concepts
- Catalog module: `Modules/Catalog/Catolog/` (DbContext, repository, endpoints)
- Basket module: `Modules/Basket/Basket/` (DbContext, Cached repository, Outbox, endpoints)
- Ordering module: `Modules/Ordering/Ordering/` (DbContext, repositories, endpoints)
- Shared repository / UoW base types: `Shared/Shared/Data/Repository` and `Shared/Shared/Data/UnitOfWork`
- Integration events: `Shared/Shared.Messaging/Events`

Module details, endpoints and DTOs
---------------------------------
Below is an endpoint-by-endpoint reference for public HTTP routes exposed by each module. All endpoints require Authorization unless noted.

Catalog module (products)
- Base path: root paths are registered by Carter; paths below assume the API root (e.g., `http://localhost:6000/`)

- GET /products
  - Purpose: Get paginated list of products
  - Request: query parameters via `PaginationRequest` (pageIndex, pageSize)
  - Response: PaginatedResult<ProductDto>
  - Code: `Modules/Catalog/Catolog/Products/Features/GetProducts/GetProductsEndpoint.cs`

- GET /products/{id}
  - Purpose: Get a single product by id
  - Request: route param `id` (GUID)
  - Response: ProductDto
  - Code: `.../GetProductById/GetProductByIdEndpoint.cs`

- GET /products/category/{category}
  - Purpose: Get products in a given category (non-paged or large page)
  - Request: route param `category`
  - Response: list of ProductDto
  - Code: `.../GetProductByCategory/GetProductByCategoryEndpoint.cs`

- POST /products
  - Purpose: Create a new product (admin)
  - Request: CreateProductRequest (ProductDto in body)
  - Response: CreateProductResponse (created Id)
  - Code: `.../CreateProduct/CreateProductEndpoint.cs`

- PUT /products
  - Purpose: Update a product
  - Request: UpdateProductRequest (ProductDto)
  - Response: success/fail
  - Code: `.../UpdateProduct/UpdateProductEndpoint.cs`

- DELETE /products/{id}
  - Purpose: Delete a product by id
  - Request: route param `id`
  - Response: DeleteProductResponse
  - Code: `.../DeleteProduct/DeleteProductEndpoint.cs`

Basket module (shopping cart)
- GET /basket/{userName}
  - Purpose: Get shopping basket for a user
  - Auth: Required
  - Request: route param `userName`
  - Response: ShoppingCartDto
  - Code: `Modules/Basket/Basket/Basket/Features/GetBasket/GetBasketEndpoint.cs`

- POST /basket
  - Purpose: Create a new basket (uses authenticated user identity)
  - Auth: Required
  - Request: CreateBasketRequest (ShoppingCartDto) — userName is set from authenticated user
  - Response: CreateBasketResponse (Id)
  - Code: `.../CreateBasket/CreateBasketEndpoint.cs`

- DELETE /basket/{userName}
  - Purpose: Delete a user's basket
  - Auth: Required
  - Response: DeleteBasketResponse
  - Code: `.../DeleteBasket/DeleteBasketEndpoint.cs`

- POST /basket/{userName}/items
  - Purpose: Add an item into a user's basket
  - Auth: Required
  - Request: AddItemIntoBasketRequest (ShoppingCartItemDto)
  - Response: AddItemIntoBasketResponse (Id)
  - Code: `.../AddItemIntoBasket/AddItemIntoBasketEndpoint.cs`

- DELETE /basket/{userName}/items/{productId}
  - Purpose: Remove an item from a user's basket
  - Auth: Required
  - Code: `.../RemoveItemFromBasket/RemoveItemFromBasketEndpoint.cs`

- POST /basket/checkout
  - Purpose: Checkout a basket (produces an integration event via outbox)
  - Auth: Required
  - Request: CheckoutBasketRequest (BasketCheckoutDto)
  - Response: CheckoutBasketResponse (success) — handler writes an OutboxMessage entry and deletes basket
  - Code: `.../CheckoutBasket/CheckoutBasketEndpoint.cs`

Ordering module (orders)
- GET /orders
  - Purpose: List orders (paginated)
  - Code: `Modules/Ordering/Ordering/Orders/Features/GetOrders/GetOrdersEndpoints.cs`

- GET /orders/{id}
  - Purpose: Get order details by id
  - Code: `.../GetOrderById/GetOrderByIdEndpoints.cs`

- POST /orders
  - Purpose: Create a new order
  - Code: `.../CreateOrder/CreateOrderEndpoint.cs`

- DELETE /orders/{id}
  - Purpose: Delete an order
  - Code: `.../DeleteOrder/DeleteOrderEndpoint.cs`

Messaging and integration events
--------------------------------
The solution defines integration event types in `Shared/Shared.Messaging/Events`.

Examples:
- `BasketCheckoutIntegrationEvent`
  - Fields: UserName, CustomerId, TotalPrice, Shipping/Billing address fields, Payment fields
  - Purpose: Sent (via outbox + message bus) when a basket is checked out to be consumed by the Ordering or Payment systems
  - File: `Shared/Shared.Messaging/Events/BasketCheckoutIntegrationEvent.cs`

- `ProductPriceChangeIntegrationEvent`
  - Fields: ProductId, Name, Category, Description, ImageFile, Price
  - Purpose: Broadcast product price or detail changes to other modules (e.g., Basket to update cached prices)
  - File: `Shared/Shared.Messaging/Events/ProductPriceChangeIntegrationEvent.cs`

Outbox pattern
- Basket module persists integration events as `OutboxMessage` rows (entity and table in Basket module) — this avoids immediate publishing, enabling transactional consistency.
- A background `OutboxProcessor` (hosted service) reads unprocessed OutboxMessage entries, publishes them to the message broker (MassTransit/RabbitMQ), and marks them processed.
- Files:
  - `Modules/Basket/Basket/Basket/Modules/OutboxMessage.cs`
  - `Modules/Basket/Basket/Data/Processors/OutboxProcessor.cs`
  - `Modules/Basket/Basket/Data/Repository/OutboxRepository.cs`

Repository + UnitOfWork pattern
-------------------------------
Why: decouple business logic from EF Core, easier to swap ORMs, improved testability.

- Shared abstractions:
  - `Shared/Shared/Data/Repository/IRepository.cs` (generic repository interface)
  - `Shared/Shared/Data/Repository/Repository.cs` (EF Core implementation)
  - `Shared/Shared/Data/UnitOfWork/IUnitOfWork.cs` (transaction & SaveChanges contract)
  - `Shared/Shared/Data/UnitOfWork/UnitOfWork.cs` (EF Core backed implementation)

- Per-module usage: each module exposes a module-specific `I{Module}UnitOfWork` (for example `IBasketUnitOfWork`, `ICatalogUnitOfWork`, `IOrderingUnitOfWork`) and module-specific repositories (e.g., `IProductRepository`). The UnitOfWork exposes repositories as properties and handles SaveChanges/transactions.

- Cached repository (Basket): `CachedBasketRepository` is registered as a decorator over `IBasketRepository` (via `services.Decorate<IBasketRepository, CachedBasketRepository>()`). It intercepts read/write operations to sync Redis cache.

Transactions
- UnitOfWork supports BeginTransactionAsync, CommitTransactionAsync, RollbackTransactionAsync. Checkout flow uses BeginTransaction -> write Outbox message + delete basket -> SaveChanges -> Commit.

Authentication & Authorization
------------------------------
- Keycloak integration used for authentication (OpenID Connect). Helpers are in `Shared` and wired in the `Bootstrapper/Api` via `builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration)`.
- All Carter endpoints use `.RequireAuthorization()` where required. The Create Basket endpoint looks up authenticated user identity to set the username.

Logging & Observability
-----------------------
- Serilog is wired in Program.cs and configured to use Seq for centralized log ingestion.
- Seq is expected to run in Docker in development and receive logs at port 5341 (default in compose).

Swagger / API docs
------------------
- Swagger is enabled only in Development environment. The `Bootstrapper/Api/Extensions/SwaggerExtensions.cs` registers Swagger and the UI and `Program.cs` uses the extension only in development.
- Swagger UI (dev) is available at the app root (RoutePrefix = string.Empty) when running in Development.

Running locally — prerequisites
-------------------------------
- .NET 8 SDK (matching local dev environment — preview/RC may be used)
- Docker Desktop (for running infrastructure: Postgres, Redis, RabbitMQ, Keycloak, Seq)

Run steps (Windows, cmd.exe)
1) Build the solution:

```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src"
dotnet build
```

2) Start infrastructure + API using Docker Compose (development):

```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src"
docker-compose up -d --build
```

This will start containers for Postgres, Redis, RabbitMQ, Keycloak, Seq, and the API (if configured in compose files).

3) Open the API in browser (Development only):
- Swagger UI: http://localhost:5000/ (if the API is configured to publish on port 5000)

4) Stop services:

```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src"
docker-compose down
```

Running locally without Docker (if you already have infra)
- Start Postgres, Redis, and RabbitMQ separately.
- Then run the API:

```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src"
dotnet run --project .\Bootstrapper\Api\Api.csproj
```

Database migrations
- Migrations are generated per module (each DbContext has a Migrations folder). The application also calls `UseMigration<TContext>()` in module wiring to run migrations on startup in dev.
- You can generate and apply EF migrations as usual with `dotnet ef` for a specific project if needed.

Configuration & environment variables
- Primary configuration is in `appsettings.json` and `appsettings.Development.json` in the `Bootstrapper/Api` project.
- Important env vars / configuration keys (examples):
  - ConnectionStrings:Database — Postgres connection string
  - ConnectionStrings:Redis — Redis connection
  - MessageBroker:Host — RabbitMQ host
  - Keycloak related keys (auth server URL, realm, client id)

Docker compose notes
- `docker-compose.yml` + `docker-compose.override.yml` define dev infra and should map ports to host.
- Common compose service names: `api`, `eshopdb` (postgres), `distributedcache` (redis), `messagebus` (rabbitmq), `keycloak`, `seq`.
- If you see errors about `version` attribute warnings in compose files, remove the obsolete `version` attribute to avoid confusion.
- RabbitMQ env vs command: ensure RabbitMQ credentials are set in the `environment` section of the service, not as a `command` line.

Endpoints quick reference
------------------------
Catalog
- GET /products — list products (paginated)
- GET /products/{id} — get product by id
- GET /products/category/{category} — get products in category
- POST /products — create product
- PUT /products — update product
- DELETE /products/{id} — delete product

Basket
- GET /basket/{userName} — get basket for user
- POST /basket — create a basket (user inferred from auth)
- DELETE /basket/{userName} — delete basket for user
- POST /basket/{userName}/items — add item to basket
- DELETE /basket/{userName}/items/{productId} — remove item
- POST /basket/checkout — checkout (outbox event created and basket removed)

Ordering
- GET /orders — list orders
- GET /orders/{id} — get order by id
- POST /orders — create order
- DELETE /orders/{id} — delete order

Request/response DTO locations
- Basket DTOs: `Modules/Basket/Basket/Basket/Dtos/` (ShoppingCartDto, ShoppingCartItemDto, CheckoutBasketDto)
- Catalog DTOs: `Modules/Catalog/Catolog/Products/Dtos/` and contracts under `Modules/Catalog/Catalog.Contracts`
- Ordering DTOs: `Modules/Ordering/Ordering/Orders/Dtos/`

Events & consumers
- Events are defined in `Shared/Shared.Messaging/Events`. Typical events:
  - `BasketCheckoutIntegrationEvent` — published on checkout (via outbox processor)
  - `ProductPriceChangeIntegrationEvent` — published when product price is changed
- Consumers can be wired via MassTransit configuration in `Bootstrapper/Api` (AddMassTransitWithAssemblies) which scans modules.

How to extend the project (add a module)
----------------------------------------
1. Create a new folder under `Modules/MyModule/MyModule`.
2. Add a new .csproj and register it in solution.
3. Implement domain models, DbContext (if persistence needed), repository interface and implementation, and a module-specific UnitOfWork interface if you need transactions.
4. Add Carter endpoints (ICarterModule) in the module and MediatR handlers for business logic.
5. Register the module in `Bootstrapper/Api/Program.cs` with assembly scanning helpers used by existing modules.
6. If the module publishes events, define IntegrationEvent records under `Shared.Shared.Messaging.Events` and make sure to persist via outbox (if producing events within transactions).

Troubleshooting tips
--------------------
- EF Core version warnings: ensure all projects reference the same EF Core package versions (Microsoft.EntityFrameworkCore, Relational, Npgsql provider). Version mismatches cause build or runtime issues.
- RabbitMQ errors about env or command: verify RabbitMQ service `environment` entries in compose; do not use `command` for environment variables.
- `services.Decorate<IBasketRepository, CachedBasketRepository>()` must be registered after the concrete repository registration so DI can wrap the original implementation.
- Swagger not visible: ensure `ASPNETCORE_ENVIRONMENT=Development` when running the app container or locally.

Where to look in the code (quick map)
-------------------------------------
- Host / startup: `Bootstrapper/Api/Program.cs`, `Bootstrapper/Api/Extensions/SwaggerExtensions.cs`
- Shared primitives: `Shared/Shared/*` (Repository, UnitOfWork, Messaging Events)
- Catalog: `Modules/Catalog/Catolog/*`
- Basket: `Modules/Basket/Basket/*`
- Ordering: `Modules/Ordering/Ordering/*`

Useful commands (copy-able)

Build solution
```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src"
dotnet build
```

Run with Docker Compose
```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src"
docker-compose up -d --build
```

Run API locally (no Docker)
```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src"
dotnet run --project .\Bootstrapper\Api\Api.csproj
```

Run Catalog migrations (example)
```cmd
cd /d "D:\My Learn\EshopModularMonoliths\src\Modules\Catalog\Catolog"
dotnet ef migrations add MyMigration --project Catalog.csproj --startup-project ..\..\Bootstrapper\Api\Api.csproj
```

Final notes
-----------
This README is intentionally detailed to give you a comprehensive orientation without immediately opening all the code. If you want, I can also:
- Generate a `DEVELOPER_GUIDE.md` that contains step-by-step developer tasks (how to add a module, code conventions, tests), or
- Produce a `POSTMAN` collection or OpenAPI export for easy testing, or
- Add examples of typical JSON bodies for each endpoint.

Tell me which of the above extras you'd like and I will add them.

---

README generated on: 2025-10-28


