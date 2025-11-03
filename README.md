# Movie88 - Clean Architecture Project

## 📋 Overview
Movie88 is a .NET 8 web API project following Clean Architecture principles with CQRS pattern using MediatR.

## 🏗️ Project Structure

```
Movie88.aspnet/
├── Movie88.Domain/              # Core domain entities and interfaces
│   ├── Entities/
│   │   └── EntityBase.cs       # Base entity with common properties
│   └── Interfaces/
│       ├── IBaseRepository.cs  # Generic repository interface
│       └── IUnitOfWork.cs      # Unit of work pattern interface
│
├── Movie88.Application/         # Application business logic
│   ├── Configuration/
│   │   └── ServiceExtensions.cs
│   ├── DTOs/                   # Data Transfer Objects
│   ├── HandlerResponse/
│   │   └── Response.cs         # Generic response wrapper
│   ├── Services/               # Application services
│   ├── Shared/
│   │   └── ValidationBehavior.cs
│   └── UseCases/               # CQRS handlers (Commands/Queries)
│
├── Movie88.Infrastructure/      # Data access and external services
│   ├── Context/
│   │   └── AppDbContext.cs     # EF Core DbContext
│   ├── EntitiesConfiguration/  # EF Core entity configurations
│   ├── Repositories/
│   │   ├── BaseRepository.cs   # Generic repository implementation
│   │   └── UnitOfWork.cs       # Unit of work implementation
│   └── ServiceExtensions.cs    # DI configuration
│
└── Movie88.WebApi/              # API layer
    ├── Controllers/             # API controllers
    ├── Extensions/
    │   └── CorsPolicyExtensions.cs
    └── Program.cs              # Application entry point
```

## 🚀 Technologies Used

- **.NET 8.0**
- **Entity Framework Core 8.0.4** with PostgreSQL
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Swagger/OpenAPI** - API documentation

## 🔧 Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL database

### Configuration

Update connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=Movie88;Username=postgres;Password=yourpassword"
  }
}
```

### Running the Application

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Build the solution:**
   ```bash
   dotnet build
   ```

3. **Run the API:**
   ```bash
   dotnet run --project Movie88.WebApi
   ```

4. **Access Swagger UI:**
   Navigate to `https://localhost:xxxx/swagger`

## 📦 Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName --project Movie88.Infrastructure --startup-project Movie88.WebApi
```

Update database:
```bash
dotnet ef database update --project Movie88.Infrastructure --startup-project Movie88.WebApi
```

## 🏛️ Clean Architecture Layers

### Domain Layer
- Contains core business logic and entities
- No dependencies on other layers
- Defines repository interfaces

### Application Layer
- Contains application business rules
- Implements CQRS with MediatR
- Defines DTOs and application services
- Depends only on Domain layer

### Infrastructure Layer
- Implements data access with EF Core
- Contains repository implementations
- Database migrations
- External service integrations

### Presentation Layer (WebApi)
- RESTful API endpoints
- Request/response handling
- Swagger documentation
- Depends on Application and Infrastructure layers

## 📝 Adding New Features

### 1. Create Entity
Add new entity in `Movie88.Domain/Entities/`:
```csharp
public class YourEntity : EntityBase
{
    // Properties
}
```

### 2. Create Repository Interface
Add interface in `Movie88.Domain/Interfaces/`:
```csharp
public interface IYourRepository : IBaseRepository<YourEntity>
{
    // Custom methods
}
```

### 3. Implement Repository
Add implementation in `Movie88.Infrastructure/Repositories/`:
```csharp
public class YourRepository : BaseRepository<YourEntity>, IYourRepository
{
    // Implementation
}
```

### 4. Configure Entity
Add configuration in `Movie88.Infrastructure/EntitiesConfiguration/`:
```csharp
public class YourEntityConfiguration : IEntityTypeConfiguration<YourEntity>
{
    public void Configure(EntityTypeBuilder<YourEntity> builder)
    {
        // Configuration
    }
}
```

### 5. Register in DI
Update `Movie88.Infrastructure/ServiceExtensions.cs`:
```csharp
services.AddScoped<IYourRepository, YourRepository>();
```

### 6. Add DbSet
Update `Movie88.Infrastructure/Context/AppDbContext.cs`:
```csharp
public DbSet<YourEntity> YourEntities { get; set; }
```

### 7. Create Use Cases
Add commands/queries in `Movie88.Application/UseCases/`:
```csharp
public record CreateYourEntityCommand : IRequest<Response<YourEntityDTO>>;

public class CreateYourEntityHandler : IRequestHandler<CreateYourEntityCommand, Response<YourEntityDTO>>
{
    // Handler implementation
}
```

### 8. Create Controller
Add controller in `Movie88.WebApi/Controllers/`:
```csharp
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public YourController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // Endpoints
}
```

## 🔐 Note
This is a clean template with authentication/authorization removed. Add your own security implementation as needed.

## 📄 License
This project is for educational purposes.
