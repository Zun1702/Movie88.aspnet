# 🏗 Backend Architecture - 3-Layer Pattern

## 1. Tổng quan

Hệ thống Movie88 sử dụng **3-Layer Architecture** (Repository - Service - Controller) để đảm bảo:
- ✅ Tách biệt rõ ràng giữa các layer
- ✅ Dễ dàng maintain và test
- ✅ Business logic tập trung tại Service layer
- ✅ Data access tập trung tại Repository layer
- ✅ Controllers chỉ xử lý HTTP requests/responses

## 2. Kiến trúc 3 tầng

```
┌─────────────────────────────────────────────┐
│         PRESENTATION LAYER                  │
│            (Controllers)                    │
│                                             │
│  Trách nhiệm:                              │
│  • Nhận HTTP requests                       │
│  • Validate input DTOs                      │
│  • Call Business Logic Layer                │
│  • Return HTTP responses                    │
│  • Handle authentication/authorization      │
└────────────────┬────────────────────────────┘
                 │
                 │ Inject IService
                 │
                 ↓
┌─────────────────────────────────────────────┐
│        BUSINESS LOGIC LAYER                 │
│             (Services)                      │
│                                             │
│  Trách nhiệm:                              │
│  • Implement business rules                 │
│  • Orchestrate operations                   │
│  • Call Data Access Layer                   │
│  • Transform entities ↔ DTOs                │
│  • Transaction management                   │
│  • Complex validations                      │
└────────────────┬────────────────────────────┘
                 │
                 │ Inject IRepository
                 │
                 ↓
┌─────────────────────────────────────────────┐
│         DATA ACCESS LAYER                   │
│           (Repositories)                    │
│                                             │
│  Trách nhiệm:                              │
│  • CRUD operations                          │
│  • Database queries (EF Core)               │
│  • Data persistence                         │
│  • No business logic                        │
└────────────────┬────────────────────────────┘
                 │
                 │ DbContext
                 │
                 ↓
         ┌───────────────┐
         │  PostgreSQL   │
         │   Database    │
         └───────────────┘
```

## 3. Chi tiết từng Layer

### 3.1 Presentation Layer (Controllers)

**Mục đích**: Xử lý HTTP requests và responses

**Không được phép**:
- ❌ Gọi trực tiếp Database
- ❌ Chứa business logic
- ❌ Tạo entities trực tiếp

**Được phép**:
- ✅ Validate DTOs
- ✅ Call Service methods
- ✅ Handle authentication
- ✅ Return appropriate HTTP status codes

**Example**:
```csharp
[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO dto)
    {
        // 1. Validate DTO (automatically by [ApiController])
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        // 2. Call service layer
        var result = await _bookingService.CreateBookingAsync(dto);
        
        // 3. Return appropriate response
        return CreatedAtAction(
            nameof(GetBooking), 
            new { id = result.BookingId }, 
            new { success = true, data = result }
        );
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        
        if (booking == null)
            return NotFound(new { success = false, message = "Booking not found" });
        
        return Ok(new { success = true, data = booking });
    }
}
```

### 3.2 Business Logic Layer (Services)

**Mục đích**: Chứa toàn bộ business logic và orchestration

**Không được phép**:
- ❌ Truy cập trực tiếp DbContext
- ❌ Xử lý HTTP requests/responses
- ❌ Return entities (phải return DTOs)

**Được phép**:
- ✅ Complex validations
- ✅ Business rules enforcement
- ✅ Call multiple repositories
- ✅ Transaction management
- ✅ Transform entities ↔ DTOs
- ✅ Call external services (payment, email)

**Example**:
```csharp
public interface IBookingService
{
    Task<BookingDTO> CreateBookingAsync(CreateBookingDTO dto);
    Task<BookingDTO> GetBookingByIdAsync(int bookingId);
    Task<bool> CancelBookingAsync(int bookingId, string reason);
}

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public BookingService(
        IBookingRepository bookingRepository,
        IShowtimeRepository showtimeRepository,
        ISeatRepository seatRepository,
        IVoucherRepository voucherRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _showtimeRepository = showtimeRepository;
        _seatRepository = seatRepository;
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO dto)
    {
        // 1. Business validations
        var showtime = await _showtimeRepository.GetByIdAsync(dto.ShowtimeId);
        if (showtime == null)
            throw new NotFoundException("Showtime not found");
        
        if (showtime.StartTime <= DateTime.Now)
            throw new BadRequestException("Cannot book past showtime");
        
        // 2. Check seat availability
        var seats = await _seatRepository.GetByIdsAsync(dto.SeatIds);
        var unavailableSeats = seats.Where(s => !s.IsAvailable).ToList();
        if (unavailableSeats.Any())
            throw new BadRequestException($"Seats {string.Join(", ", unavailableSeats.Select(s => s.SeatNumber))} are not available");
        
        // 3. Calculate pricing
        decimal totalAmount = CalculateTotalAmount(seats, dto.ComboIds);
        
        // 4. Apply voucher if provided
        if (!string.IsNullOrEmpty(dto.VoucherCode))
        {
            var voucher = await _voucherRepository.GetByCodeAsync(dto.VoucherCode);
            if (voucher != null && voucher.IsValid())
            {
                totalAmount = ApplyVoucherDiscount(totalAmount, voucher);
            }
        }
        
        // 5. Create booking entity
        var booking = new Booking
        {
            CustomerId = dto.CustomerId,
            ShowtimeId = dto.ShowtimeId,
            Status = "Pending",
            TotalAmount = totalAmount,
            BookingTime = DateTime.Now,
            ExpiryTime = DateTime.Now.AddMinutes(15)
        };
        
        // 6. Use transaction for multiple operations
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Save booking
            await _bookingRepository.CreateAsync(booking);
            
            // Lock seats (temporarily reserve)
            await _seatRepository.LockSeatsAsync(dto.SeatIds, booking.BookingId);
            
            // Save booking details (seats, combos)
            await SaveBookingDetails(booking.BookingId, dto.SeatIds, dto.ComboIds);
            
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        
        // 7. Transform to DTO
        return MapToDTO(booking);
    }
    
    private decimal CalculateTotalAmount(List<Seat> seats, List<int> comboIds)
    {
        // Business logic for pricing calculation
        decimal total = 0;
        
        foreach (var seat in seats)
        {
            total += seat.Price;
        }
        
        // Add combo prices...
        
        return total;
    }
    
    private BookingDTO MapToDTO(Booking booking)
    {
        return new BookingDTO
        {
            BookingId = booking.BookingId,
            CustomerId = booking.CustomerId,
            ShowtimeId = booking.ShowtimeId,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            BookingTime = booking.BookingTime
        };
    }
}
```

### 3.3 Data Access Layer (Repositories)

**Mục đích**: Truy cập database và CRUD operations

**Không được phép**:
- ❌ Chứa business logic
- ❌ Call external services
- ❌ Complex validations

**Được phép**:
- ✅ CRUD operations
- ✅ Database queries (LINQ, EF Core)
- ✅ Simple data validations
- ✅ Eager loading / lazy loading

**Example**:
```csharp
public interface IBookingRepository
{
    Task<Booking> GetByIdAsync(int bookingId);
    Task<List<Booking>> GetByCustomerIdAsync(int customerId);
    Task<Booking> CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(int bookingId);
    Task<bool> ExistsAsync(int bookingId);
}

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;
    
    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Booking> GetByIdAsync(int bookingId)
    {
        return await _context.Bookings
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Auditorium)
                    .ThenInclude(a => a.Cinema)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Include(b => b.BookingCombos)
                .ThenInclude(bc => bc.Combo)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }
    
    public async Task<List<Booking>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Bookings
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.BookingTime)
            .ToListAsync();
    }
    
    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }
    
    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int bookingId)
    {
        var booking = await GetByIdAsync(bookingId);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> ExistsAsync(int bookingId)
    {
        return await _context.Bookings.AnyAsync(b => b.BookingId == bookingId);
    }
}
```

## 4. Dependency Injection Setup

### Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ICinemaRepository, CinemaRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// External services
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();
```

## 5. Unit of Work Pattern

**Mục đích**: Quản lý transactions cho multiple repositories

```csharp
public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<int> SaveChangesAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _transaction;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
```

## 6. Folder Structure

```
CinemaBookingSystem.API/
├── Controllers/
│   ├── BookingController.cs
│   ├── MovieController.cs
│   ├── PaymentController.cs
│   └── ...
├── Services/
│   ├── Interfaces/
│   │   ├── IBookingService.cs
│   │   ├── IMovieService.cs
│   │   └── ...
│   └── Implementations/
│       ├── BookingService.cs
│       ├── MovieService.cs
│       └── ...
├── Repositories/
│   ├── Interfaces/
│   │   ├── IBookingRepository.cs
│   │   ├── IMovieRepository.cs
│   │   └── ...
│   └── Implementations/
│       ├── BookingRepository.cs
│       ├── MovieRepository.cs
│       └── ...
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Entities/
│   │   ├── Booking.cs
│   │   ├── Movie.cs
│   │   └── ...
│   └── Configurations/
│       ├── BookingConfiguration.cs
│       └── ...
├── DTOs/
│   ├── BookingDTO.cs
│   ├── CreateBookingDTO.cs
│   └── ...
├── Exceptions/
│   ├── NotFoundException.cs
│   ├── BadRequestException.cs
│   └── ...
└── Program.cs
```

## 7. Best Practices

### ✅ DO
1. **Controllers**:
   - Keep thin (no business logic)
   - Return consistent response format
   - Use ActionResults
   - Handle exceptions globally

2. **Services**:
   - Implement business rules
   - Use DTOs for input/output
   - Handle transactions
   - Validate business logic

3. **Repositories**:
   - Focus on data access only
   - Use async/await
   - Implement eager/lazy loading appropriately
   - Keep methods simple and focused

### ❌ DON'T
1. **Controllers**:
   - Don't access DbContext directly
   - Don't put business logic here
   - Don't return entities (use DTOs)

2. **Services**:
   - Don't access DbContext directly
   - Don't handle HTTP concerns
   - Don't return entities to controllers

3. **Repositories**:
   - Don't put business logic here
   - Don't call external services
   - Don't expose DbContext

## 8. Testing Strategy

### Unit Tests
- **Controllers**: Mock IService
- **Services**: Mock IRepository
- **Repositories**: Use In-Memory Database

```csharp
// Example: Testing BookingService
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly Mock<IShowtimeRepository> _mockShowtimeRepo;
    private readonly BookingService _bookingService;
    
    public BookingServiceTests()
    {
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockShowtimeRepo = new Mock<IShowtimeRepository>();
        _bookingService = new BookingService(
            _mockBookingRepo.Object,
            _mockShowtimeRepo.Object,
            // ... other mocked dependencies
        );
    }
    
    [Fact]
    public async Task CreateBooking_WithValidData_ReturnsBookingDTO()
    {
        // Arrange
        var dto = new CreateBookingDTO { /* ... */ };
        _mockShowtimeRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Showtime { /* ... */ });
        
        // Act
        var result = await _bookingService.CreateBookingAsync(dto);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.BookingId > 0);
    }
}
```

---

**Last Updated**: October 29, 2025
**Document Version**: v1.0
