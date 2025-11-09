# Cinema Coordinates Feature

## 📍 Tổng quan

Đã thêm 2 field mới vào Cinema entity để lưu trữ tọa độ GPS:
- `latitude`: Vĩ độ (-90 đến +90)
- `longitude`: Kinh độ (-180 đến +180)

Độ chính xác: 8 chữ số thập phân (~1.1mm precision)

## 🗂️ Các file đã cập nhật

### Backend Code:
1. **Movie88.Infrastructure/Entities/Cinema.cs**
   - Thêm `Latitude` và `Longitude` properties
   - Precision: NUMERIC(10,8) và NUMERIC(11,8)

2. **Movie88.Domain/Models/CinemaModel.cs**
   - Thêm `Latitude` và `Longitude` properties

3. **Movie88.Application/DTOs/Cinemas/CinemaDTO.cs**
   - Thêm `Latitude` và `Longitude` vào response

4. **Movie88.Application/DTOs/Bookings/BookingListDTO.cs**
   - Cập nhật `CinemaDTO` nested class

5. **Movie88.Infrastructure/Context/AppDbContext.cs**
   - Configure precision cho Latitude/Longitude columns

### Database Scripts:
1. **database/migrations/003_add_cinema_coordinates.sql**
   - Migration script để add columns
   - Thêm check constraints cho valid ranges
   - Thêm comments documentation

2. **database/seed-data/03-UPDATE-CINEMA-COORDINATES.sql**
   - Update tọa độ thực tế cho 5 rạp ở TP.HCM
   - Query verify và tính khoảng cách giữa các rạp

## 🚀 Cách deploy lên Supabase

### Bước 1: Chạy Migration Script
```sql
-- Copy toàn bộ nội dung file: 
-- database/migrations/003_add_cinema_coordinates.sql

-- Paste vào Supabase SQL Editor và Execute
```

### Bước 2: Update Sample Data (Optional)
```sql
-- Copy toàn bộ nội dung file:
-- database/seed-data/03-UPDATE-CINEMA-COORDINATES.sql

-- Paste vào Supabase SQL Editor và Execute
```

### Bước 3: Verify
```sql
SELECT 
    cinemaid,
    name,
    latitude,
    longitude
FROM cinemas
ORDER BY cinemaid;
```

## 📊 API Response Examples

### GET /api/cinemas (example)
```json
{
  "data": [
    {
      "cinemaid": 1,
      "name": "Movie 88 - Nguyễn Trãi",
      "address": "123 Nguyễn Trãi, P. Nguyễn Cư Trinh",
      "city": "TP.HCM",
      "latitude": 10.76260000,
      "longitude": 106.68270000,
      "createdat": "2024-01-01T00:00:00Z"
    }
  ]
}
```

### GET /api/bookings/my-bookings (example)
```json
{
  "data": {
    "items": [
      {
        "cinema": {
          "cinemaid": 2,
          "name": "Movie 88 - Sư Vạn Hạnh",
          "address": "10 Sư Vạn Hạnh, P. 12, Quận 10",
          "city": "TP.HCM",
          "latitude": 10.77170000,
          "longitude": 106.66570000
        }
      }
    ]
  }
}
```

## 🗺️ Use Cases

### 1. Hiển thị rạp trên Google Maps (Android)
```kotlin
// Frontend có thể dùng latitude/longitude để:
val location = LatLng(cinema.latitude, cinema.longitude)
googleMap.addMarker(MarkerOptions().position(location).title(cinema.name))
```

### 2. Tính khoảng cách từ vị trí user đến rạp
```kotlin
fun calculateDistance(userLat: Double, userLng: Double, cinemaLat: Double, cinemaLng: Double): Float {
    val results = FloatArray(1)
    Location.distanceBetween(userLat, userLng, cinemaLat, cinemaLng, results)
    return results[0] / 1000 // Convert to kilometers
}
```

### 3. Sắp xếp rạp theo khoảng cách gần nhất
```kotlin
cinemas.sortedBy { cinema ->
    calculateDistance(userLocation.latitude, userLocation.longitude, 
                     cinema.latitude, cinema.longitude)
}
```

## 📝 Notes

- **Nullable Fields**: Latitude và Longitude là nullable để support trường hợp chưa có tọa độ
- **Validation**: Database có check constraints để đảm bảo tọa độ hợp lệ
- **Precision**: 8 decimal places cho độ chính xác ~1.1mm (đủ cho cinema location)
- **AutoMapper**: Tự động map giữa Entity → Model → DTO

## 🔍 Sample Queries

### Tìm rạp gần nhất (PostgreSQL Haversine formula)
```sql
-- Example: Tìm rạp trong bán kính 5km từ vị trí user
SELECT 
    cinemaid,
    name,
    ROUND(
        6371 * acos(
            cos(radians(10.7626)) * cos(radians(latitude)) * 
            cos(radians(longitude) - radians(106.6827)) + 
            sin(radians(10.7626)) * sin(radians(latitude))
        )::numeric, 
    2) as distance_km
FROM cinemas
WHERE latitude IS NOT NULL 
  AND longitude IS NOT NULL
HAVING distance_km <= 5
ORDER BY distance_km;
```

## ✅ Testing Checklist

- [ ] Migration script chạy thành công trên Supabase
- [ ] Sample data được update với tọa độ thực tế
- [ ] GET /api/cinemas trả về latitude/longitude
- [ ] GET /api/bookings/my-bookings có cinema coordinates
- [ ] Frontend hiển thị được rạp trên map
- [ ] Tính khoảng cách user-to-cinema hoạt động đúng
- [ ] Sắp xếp theo khoảng cách hoạt động đúng

## 🎯 Next Steps

1. **Deploy migration script lên Supabase**
2. **Test API endpoints** để verify coordinates được trả về
3. **Update frontend** để hiển thị map với cinema markers
4. **Implement distance calculation** feature
5. **Add "Rạp gần bạn"** filter/sort trong app
