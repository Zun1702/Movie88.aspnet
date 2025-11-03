# 🎬 Movie Management API

## 1. Mô tả

Module Movie quản lý toàn bộ thông tin về phim trong hệ thống Movie88, bao gồm:
- Quản lý thông tin phim (CRUD)
- Upload và quản lý poster, trailer
- Phân loại phim theo thể loại, độ tuổi
- Tìm kiếm và lọc phim
- Quản lý trạng thái phim (đang chiếu, sắp chiếu)
- Hiển thị thông tin phim cho khách hàng

## 2. Danh sách Endpoint

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/movies` | Lấy danh sách phim | Query params | List<MovieDTO> | Public |
| GET | `/api/movies/{id}` | Lấy chi tiết phim | movieId | MovieDetailDTO | Public |
| GET | `/api/movies/now-showing` | Phim đang chiếu | Query params | List<MovieDTO> | Public |
| GET | `/api/movies/coming-soon` | Phim sắp chiếu | Query params | List<MovieDTO> | Public |
| GET | `/api/movies/search` | Tìm kiếm phim | Query params | List<MovieDTO> | Public |
| GET | `/api/movies/{id}/showtimes` | Suất chiếu của phim | movieId, date | List<ShowtimeDTO> | Public |
| POST | `/api/movies` | Thêm phim mới | CreateMovieDTO | MovieDTO | Admin/Manager |
| PUT | `/api/movies/{id}` | Cập nhật phim | UpdateMovieDTO | MovieDTO | Admin/Manager |
| DELETE | `/api/movies/{id}` | Xóa phim | movieId | Success message | Admin |
| POST | `/api/movies/{id}/upload-poster` | Upload poster | File | ImageUrlDTO | Admin/Manager |

## 3. Data Transfer Objects (DTOs)

### 3.1 MovieDTO
```json
{
  "movieId": 123,
  "title": "Avengers: Endgame",
  "description": "Sau sự kiện tàn khốc của Infinity War, các siêu anh hùng còn lại tập hợp để đảo ngược hành động của Thanos...",
  "durationMinutes": 181,
  "director": "Anthony Russo, Joe Russo",
  "genre": "Action, Adventure, Sci-Fi",
  "rating": "T13",
  "country": "USA",
  "releaseDate": "2019-04-26",
  "posterUrl": "https://example.com/posters/avengers-endgame.jpg",
  "trailerUrl": "https://youtube.com/watch?v=abc123",
  "averageRating": 4.5,
  "totalReviews": 1250,
  "isNowShowing": true,
  "createdAt": "2019-04-01T00:00:00Z"
}
```

### 3.2 MovieDetailDTO
```json
{
  "movieId": 123,
  "title": "Avengers: Endgame",
  "description": "Sau sự kiện tàn khốc của Infinity War...",
  "durationMinutes": 181,
  "director": "Anthony Russo, Joe Russo",
  "cast": "Robert Downey Jr., Chris Evans, Mark Ruffalo, Chris Hemsworth",
  "genre": "Action, Adventure, Sci-Fi",
  "rating": "T13",
  "country": "USA",
  "releaseDate": "2019-04-26",
  "posterUrl": "https://example.com/posters/avengers-endgame.jpg",
  "trailerUrl": "https://youtube.com/watch?v=abc123",
  "averageRating": 4.5,
  "totalReviews": 1250,
  "isNowShowing": true,
  "upcomingShowtimes": [
    {
      "showtimeId": 567,
      "cinemaName": "CGV Vincom Center",
      "startTime": "2025-10-30T19:30:00Z",
      "price": 80000,
      "format": "2D"
    }
  ],
  "reviews": [
    {
      "reviewId": 1,
      "customerName": "Nguyễn Văn A",
      "rating": 5,
      "comment": "Phim rất hay!",
      "createdAt": "2025-10-29T10:00:00Z"
    }
  ],
  "createdAt": "2019-04-01T00:00:00Z"
}
```

### 3.3 CreateMovieDTO
```json
{
  "title": "Avatar: The Way of Water",
  "description": "Jake Sully sống cùng gia đình mới trên hành tinh Pandora...",
  "durationMinutes": 192,
  "director": "James Cameron",
  "cast": "Sam Worthington, Zoe Saldana, Sigourney Weaver",
  "genre": "Action, Adventure, Fantasy, Sci-Fi",
  "rating": "T13",
  "country": "USA",
  "releaseDate": "2022-12-16",
  "trailerUrl": "https://youtube.com/watch?v=xyz789"
}
```

**Validation Rules:**
- `title`: Required, 1-200 ký tự, unique
- `durationMinutes`: Required, 30-300 phút
- `director`: Required, 1-100 ký tự
- `genre`: Required
- `rating`: Required, giá trị: P, K, T13, T16, T18, C
- `releaseDate`: Required, date format

### 3.4 UpdateMovieDTO
```json
{
  "title": "Avatar: The Way of Water (Updated)",
  "description": "Mô tả mới...",
  "durationMinutes": 192,
  "director": "James Cameron",
  "cast": "Sam Worthington, Zoe Saldana",
  "genre": "Action, Adventure, Fantasy, Sci-Fi",
  "rating": "T13",
  "country": "USA",
  "releaseDate": "2022-12-16",
  "trailerUrl": "https://youtube.com/watch?v=xyz789"
}
```

### 3.5 MovieSearchParams
```json
{
  "keyword": "avatar",
  "genre": "Action",
  "rating": "T13",
  "status": "now-showing",
  "sortBy": "releaseDate",
  "sortOrder": "desc",
  "page": 1,
  "pageSize": 10
}
```

## 4. Luồng xử lý (Flow)

### 4.1 Browse Movies Flow (Public)

```
User vào trang chủ
↓
GET /api/movies/now-showing?page=1&pageSize=12
↓
Backend query:
SELECT * FROM Movies 
WHERE ReleaseDate <= GETDATE()
  AND MovieId IN (
    SELECT DISTINCT MovieId FROM Showtimes 
    WHERE StartTime >= GETDATE()
  )
ORDER BY ReleaseDate DESC
↓
Return List<MovieDTO> với pagination
↓
Frontend hiển thị grid phim với:
- Poster
- Title
- Genre
- Duration
- Rating
- Average rating
```

### 4.2 Movie Detail Flow

```
User click vào phim
↓
GET /api/movies/{movieId}
↓
Backend query:
1. Lấy thông tin phim từ Movies table
2. Tính average rating từ Reviews table
3. Lấy 5 reviews mới nhất
4. Lấy upcoming showtimes (3 ngày tới)
↓
JOIN với:
- Reviews (LEFT JOIN)
- Showtimes (LEFT JOIN)
↓
Return MovieDetailDTO
↓
Frontend hiển thị:
- Hero section với poster & trailer
- Movie info
- Upcoming showtimes
- Reviews
- Book ticket button
```

### 4.3 Search Movies Flow

```
User nhập keyword "avatar" và chọn filter
↓
GET /api/movies/search?keyword=avatar&genre=Action&rating=T13
↓
Backend build dynamic query:
SELECT * FROM Movies
WHERE 1=1
  AND (Title LIKE '%avatar%' OR Description LIKE '%avatar%')
  AND Genre LIKE '%Action%'
  AND Rating = 'T13'
ORDER BY ReleaseDate DESC
↓
Return filtered List<MovieDTO>
↓
Frontend hiển thị kết quả tìm kiếm
```

### 4.4 Admin Create Movie Flow

```
Admin vào trang "Thêm phim mới"
↓
Điền form CreateMovieDTO
↓
POST /api/movies
Authorization: Bearer {adminToken}
↓
Backend validate:
├─ Check title không trùng
├─ Validate all required fields
├─ Validate rating enum
└─ Validate duration range
↓
INSERT INTO Movies (...)
VALUES (...)
↓
Return MovieDTO với movieId mới
↓
Admin tiếp tục upload poster:
POST /api/movies/{movieId}/upload-poster
↓
Upload file lên storage (Azure Blob / AWS S3)
↓
UPDATE Movies SET PosterUrl = {url} WHERE MovieId = {movieId}
↓
Success message
```

### 4.5 Update Movie Flow

```
Admin chỉnh sửa thông tin phim
↓
PUT /api/movies/{movieId}
Authorization: Bearer {adminToken}
↓
Backend validate:
├─ Check movieId exists
├─ Validate updated fields
└─ Check title không trùng (nếu thay đổi)
↓
UPDATE Movies 
SET Title = {newTitle}, 
    Description = {newDesc}, 
    ...
WHERE MovieId = {movieId}
↓
Return updated MovieDTO
```

## 5. Business Rules

### 5.1 Movie Status Rules
- **Now Showing**: Phim có releaseDate <= today VÀ có ít nhất 1 showtime trong tương lai
- **Coming Soon**: Phim có releaseDate > today HOẶC chưa có showtime nào

### 5.2 Rating System (Phân loại độ tuổi)
| Rating | Mô tả | Ý nghĩa |
|--------|-------|---------|
| P | For all ages | Phổ thông - Mọi lứa tuổi |
| K | Under parental guidance | Cần có cha mẹ đi cùng |
| T13 | 13 and above | Từ 13 tuổi trở lên |
| T16 | 16 and above | Từ 16 tuổi trở lên |
| T18 | 18 and above | Từ 18 tuổi trở lên |
| C | Banned | Cấm chiếu |

### 5.3 Genre List (Multi-select)
- Action
- Adventure
- Comedy
- Drama
- Fantasy
- Horror
- Romance
- Sci-Fi
- Thriller
- Animation
- Documentary
- Musical

Format: `"Action, Adventure, Sci-Fi"` (comma-separated)

### 5.4 Deletion Rules
- Không xóa được phim nếu có bookings liên quan
- Soft delete: Set IsDeleted = true thay vì DELETE
- Chỉ Admin mới có quyền xóa

## 6. Validation Rules

### CreateMovieDTO Validation
```csharp
public class CreateMovieDTO
{
    [Required(ErrorMessage = "Tên phim là bắt buộc")]
    [MaxLength(200, ErrorMessage = "Tên phim tối đa 200 ký tự")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Mô tả là bắt buộc")]
    [MaxLength(2000)]
    public string Description { get; set; }

    [Required]
    [Range(30, 300, ErrorMessage = "Thời lượng phải từ 30-300 phút")]
    public int DurationMinutes { get; set; }

    [Required]
    [MaxLength(100)]
    public string Director { get; set; }

    [MaxLength(500)]
    public string Cast { get; set; }

    [Required]
    [MaxLength(255)]
    public string Genre { get; set; }

    [Required]
    [RegularExpression("^(P|K|T13|T16|T18|C)$", 
        ErrorMessage = "Rating không hợp lệ")]
    public string Rating { get; set; }

    [MaxLength(100)]
    public string Country { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Url(ErrorMessage = "URL trailer không hợp lệ")]
    public string TrailerUrl { get; set; }
}
```

## 7. Error Handling

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 400 | `INVALID_DURATION` | "Thời lượng phim không hợp lệ" | Duration < 30 hoặc > 300 |
| 400 | `INVALID_RATING` | "Phân loại độ tuổi không hợp lệ" | Rating không nằm trong enum |
| 404 | `MOVIE_NOT_FOUND` | "Không tìm thấy phim" | MovieId không tồn tại |
| 409 | `MOVIE_TITLE_EXISTS` | "Tên phim đã tồn tại" | Duplicate title |
| 409 | `CANNOT_DELETE_MOVIE` | "Không thể xóa phim có booking" | Movie có bookings |
| 400 | `INVALID_FILE_FORMAT` | "Định dạng file không hợp lệ" | Poster không phải jpg/png |
| 413 | `FILE_TOO_LARGE` | "File quá lớn" | Poster > 5MB |

## 8. Query Optimization

### 8.1 Indexes
```sql
CREATE INDEX idx_movies_title ON Movies(Title);
CREATE INDEX idx_movies_releasedate ON Movies(ReleaseDate DESC);
CREATE INDEX idx_movies_genre ON Movies(Genre);
CREATE INDEX idx_movies_rating ON Movies(Rating);
```

### 8.2 Caching Strategy
```csharp
// Cache now-showing movies (30 minutes)
Cache: "movies:now-showing:page:{page}" → List<MovieDTO>

// Cache movie detail (1 hour)
Cache: "movie:detail:{movieId}" → MovieDetailDTO

// Cache search results (15 minutes)
Cache: "movies:search:{hash}" → List<MovieDTO>
```

### 8.3 Pagination
```csharp
// Default page size = 12
// Max page size = 50
public async Task<PagedResult<MovieDTO>> GetMoviesAsync(
    int page = 1, 
    int pageSize = 12)
{
    if (pageSize > 50) pageSize = 50;
    
    var skip = (page - 1) * pageSize;
    
    var movies = await _context.Movies
        .OrderByDescending(m => m.ReleaseDate)
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync();
    
    var total = await _context.Movies.CountAsync();
    
    return new PagedResult<MovieDTO>
    {
        Data = _mapper.Map<List<MovieDTO>>(movies),
        CurrentPage = page,
        PageSize = pageSize,
        TotalItems = total,
        TotalPages = (int)Math.Ceiling(total / (double)pageSize)
    };
}
```

## 9. File Upload (Poster)

### 9.1 Upload Configuration
```json
{
  "FileUpload": {
    "MaxFileSize": 5242880,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".webp"],
    "StorageType": "AzureBlob",
    "AzureBlobConnectionString": "...",
    "ContainerName": "movie-posters"
  }
}
```

### 9.2 Upload Flow
```csharp
[HttpPost("{id}/upload-poster")]
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> UploadPoster(
    int id, 
    IFormFile file)
{
    // Validate file
    if (file == null || file.Length == 0)
        return BadRequest("File không hợp lệ");
    
    if (file.Length > 5 * 1024 * 1024)
        return BadRequest("File quá lớn (max 5MB)");
    
    var extension = Path.GetExtension(file.FileName).ToLower();
    if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(extension))
        return BadRequest("Định dạng file không hợp lệ");
    
    // Check movie exists
    var movie = await _movieService.GetByIdAsync(id);
    if (movie == null)
        return NotFound();
    
    // Upload to Azure Blob
    var fileName = $"movie_{id}_{Guid.NewGuid()}{extension}";
    var url = await _storageService.UploadAsync(file, fileName);
    
    // Update movie
    movie.PosterUrl = url;
    await _movieService.UpdateAsync(movie);
    
    return Ok(new { posterUrl = url });
}
```

## 10. Sample API Calls

### Lấy phim đang chiếu
```bash
GET /api/movies/now-showing?page=1&pageSize=12

Response:
{
  "success": true,
  "data": [
    {
      "movieId": 123,
      "title": "Avengers: Endgame",
      "posterUrl": "https://...",
      "durationMinutes": 181,
      "rating": "T13",
      "averageRating": 4.5
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 12,
    "totalPages": 5,
    "totalItems": 60
  }
}
```

### Tìm kiếm phim
```bash
GET /api/movies/search?keyword=avatar&genre=Action&rating=T13

Response:
{
  "success": true,
  "data": [
    {
      "movieId": 456,
      "title": "Avatar: The Way of Water",
      ...
    }
  ]
}
```

### Thêm phim mới (Admin)
```bash
POST /api/movies
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "title": "Dune: Part Two",
  "description": "Paul Atreides unites with Chani...",
  "durationMinutes": 166,
  "director": "Denis Villeneuve",
  "cast": "Timothée Chalamet, Zendaya",
  "genre": "Action, Adventure, Drama, Sci-Fi",
  "rating": "T13",
  "country": "USA",
  "releaseDate": "2024-03-01",
  "trailerUrl": "https://youtube.com/watch?v=..."
}

Response:
{
  "success": true,
  "data": {
    "movieId": 789,
    "title": "Dune: Part Two",
    ...
  }
}
```

### Upload poster
```bash
POST /api/movies/789/upload-poster
Authorization: Bearer {adminToken}
Content-Type: multipart/form-data

file: [binary data]

Response:
{
  "success": true,
  "data": {
    "posterUrl": "https://storage.azure.com/.../movie_789_abc.jpg"
  }
}
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
