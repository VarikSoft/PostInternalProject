# InternalProject

InternalProject is a small ASP.NET Core Web API for managing blog-style posts.
The current implementation focuses on a clean layered structure, basic post
lifecycle operations, SQLite persistence, and Swagger documentation.

## Current Implementation

The project currently implements:

- ASP.NET Core Web API with controller-based endpoints.
- Swagger/OpenAPI UI for testing the API in a browser.
- Entity Framework Core with SQLite as the persistence layer.
- Automatic SQLite database creation on application startup.
- Centralized NuGet package version management.
- A simple post domain model with lifecycle states.
- Version-based optimistic concurrency for post mutations.
- Repository interfaces for separating read and write data access.
- Application service layer that contains post use cases.
- Injectable system value provider for deterministic GUID/time usage.
- Global exception handling middleware with JSON error responses and server-side logging.
- Health check endpoints for application and database readiness.
- Console/debug logging configuration for clearer local diagnostics.
- Basic filtering and pagination for the post list endpoint.

## Technology Stack

- .NET 10
- ASP.NET Core
- Entity Framework Core
- SQLite
- Swashbuckle / Swagger

Package versions are managed centrally in `Directory.Packages.props`.
Project files should reference packages without inline `Version` attributes so
the API project and future test projects use the same dependency versions.

## Project Structure

```text
Directory.Packages.props
InternalProject/
  Application/
    ISystemValueProvider.cs
    IPostReadRepository.cs
    IPostWriteRepository.cs
    PostMappingExtensions.cs
    PostService.cs
  Contracts/
    CreatePostRequest.cs
    ErrorResponse.cs
    PostListItemResponse.cs
    PostResponse.cs
    PublishPostRequest.cs
    UpdatePostRequest.cs
  Controllers/
    PostsController.cs
  Data/
    AppDbContext.cs
  Domain/
    ConcurrencyConflictException.cs
    Post.cs
    PostStatus.cs
  Infrastructure/
    EfPostRepository.cs
    HealthChecks/
      DatabaseHealthCheck.cs
      HealthCheckResponseWriter.cs
    SystemValueProvider.cs
    Middleware/
      ExceptionHandlingMiddleware.cs
  Program.cs
  appsettings.json
```

## Domain Model

The main domain entity is `Post`.

A post contains:

- `Id`
- `Title`
- `Body`
- `AuthorId`
- `Status`
- `CreatedAt`
- `PublishedAt`
- `Version`

Supported post statuses:

- `Draft`
- `Published`
- `Unpublished`

## Business Rules

The current domain rules are:

- A post title is required.
- A post body is required.
- A post is created in the `Draft` status.
- Only the author can update, publish, or unpublish a post.
- A published post cannot be edited directly.
- A post cannot be published if its title or body is empty.
- Publishing a post sets `PublishedAt` to the current UTC time.
- Unpublishing a post changes its status to `Unpublished`.
- Update, publish, and unpublish operations require the caller's expected post version.
- If the expected version does not match the current version, the operation is rejected.

## API Endpoints

Base route:

```text
/posts
```

### Create Post

```http
POST /posts
```

Request body:

```json
{
  "title": "Post title",
  "body": "Post body",
  "authorId": "00000000-0000-0000-0000-000000000000"
}
```

Successful response:

```http
201 Created
```

### Get Post By Id

```http
GET /posts/{id}
```

Returns a single post by its identifier.

Possible responses:

- `200 OK`
- `404 Not Found`

### List Posts

```http
GET /posts
```

Supported query parameters:

- `status`
- `authorId`
- `page`
- `pageSize`

Example:

```http
GET /posts?status=Published&page=1&pageSize=20
```

Pagination behavior:

- `page` defaults to `1`.
- `pageSize` defaults to `20`.
- `pageSize` is limited to `100`.

### Update Post

```http
PATCH /posts/{id}
```

Request body:

```json
{
  "title": "Updated title",
  "body": "Updated body",
  "requesterId": "00000000-0000-0000-0000-000000000000",
  "expectedVersion": 1
}
```

Possible responses:

- `200 OK`
- `400 Bad Request`
- `403 Forbidden`
- `404 Not Found`
- `409 Conflict`

### Publish Post

```http
PATCH /posts/{id}/publish
```

Request body:

```json
{
  "requesterId": "00000000-0000-0000-0000-000000000000",
  "expectedVersion": 1
}
```

Possible responses:

- `200 OK`
- `400 Bad Request`
- `403 Forbidden`
- `404 Not Found`
- `409 Conflict`

### Unpublish Post

```http
PATCH /posts/{id}/unpublish
```

Request body:

```json
{
  "requesterId": "00000000-0000-0000-0000-000000000000",
  "expectedVersion": 1
}
```

Possible responses:

- `200 OK`
- `400 Bad Request`
- `403 Forbidden`
- `404 Not Found`
- `409 Conflict`

## Error Handling

The API uses a global exception handling middleware.

Current mappings:

- `ArgumentException` -> `400 Bad Request`
- `InvalidOperationException` -> `400 Bad Request`
- `UnauthorizedAccessException` -> `403 Forbidden`
- `ConcurrencyConflictException` -> `409 Conflict`
- Any other exception -> `500 Internal Server Error`

The middleware logs handled validation, authorization, concurrency, and
unexpected errors internally while returning safe JSON messages to clients.
Logging is configured to write application diagnostics to console and debug
output.

Error response format:

```json
{
  "message": "Error message"
}
```

## Health Checks

The API exposes JSON health check endpoints:

- `GET /health/live` checks whether the application process is running.
- `GET /health/ready` checks whether dependencies required for handling traffic are available.
- `GET /health` runs all configured checks.

Example response:

```json
{
  "status": "Healthy",
  "totalDurationMilliseconds": 39.1584,
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "description": "Application is running.",
      "durationMilliseconds": 0.0212
    },
    {
      "name": "database",
      "status": "Healthy",
      "description": "Database connection is available.",
      "durationMilliseconds": 37.6742
    }
  ]
}
```

## Database

The project uses SQLite.

The default connection string is configured in `InternalProject/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=internal-project.db"
  }
}
```

The database schema is created automatically on startup with
`EnsureCreatedAsync()`. No EF Core migrations are currently implemented.

Local SQLite files are ignored by git:

```text
internal-project.db*
```

## How To Run

From the solution root:

```bash
dotnet restore
dotnet build InternalProject.sln
dotnet run --project InternalProject/InternalProject.csproj
```

To run on a specific URL:

```bash
dotnet run --project InternalProject/InternalProject.csproj --urls http://localhost:5265
```

Swagger UI is available at:

```text
http://localhost:5265/swagger
```

## Example Request

```bash
curl -X POST http://localhost:5265/posts \
  -H "Content-Type: application/json" \
  -d '{
    "title": "First post",
    "body": "Hello from the API",
    "authorId": "11111111-1111-1111-1111-111111111111"
  }'
```

Example response:

```json
{
  "id": "5567dfa5-868a-43c3-8cab-b3f3f202e70a",
  "title": "First post",
  "body": "Hello from the API",
  "authorId": "11111111-1111-1111-1111-111111111111",
  "status": "Draft",
  "createdAt": "2026-05-05T21:17:41.7595799Z",
  "publishedAt": null,
  "version": 1
}
```

## Current Limitations

The following parts are not implemented yet:

- Authentication and authorization.
- User management.
- EF Core migrations.
- Automated tests.
- DTO validation attributes.
- Separate development and production database setup.
- External log aggregation.
