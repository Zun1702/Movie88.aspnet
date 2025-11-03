# ⭐ Review & Rating API

## 1. Mô tả

Module Review & Rating cho phép khách hàng đánh giá và nhận xét phim sau khi xem, bao gồm:
- Customer viết review và rating (1-5 sao)
- Chỉ cho phép review sau khi đã xem phim (booking status = Paid)
- 1 customer chỉ review 1 lần per movie
- Tính average rating cho phim
- Hiển thị reviews với pagination
- Admin/Manager có thể xóa review vi phạm

## 2. Danh sách Endpoint

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/reviews/movie/{movieId}` | Danh sách review của phim | movieId, page, size | PagedList<ReviewDTO> | Public |
| GET | `/api/reviews/{id}` | Chi tiết review | reviewId | ReviewDTO | Public |
| POST | `/api/reviews` | Tạo review mới | CreateReviewDTO | ReviewDTO | Customer |
| PUT | `/api/reviews/{id}` | Cập nhật review | UpdateReviewDTO | ReviewDTO | Owner |
| DELETE | `/api/reviews/{id}` | Xóa review | reviewId | Success message | Owner/Admin |
| GET | `/api/reviews/my-reviews` | Reviews của tôi | page, size | PagedList<ReviewDTO> | Customer |

## 3. Data Transfer Objects (DTOs)

### 3.1 ReviewDTO
```json
{
  "reviewId": 150,
  "movieId": 10,
  "movieTitle": "Avengers: Endgame",
  "customerId": 25,
  "customerName": "Nguyen Van A",
  "rating": 5,
  "comment": "Phim rất hay, đồ họa đẹp, kịch bản hấp dẫn!",
  "createdAt": "2025-10-15T20:30:00Z",
  "updatedAt": null,
  "isEdited": false
}
```

### 3.2 CreateReviewDTO
```json
{
  "movieId": 10,
  "rating": 5,
  "comment": "Phim rất hay, đồ họa đẹp, kịch bản hấp dẫn!"
}
```

**Validation Rules:**
- `movieId`: Required, must exist
- `rating`: Required, integer 1-5
- `comment`: Required, 10-1000 ký tự
- Customer must have booking with this movie and status = "Paid"
- Customer can only review once per movie

### 3.3 UpdateReviewDTO
```json
{
  "rating": 4,
  "comment": "Cập nhật: Phim hay nhưng hơi dài, 4 sao thôi."
}
```

**Validation Rules:**
- Chỉ owner có thể update
- `rating`: Optional, integer 1-5
- `comment`: Optional, 10-1000 ký tự
- Chỉ update được trong 7 ngày kể từ lúc tạo

### 3.4 MovieRatingDTO
```json
{
  "movieId": 10,
  "totalReviews": 1250,
  "averageRating": 4.3,
  "ratingDistribution": {
    "5star": 600,
    "4star": 400,
    "3star": 150,
    "2star": 70,
    "1star": 30
  }
}
```

## 4. Luồng xử lý (Flow)

### 4.1 Customer Create Review Flow

```
Customer đã xem phim
↓
Vào trang chi tiết phim
↓
Check: Đã xem phim chưa?
GET /api/bookings/my-bookings?movieId=10
↓
Backend check:
SELECT b.BookingId 
FROM Bookings b
JOIN Showtimes s ON b.ShowtimeId = s.ShowtimeId
WHERE b.CustomerId = @CustomerId 
  AND s.MovieId = @MovieId
  AND b.Status = 'Paid'
  AND s.StartTime < GETDATE();  -- Suất chiếu đã qua
↓
IF có booking hợp lệ:
    Check đã review chưa:
    SELECT * FROM Reviews 
    WHERE CustomerId = @CustomerId AND MovieId = @MovieId
    
    IF chưa review:
        Hiển thị form "Viết đánh giá"
    ELSE:
        Hiển thị review đã viết + nút "Sửa đánh giá"
ELSE:
    Không hiển thị form review
↓
User điền form:
- Chọn số sao: 1-5
- Nhập comment: min 10, max 1000 ký tự
↓
POST /api/reviews
Authorization: Bearer {customerToken}
{
  "movieId": 10,
  "rating": 5,
  "comment": "Phim rất hay..."
}
↓
Backend validate:
1. Check token → Extract customerId
2. Validate movieId exists
3. Validate rating 1-5
4. Validate comment length 10-1000
5. Check đã review chưa (duplicate check)
6. Check có booking hợp lệ không
↓
IF all validations pass:
    BEGIN TRANSACTION
    
    -- Insert review
    INSERT INTO Reviews (MovieId, CustomerId, Rating, Comment, CreatedAt)
    VALUES (@MovieId, @CustomerId, @Rating, @Comment, GETDATE());
    
    -- Update movie average rating
    UPDATE Movies
    SET AverageRating = (
        SELECT AVG(CAST(Rating AS FLOAT)) 
        FROM Reviews 
        WHERE MovieId = @MovieId
    )
    WHERE MovieId = @MovieId;
    
    COMMIT TRANSACTION
    
    -- Clear cache
    Cache.Remove("movie:rating:" + movieId)
    Cache.Remove("reviews:movie:" + movieId)
↓
Return ReviewDTO
↓
Frontend hiển thị success + cập nhật rating trên UI
```

### 4.2 Get Movie Reviews with Rating Summary

```
User vào trang phim
↓
GET /api/reviews/movie/10?page=1&size=10
↓
Backend:
1. Lấy average rating và distribution
   SELECT 
       COUNT(*) AS TotalReviews,
       AVG(CAST(Rating AS FLOAT)) AS AverageRating,
       SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) AS Star5,
       SUM(CASE WHEN Rating = 4 THEN 1 ELSE 0 END) AS Star4,
       SUM(CASE WHEN Rating = 3 THEN 1 ELSE 0 END) AS Star3,
       SUM(CASE WHEN Rating = 2 THEN 1 ELSE 0 END) AS Star2,
       SUM(CASE WHEN Rating = 1 THEN 1 ELSE 0 END) AS Star1
   FROM Reviews
   WHERE MovieId = @MovieId;

2. Lấy danh sách reviews (pagination)
   SELECT 
       r.ReviewId,
       r.Rating,
       r.Comment,
       r.CreatedAt,
       r.UpdatedAt,
       c.Name AS CustomerName
   FROM Reviews r
   JOIN Customers c ON r.CustomerId = c.CustomerId
   WHERE r.MovieId = @MovieId
   ORDER BY r.CreatedAt DESC
   OFFSET @Offset ROWS
   FETCH NEXT @PageSize ROWS ONLY;
↓
Return PagedList<ReviewDTO> + MovieRatingDTO
↓
Frontend render:
- Rating summary (4.3★ - 1,250 đánh giá)
- Distribution bars (5★: 600, 4★: 400, ...)
- List reviews with pagination
```

### 4.3 Customer Update Review Flow

```
Customer vào "Đánh giá của tôi"
↓
GET /api/reviews/my-reviews?page=1&size=10
↓
Chọn review muốn sửa
↓
Check: Có được sửa không?
- Chỉ owner có thể sửa
- Chỉ sửa được trong 7 ngày
↓
IF createdAt + 7 days >= now:
    Hiển thị form edit
ELSE:
    "Bạn chỉ có thể sửa đánh giá trong 7 ngày"
↓
User sửa rating hoặc comment
↓
PUT /api/reviews/150
Authorization: Bearer {customerToken}
{
  "rating": 4,
  "comment": "Cập nhật: Phim hay nhưng hơi dài..."
}
↓
Backend validate:
1. Check review exists
2. Check ownership (customerId matches)
3. Check edit window (< 7 days)
4. Validate rating & comment
↓
IF all pass:
    BEGIN TRANSACTION
    
    UPDATE Reviews
    SET Rating = @Rating,
        Comment = @Comment,
        UpdatedAt = GETDATE()
    WHERE ReviewId = @ReviewId;
    
    -- Recalculate movie average rating
    UPDATE Movies
    SET AverageRating = (
        SELECT AVG(CAST(Rating AS FLOAT)) 
        FROM Reviews 
        WHERE MovieId = (SELECT MovieId FROM Reviews WHERE ReviewId = @ReviewId)
    )
    WHERE MovieId = (SELECT MovieId FROM Reviews WHERE ReviewId = @ReviewId);
    
    COMMIT TRANSACTION
    
    -- Clear cache
    Cache.Remove("movie:rating:" + movieId)
    Cache.Remove("reviews:movie:" + movieId)
↓
Return updated ReviewDTO with isEdited = true
```

### 4.4 Admin Delete Inappropriate Review

```
Admin thấy review vi phạm
↓
DELETE /api/reviews/150
Authorization: Bearer {adminToken}
↓
Backend validate:
- Check role (Admin or Manager)
- Check review exists
↓
IF authorized:
    BEGIN TRANSACTION
    
    -- Get movieId trước khi xóa
    DECLARE @MovieId INT = (SELECT MovieId FROM Reviews WHERE ReviewId = @ReviewId);
    
    -- Xóa review
    DELETE FROM Reviews WHERE ReviewId = @ReviewId;
    
    -- Recalculate average rating
    UPDATE Movies
    SET AverageRating = (
        SELECT AVG(CAST(Rating AS FLOAT)) 
        FROM Reviews 
        WHERE MovieId = @MovieId
    )
    WHERE MovieId = @MovieId;
    
    -- Nếu không còn review nào
    IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieId = @MovieId):
        UPDATE Movies SET AverageRating = NULL WHERE MovieId = @MovieId;
    
    COMMIT TRANSACTION
    
    -- Clear cache
    Cache.Remove("movie:rating:" + movieId)
    Cache.Remove("reviews:movie:" + movieId)
    
    -- Gửi email thông báo cho customer (optional)
    Send email: "Đánh giá của bạn đã bị xóa do vi phạm..."
↓
Return success message
```

## 5. Business Rules

### 5.1 Review Eligibility
- Customer phải có ít nhất 1 booking với movie và status = "Paid"
- Showtime của booking đó phải đã qua (StartTime < now)
- Mỗi customer chỉ review 1 lần per movie
- Nếu đã review rồi thì chỉ có thể update, không tạo mới

### 5.2 Edit Rules
- Chỉ owner có thể edit review
- Chỉ edit được trong 7 ngày kể từ lúc tạo
- Sau 7 ngày: review bị "lock", không sửa được nữa
- Mỗi lần edit: set UpdatedAt = now, isEdited = true

### 5.3 Delete Rules
- Owner có thể delete bất kỳ lúc nào
- Admin/Manager có thể delete review vi phạm
- Khi delete: recalculate average rating của movie

### 5.4 Rating Calculation
```
averageRating = ROUND(SUM(rating) / COUNT(*), 1)

Example:
- 5 sao: 600 reviews
- 4 sao: 400 reviews
- 3 sao: 150 reviews
- 2 sao: 70 reviews
- 1 sao: 30 reviews

Total = 1250 reviews
Sum = (5*600) + (4*400) + (3*150) + (2*70) + (1*30)
    = 3000 + 1600 + 450 + 140 + 30
    = 5220

Average = 5220 / 1250 = 4.176 ≈ 4.2 (round to 1 decimal)
```

### 5.5 Display Rules
- Hiển thị rating với 1 chữ số thập phân (4.2★)
- Sort reviews by CreatedAt DESC (mới nhất lên đầu)
- Pagination: 10 reviews per page
- Hiển thị "Đã chỉnh sửa" nếu UpdatedAt != null

## 6. Validation Rules

### CreateReviewDTO Validation
```csharp
public class CreateReviewDTO
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating phải từ 1-5 sao")]
    public int Rating { get; set; }

    [Required]
    [MinLength(10, ErrorMessage = "Comment tối thiểu 10 ký tự")]
    [MaxLength(1000, ErrorMessage = "Comment tối đa 1000 ký tự")]
    public string Comment { get; set; }
}

// Custom validation trong service
public async Task ValidateCreateReview(int customerId, CreateReviewDTO dto)
{
    // 1. Check movie exists
    var movie = await _db.Movies.FindAsync(dto.MovieId);
    if (movie == null)
        throw new NotFoundException("Phim không tồn tại");

    // 2. Check duplicate review
    var existingReview = await _db.Reviews
        .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.MovieId == dto.MovieId);
    if (existingReview != null)
        throw new BadRequestException("Bạn đã đánh giá phim này rồi");

    // 3. Check eligibility (đã xem phim chưa)
    var hasWatchedMovie = await _db.Bookings
        .Include(b => b.Showtime)
        .AnyAsync(b => 
            b.CustomerId == customerId &&
            b.Showtime.MovieId == dto.MovieId &&
            b.Status == "Paid" &&
            b.Showtime.StartTime < DateTime.Now);
    
    if (!hasWatchedMovie)
        throw new BadRequestException("Bạn chưa xem phim này");
}
```

## 7. Error Handling

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 404 | `REVIEW_NOT_FOUND` | "Không tìm thấy đánh giá" | Review không tồn tại |
| 400 | `ALREADY_REVIEWED` | "Bạn đã đánh giá phim này" | Duplicate review |
| 403 | `NOT_ELIGIBLE` | "Bạn chưa xem phim này" | No paid booking |
| 403 | `NOT_OWNER` | "Bạn không thể sửa đánh giá này" | Not review owner |
| 400 | `EDIT_WINDOW_EXPIRED` | "Bạn chỉ có thể sửa trong 7 ngày" | > 7 days since created |
| 400 | `INVALID_RATING` | "Rating phải từ 1-5" | Out of range |
| 400 | `COMMENT_TOO_SHORT` | "Comment tối thiểu 10 ký tự" | < 10 chars |
| 400 | `COMMENT_TOO_LONG` | "Comment tối đa 1000 ký tự" | > 1000 chars |

## 8. Query Optimization

### 8.1 Get Movie Reviews with Customer Info
```sql
-- Lấy reviews của phim với thông tin customer
SELECT 
    r.ReviewId,
    r.Rating,
    r.Comment,
    r.CreatedAt,
    r.UpdatedAt,
    CASE WHEN r.UpdatedAt IS NOT NULL THEN 1 ELSE 0 END AS IsEdited,
    c.Name AS CustomerName
FROM Reviews r
JOIN Customers c ON r.CustomerId = c.CustomerId
WHERE r.MovieId = @MovieId
ORDER BY r.CreatedAt DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;
```

### 8.2 Check Review Eligibility
```sql
-- Check customer đã xem phim chưa
SELECT TOP 1 1
FROM Bookings b
JOIN Showtimes s ON b.ShowtimeId = s.ShowtimeId
WHERE b.CustomerId = @CustomerId
  AND s.MovieId = @MovieId
  AND b.Status = 'Paid'
  AND s.StartTime < GETDATE();
```

### 8.3 Calculate Movie Rating Statistics
```sql
-- Tính rating stats cho phim
SELECT 
    COUNT(*) AS TotalReviews,
    AVG(CAST(Rating AS FLOAT)) AS AverageRating,
    SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) AS Star5,
    SUM(CASE WHEN Rating = 4 THEN 1 ELSE 0 END) AS Star4,
    SUM(CASE WHEN Rating = 3 THEN 1 ELSE 0 END) AS Star3,
    SUM(CASE WHEN Rating = 2 THEN 1 ELSE 0 END) AS Star2,
    SUM(CASE WHEN Rating = 1 THEN 1 ELSE 0 END) AS Star1
FROM Reviews
WHERE MovieId = @MovieId;
```

### 8.4 Get Top Rated Movies
```sql
-- Lấy top 10 phim có rating cao nhất
SELECT TOP 10
    m.MovieId,
    m.Title,
    m.AverageRating,
    COUNT(r.ReviewId) AS TotalReviews
FROM Movies m
LEFT JOIN Reviews r ON m.MovieId = r.MovieId
WHERE m.AverageRating IS NOT NULL
  AND m.Status = 'NowShowing'
GROUP BY m.MovieId, m.Title, m.AverageRating
HAVING COUNT(r.ReviewId) >= 10  -- Ít nhất 10 reviews
ORDER BY m.AverageRating DESC, COUNT(r.ReviewId) DESC;
```

## 9. Caching Strategy

```csharp
// Cache movie rating summary (30 minutes)
Cache: "movie:rating:{movieId}" → MovieRatingDTO

// Cache movie reviews page (15 minutes)
Cache: "reviews:movie:{movieId}:page:{page}" → PagedList<ReviewDTO>

// Cache customer's reviews (10 minutes)
Cache: "reviews:customer:{customerId}" → List<ReviewDTO>

// Clear cache khi có thay đổi
OnReviewCreated/Updated/Deleted:
    Cache.Remove("movie:rating:" + movieId)
    Cache.Remove("reviews:movie:" + movieId + ":*")  // Clear all pages
```

## 10. Database Indexes

```sql
-- Index cho query reviews by movie
CREATE INDEX IX_Reviews_MovieId_CreatedAt 
ON Reviews(MovieId, CreatedAt DESC);

-- Index cho check duplicate review
CREATE UNIQUE INDEX IX_Reviews_CustomerId_MovieId 
ON Reviews(CustomerId, MovieId);

-- Index cho query customer's reviews
CREATE INDEX IX_Reviews_CustomerId_CreatedAt 
ON Reviews(CustomerId, CreatedAt DESC);
```

## 11. Sample API Calls

### Tạo review
```bash
POST /api/reviews
Authorization: Bearer {customerToken}
Content-Type: application/json

{
  "movieId": 10,
  "rating": 5,
  "comment": "Phim rất hay, kịch bản hấp dẫn, diễn xuất tốt. Tôi rất thích!"
}

Response:
{
  "success": true,
  "message": "Cảm ơn bạn đã đánh giá!",
  "data": {
    "reviewId": 150,
    "movieId": 10,
    "rating": 5,
    "comment": "Phim rất hay...",
    "createdAt": "2025-10-29T14:30:00Z"
  }
}
```

### Lấy reviews của phim
```bash
GET /api/reviews/movie/10?page=1&size=10

Response:
{
  "success": true,
  "data": {
    "items": [
      {
        "reviewId": 150,
        "customerName": "Nguyen Van A",
        "rating": 5,
        "comment": "Phim rất hay...",
        "createdAt": "2025-10-29T14:30:00Z",
        "isEdited": false
      }
    ],
    "page": 1,
    "pageSize": 10,
    "totalItems": 1250,
    "totalPages": 125,
    "rating": {
      "averageRating": 4.3,
      "totalReviews": 1250,
      "distribution": {
        "5star": 600,
        "4star": 400,
        "3star": 150,
        "2star": 70,
        "1star": 30
      }
    }
  }
}
```

### Sửa review
```bash
PUT /api/reviews/150
Authorization: Bearer {customerToken}

{
  "rating": 4,
  "comment": "Cập nhật: Phim hay nhưng hơi dài, 4 sao thôi."
}

Response:
{
  "success": true,
  "message": "Đã cập nhật đánh giá",
  "data": {
    "reviewId": 150,
    "rating": 4,
    "comment": "Cập nhật: Phim hay nhưng hơi dài...",
    "updatedAt": "2025-10-30T10:00:00Z",
    "isEdited": true
  }
}
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
