# Movie88 - Database Setup Summary

## ✅ Kết nối Database Supabase thành công!

### 📊 **Entities đã được scaffold từ Supabase:**

#### **Core Business Entities:**
- ✅ `Movie` - Thông tin phim
- ✅ `Cinema` - Rạp chiếu phim
- ✅ `Auditorium` - Phòng chiếu
- ✅ `Seat` - Ghế ngồi
- ✅ `Showtime` - Lịch chiếu phim

#### **User & Authentication:**
- ✅ `User` - Người dùng hệ thống
- ✅ `Role` - Vai trò (Admin, User, etc.)
- ✅ `Customer` - Khách hàng

#### **Booking System:**
- ✅ `Booking` - Đơn đặt vé
- ✅ `Bookingseat` - Ghế đã đặt
- ✅ `Bookingcombo` - Combo đi kèm
- ✅ `Bookingpromotion` - Khuyến mãi áp dụng

#### **Products & Services:**
- ✅ `Combo` - Combo bỏng nước
- ✅ `Promotion` - Chương trình khuyến mãi

#### **Payment:**
- ✅ `Payment` - Giao dịch thanh toán
- ✅ `Paymentmethod` - Phương thức thanh toán

#### **Others:**
- ✅ `Review` - Đánh giá phim

## 🔌 **Connection String:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-1-ap-southeast-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.qnxagekrgvclymbkrkaz;Password=Yeah@17022004;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=10"
  }
}
```

## 📁 **Project Structure:**

```
Movie88.Infrastructure/
├── Context/
│   └── AppDbContext.cs          ✅ Đã kết nối với Supabase
├── Entities/                     ✅ 17 entities từ database
│   ├── Movie.cs
│   ├── Cinema.cs
│   ├── Auditorium.cs
│   ├── Seat.cs
│   ├── Showtime.cs
│   ├── User.cs
│   ├── Role.cs
│   ├── Customer.cs
│   ├── Booking.cs
│   ├── Bookingseat.cs
│   ├── Bookingcombo.cs
│   ├── Bookingpromotion.cs
│   ├── Combo.cs
│   ├── Promotion.cs
│   ├── Payment.cs
│   ├── Paymentmethod.cs
│   └── Review.cs
└── Repositories/
    ├── BaseRepository.cs
    └── UnitOfWork.cs
```

## 🚀 **Sẵn sàng phát triển:**

### **1. Tạo Repositories:**
```csharp
// Example: Movie Repository
public interface IMovieRepository : IBaseRepository<Movie>
{
    Task<List<Movie>> GetMoviesByGenreAsync(string genre);
}

public class MovieRepository : BaseRepository<Movie>, IMovieRepository
{
    public MovieRepository(AppDbContext context) : base(context) { }
    
    public async Task<List<Movie>> GetMoviesByGenreAsync(string genre)
    {
        return await _context.Movies
            .Where(m => m.Genre == genre)
            .ToListAsync();
    }
}
```

### **2. Đăng ký DI:**
```csharp
// Movie88.Infrastructure/ServiceExtensions.cs
services.AddScoped<IMovieRepository, MovieRepository>();
```

### **3. Tạo DTOs:**
```csharp
// Movie88.Application/DTOs/MovieDto.cs
public class MovieDto
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    // ... other properties
}
```

### **4. Tạo Use Cases (CQRS):**
```csharp
// Movie88.Application/UseCases/Movies/GetMovieQuery.cs
public record GetMovieQuery(int MovieId) : IRequest<Response<MovieDto>>;

public class GetMovieHandler : IRequestHandler<GetMovieQuery, Response<MovieDto>>
{
    private readonly IMovieRepository _movieRepository;
    
    public GetMovieHandler(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }
    
    public async Task<Response<MovieDto>> Handle(GetMovieQuery request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetByIdAsync(request.MovieId);
        // Map and return
    }
}
```

### **5. Tạo Controllers:**
```csharp
// Movie88.WebApi/Controllers/MoviesController.cs
[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public MoviesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMovie(int id)
    {
        var result = await _mediator.Send(new GetMovieQuery(id));
        return Ok(result);
    }
}
```

## 📝 **Notes:**

- ✅ Database đã được scaffold thành công
- ✅ Bỏ các bảng system của Supabase (auth, storage, realtime)
- ✅ Chỉ giữ lại các bảng business logic
- ⚠️ Voucher entity đã bị comment (nếu cần thì uncomment)
- ✅ Build thành công, sẵn sàng develop

## 🔧 **Run Application:**

```bash
# Build
dotnet build

# Run API
dotnet run --project Movie88.WebApi

# Access Swagger
https://localhost:xxxx/swagger
```

## 🎬 **Happy Coding! Movie88 is ready!**
