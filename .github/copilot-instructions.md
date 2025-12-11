<!-- Purpose: guidance for AI coding agents working on this repository -->
# Copilot / AI agent instructions (concise)

Purpose: Help an AI contributor be immediately productive in this repository (backend-focused).

Big picture
- Backend is an ASP.NET Core Web API in `backend/` using .NET 9 and Entity Framework Core.
- Layers: Controllers (HTTP surface) -> Services (business logic) -> ApplicationDbContext / Entities (EF Core persistence).
- Authentication is done with ASP.NET Identity + JWT (see `Program.cs` and `backend/appsettings*.json`).
- Real-time features use SignalR; CORS is configured for an Angular frontend (`AllowAngularApp`).

Key files to inspect first
- `backend/Program.cs` — DI registrations, auth, CORS, Swagger, SignalR, AutoMapper.
- `backend/Data/ApplicationDbContext.cs` — DbSets and EF model hooks.
- `backend/Data/Entities/` — domain entities (User, Project, Workspace, Task*).
- `backend/DTOs/` — public API shapes used by controllers and services.
- `backend/Services/*` — business logic; services are injected in controllers via constructor DI.
- `backend/Controllers/*` — Web API endpoints; routes use `[Route("api/[controller]")]` and `[ApiController]`.
- `backend/Middleware/ErrorHandelingMiddleware.cs` — global error handling (exceptions should bubble up to this middleware).
- `backend/Data/Migrations/` — EF migrations; use these for DB schema history.

Common patterns and conventions
- Services are thin classes in `backend/Services` and operate against `ApplicationDbContext`. Use dependency injection (constructor). Example: `TaskService`.
- Controllers should accept and return DTOs from `backend/DTOs/*` and keep mapping / transformations in Services or AutoMapper.
- Async-first: use async EF Core calls (`FindAsync`, `SaveChangesAsync`, `ToListAsync`).
- Authorization: controllers are typically decorated with `[Authorize]`; check user id from JWT via `User.FindFirst(ClaimTypes.NameIdentifier)`.
- Error handling: throw exceptions (e.g., `UnauthorizedAccessException`, `KeyNotFoundException`) — `ErrorHandelingMiddleware` will convert them to JSON errors.
- Naming: service classes end with `Service` and are registered in `Program.cs` with `AddScoped<T>()`.

Integration & external dependencies
- DB: configured via `DefaultConnection` — currently `UseSqlite` in `Program.cs` (despite README mentioning SQL Server). Check `backend/appsettings.json` for the connection string.
- Auth: JWT settings live in `backend/appsettings.json` and `appsettings.Development.json` under `JwtSettings`.
- Tools: EF Core migrations are used (`dotnet ef` commands). Ensure `Microsoft.EntityFrameworkCore.Tools` is available when running migrations.

Developer workflows (commands/examples)
- Build: `dotnet restore && dotnet build` (run from repo root or `--project backend`).
- Run backend: `dotnet run --project backend` or `dotnet watch run --project backend` for hot reload.
- Swagger UI (development only): after running, browse `https://localhost:{port}/swagger`.
- Run migrations (from repo root):
  - `dotnet ef migrations add NameHere --project backend --startup-project backend` (create)
  - `dotnet ef database update --project backend --startup-project backend` (apply)
- Debugging: use Visual Studio/VS Code attach or run the `backend` project; logs come from middleware and `ILogger` injection.

What an AI should do when changing code
- Update DTOs in `backend/DTOs` when public API shapes change; update mapping or service return types accordingly.
- When adding new services, register them in `Program.cs` with the same lifetime used across the repo (`AddScoped`).
- If database schema changes, add a migration under `backend/Data/Migrations` using `dotnet ef` and ensure `ApplicationDbContext` has appropriate `DbSet`/configuration.
- Keep controller route patterns and authorization attributes consistent with existing controllers (use `[Route("api/[controller]")]`, `[ApiController]`, and `[Authorize]` where appropriate).
- Prefer throwing exceptions for domain/authorization errors and rely on `ErrorHandelingMiddleware` for uniform responses.

Tests & CI
- There are no unit tests in the repository root; assume manual testing. If adding tests, follow xUnit/typical .NET test patterns and place tests in a parallel `tests/` folder.

Notes / gotchas discovered in code
- `Program.cs` registers `UseSqlite` — README says SQL Server. Verify `appsettings.json` before changing DB provider.
- Middleware file is named `ErrorHandelingMiddleware.cs` (note spelling) but the class is `ErrorHandlingMiddleware` — be careful when renaming to avoid breaking DI.

If uncertain, open these files first: `backend/Program.cs`, `backend/Services/*`, `backend/Controllers/*`, `backend/Data/ApplicationDbContext.cs`, `backend/Middleware/ErrorHandelingMiddleware.cs`.

If you edit this guidance, keep it short and reference the small set of canonical files above.

-- end --
