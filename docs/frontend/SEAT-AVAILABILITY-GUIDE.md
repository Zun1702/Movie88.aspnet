# 📘 HƯỚNG DẪN: Hiểu Rõ Trạng Thái Ghế (isAvailable vs isAvailableForShowtime)

**Dành cho**: Frontend Developers (Android Team)  
**Ngày tạo**: November 5, 2025  
**Cập nhật**: November 5, 2025

---

## ⚠️ **QUAN TRỌNG: CÓ 2 LOẠI "isAvailable" KHÁC NHAU!**

Trong hệ thống Movie88, có **2 khái niệm "available"** hoàn toàn khác nhau. Frontend cần hiểu rõ để tránh nhầm lẫn:

---

## 1️⃣ **`seat.isavailable` (Database Field - Bảng seats)**

### **Ý nghĩa**:
Trường này nằm trong **bảng `seats`** của database, dùng để đánh dấu **trạng thái vật lý của ghế**.

### **Mục đích**:
- ✅ **TRUE**: Ghế hoạt động tốt, có thể sử dụng
- ❌ **FALSE**: Ghế bị hỏng/bảo trì, KHÔNG hiển thị cho khách hàng

### **Đặc điểm**:
- **Cố định**: Không thay đổi theo suất chiếu
- **Vĩnh viễn**: Chỉ thay đổi khi Admin đánh dấu ghế hỏng hoặc sửa xong
- **Phạm vi**: Toàn bộ phòng chiếu, áp dụng cho TẤT CẢ suất chiếu

### **Ví dụ thực tế**:
```
Phòng chiếu 1 có sơ đồ ghế:

[A1] [A2] [XX] [A4]  ← Ghế A3 bị hỏng (isavailable = false)
[B1] [B2] [B3] [B4]  ← Tất cả ghế tốt (isavailable = true)

→ Ghế A3 KHÔNG BAO GIỜ hiển thị cho khách hàng
→ Ghế A3 bị loại bỏ ở tất cả suất chiếu (10:00, 14:00, 18:00...)
```

### **Frontend KHÔNG cần quan tâm field này!**
Backend đã tự động lọc ghế hỏng, không trả về trong API response.

---

## 2️⃣ **`IsAvailableForShowtime` (API Response Field - Computed)**

### **Ý nghĩa**:
Field này xuất hiện trong **API response** (JSON), được **tính toán động** bởi backend.

### **Mục đích**:
- ✅ **TRUE**: Ghế CHƯA có người đặt cho **SUẤT CHIẾU CỤ THỂ** này
- ❌ **FALSE**: Ghế ĐÃ có người đặt cho **SUẤT CHIẾU CỤ THỂ** này

### **Đặc điểm**:
- **Động**: Thay đổi theo từng suất chiếu
- **Tạm thời**: Chỉ áp dụng cho 1 suất chiếu cụ thể
- **Phạm vi**: Từng suất chiếu riêng biệt

### **Ví dụ thực tế**:
```
Ghế A5 (seatid=5) trong phòng chiếu 1:

Suất 10:00 - Phim Avatar 3 (showtimeId=123):
  → User A đã đặt
  → API response: isAvailableForShowtime = FALSE 🔴
  → Frontend: Hiển thị ghế màu ĐỎ, không cho click

Suất 14:00 - Phim Avengers 5 (showtimeId=456):
  → Chưa ai đặt
  → API response: isAvailableForShowtime = TRUE 🟢
  → Frontend: Hiển thị ghế màu XANH, cho phép click

Suất 18:00 - Phim Deadpool 3 (showtimeId=789):
  → User B đã đặt
  → API response: isAvailableForShowtime = FALSE 🔴
  → Frontend: Hiển thị ghế màu ĐỎ, không cho click
```

### **Cách Backend tính toán**:
```
Backend Logic (Pseudo-code):

1. Lấy TẤT CẢ ghế tốt từ bảng seats
   → seats = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, ...]

2. Query bảng bookingseats với showtimeId cụ thể
   → bookedSeatIds = [5, 6, 7]  (ghế đã đặt cho suất này)

3. So sánh từng ghế:
   → Ghế 1: isAvailableForShowtime = !(1 in [5,6,7]) = TRUE ✅
   → Ghế 5: isAvailableForShowtime = !(5 in [5,6,7]) = FALSE ❌
   → Ghế 8: isAvailableForShowtime = !(8 in [5,6,7]) = TRUE ✅
```

---

## 📡 **API Endpoint: GET /api/auditoriums/{id}/seats**

### **Request**:
```http
GET /api/auditoriums/1/seats?showtimeId=123
```

⚠️ **QUAN TRỌNG**: PHẢI truyền `showtimeId` để backend biết check ghế nào đã đặt!

### **Response**:
```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "auditoriumid": 1,
    "name": "Phòng chiếu 1",
    "seatscount": 150,
    "seats": [
      {
        "seatid": 1,
        "row": "A",
        "number": 1,
        "seattype": "Standard",
        "isAvailableForShowtime": true  // 🟢 Còn trống cho suất này
      },
      {
        "seatid": 5,
        "row": "A",
        "number": 5,
        "seattype": "VIP",
        "isAvailableForShowtime": false  // 🔴 Đã có người đặt
      },
      {
        "seatid": 8,
        "row": "A",
        "number": 8,
        "seattype": "Standard",
        "isAvailableForShowtime": true  // 🟢 Còn trống cho suất này
      }
      // ⚫ Ghế hỏng (isavailable=false trong DB) KHÔNG trả về
    ]
  }
}
```

---

## 🎨 **Frontend Implementation Guide**

### **Java Code Example**:
```java
// SelectSeatActivity.java

// Model class
public class SeatUIModel {
    private int seatId;
    private String row;
    private int number;
    private String type;
    private SeatState state;
    
    public SeatUIModel(int seatId, String row, int number, String type, SeatState state) {
        this.seatId = seatId;
        this.row = row;
        this.number = number;
        this.type = type;
        this.state = state;
    }
    
    // Getters and Setters
    public int getSeatId() { return seatId; }
    public String getRow() { return row; }
    public int getNumber() { return number; }
    public String getDisplayName() { return row + number; }
    public String getType() { return type; }
    public SeatState getState() { return state; }
    public void setState(SeatState state) { this.state = state; }
}

// Enum for seat states
public enum SeatState {
    AVAILABLE,    // 🟢 Xanh - Có thể đặt
    BOOKED,       // 🔴 Đỏ - Đã có người đặt
    SELECTED,     // 💙 Xanh dương - User đang chọn
    VIP           // 💛 Vàng - Ghế VIP
}

// Parse API response
AuditoriumSeatsResponse response = api.getAuditoriumSeats(
    1,    // auditoriumId
    123   // ⚠️ BẮT BUỘC phải truyền showtimeId
);

// Render seats
List<SeatUIModel> seatUIModels = new ArrayList<>();
for (SeatDTO seat : response.getData().getSeats()) {
    SeatState state;
    
    // ✅ Check field "isAvailableForShowtime" trong response
    if (!seat.isAvailableForShowtime()) {
        state = SeatState.BOOKED;  // Đã đặt
    } else if ("VIP".equalsIgnoreCase(seat.getSeattype())) {
        state = SeatState.VIP;     // VIP
    } else {
        state = SeatState.AVAILABLE;  // Còn trống
    }
    
    seatUIModels.add(new SeatUIModel(
        seat.getSeatid(),
        seat.getRow(),
        seat.getNumber(),
        seat.getSeattype() != null ? seat.getSeattype() : "Standard",
        state
    ));
}

// Handle click event
public void onSeatClicked(SeatUIModel seat) {
    switch (seat.getState()) {
        case AVAILABLE:
        case VIP:
            // ✅ Cho phép chọn ghế
            toggleSeatSelection(seat.getSeatId());
            break;
            
        case BOOKED:
            // ❌ Ghế đã có người đặt
            Toast.makeText(
                this,
                "Ghế " + seat.getDisplayName() + " đã được đặt",
                Toast.LENGTH_SHORT
            ).show();
            break;
            
        case SELECTED:
            // Bỏ chọn ghế
            deselectSeat(seat.getSeatId());
            break;
    }
}
```

---

## 🚨 **LỖI THƯỜNG GẶP**

### **❌ Lỗi 1: Không truyền showtimeId**
```java
// SAI:
api.getAuditoriumSeats(1);  // Thiếu showtimeId

// KẾT QUẢ:
// → Backend không biết check suất chiếu nào
// → TẤT CẢ ghế đều isAvailableForShowtime = true
// → Hiển thị sai! Ghế đã đặt vẫn hiện màu xanh
```

**✅ Cách fix**:
```java
// ĐÚNG:
api.getAuditoriumSeats(
    1,    // auditoriumId
    123   // ⚠️ BẮT BUỘC - showtimeId
);
```

### **❌ Lỗi 2: Parse sai tên field**
```java
// SAI:
boolean isAvailable = seat.isAvailable();  // Method cũ, không tồn tại

// KẾT QUẢ:
// → Crash: Method 'isAvailable()' not found

// ĐÚNG:
boolean isAvailable = seat.isAvailableForShowtime();  // ✅ Method mới
```

### **❌ Lỗi 3: Nhầm lẫn giữa 2 khái niệm**
```java
// SAI - Tư duy:
"Ghế A5 có isAvailable = false trong database 
→ Ghế A5 không thể đặt cho bất kỳ suất nào"

// ĐÚNG - Hiểu đúng:
"Ghế A5 có isavailable = false trong database
→ Ghế A5 bị HỎng, KHÔNG hiển thị trong danh sách
→ Backend đã tự động lọc, Frontend không nhận được ghế này

Ghế A5 có isAvailableForShowtime = false trong API response
→ Ghế A5 ĐÃ ĐẶT cho SUẤT CHIẾU CỤ THỂ này
→ Có thể đặt cho suất chiếu khác"
```

---

## 📊 **So Sánh 2 Loại isAvailable**

| Tiêu chí | `seat.isavailable` (DB) | `isAvailableForShowtime` (API) |
|----------|-------------------------|--------------------------------|
| **Vị trí** | Bảng `seats` trong database | Field trong JSON response |
| **Kiểu dữ liệu** | `BOOLEAN` (nullable) | `boolean` |
| **Ý nghĩa** | Ghế vật lý có hoạt động tốt không? | Ghế có còn trống cho suất này không? |
| **Phạm vi** | Toàn bộ phòng chiếu | Từng suất chiếu cụ thể |
| **Thời gian** | Vĩnh viễn (đến khi Admin sửa) | Tạm thời (theo từng suất) |
| **Ai quản lý** | Admin (đánh dấu ghế hỏng) | Backend (tính toán từ bookingseats) |
| **Frontend có thấy?** | ❌ KHÔNG (Backend đã lọc) | ✅ CÓ (trong API response) |
| **Cách tính** | Lưu trực tiếp trong DB | `!bookedSeatIds.Contains(seatId)` |
| **Mục đích** | Quản lý cơ sở vật chất | Hiển thị trạng thái booking |

---

## 🎯 **Checklist Cho Frontend Developer**

- [ ] Hiểu rõ sự khác biệt giữa 2 loại "isAvailable"
- [ ] **LUÔN LUÔN** truyền `showtimeId` khi gọi API `/api/auditoriums/{id}/seats`
- [ ] Parse đúng field `isAvailableForShowtime` (không phải `isAvailable`)
- [ ] Render màu sắc ghế dựa trên `isAvailableForShowtime`:
  - `true` → 🟢 Xanh (Available)
  - `false` → 🔴 Đỏ (Booked)
- [ ] Chỉ cho phép click ghế có `isAvailableForShowtime = true`
- [ ] Hiển thị thông báo phù hợp khi user click ghế đã đặt
- [ ] Test với nhiều suất chiếu khác nhau (cùng 1 ghế có thể available/booked khác nhau)

---

## 🔧 **Testing Guide**

### **Test Case 1: Ghế available cho suất này**
```
1. Gọi API: GET /api/auditoriums/1/seats?showtimeId=123
2. Tìm ghế có isAvailableForShowtime = true
3. Click ghế → Nên cho phép chọn
4. Màu ghế chuyển sang 💙 (Selected)
```

### **Test Case 2: Ghế đã đặt cho suất này**
```
1. Gọi API: GET /api/auditoriums/1/seats?showtimeId=123
2. Tìm ghế có isAvailableForShowtime = false
3. Click ghế → Hiển thị Toast "Ghế này đã được đặt"
4. Ghế vẫn màu 🔴 (Booked)
```

### **Test Case 3: Cùng 1 ghế, khác suất chiếu**
```
1. Gọi API suất 10:00: GET /api/auditoriums/1/seats?showtimeId=123
   → Ghế A5: isAvailableForShowtime = false (đã đặt)
   
2. Gọi API suất 14:00: GET /api/auditoriums/1/seats?showtimeId=456
   → Ghế A5: isAvailableForShowtime = true (còn trống)
   
3. Verify: Cùng 1 ghế A5 có trạng thái khác nhau ở 2 suất
```

### **Test Case 4: Không truyền showtimeId (Error Case)**
```
1. Gọi API: GET /api/auditoriums/1/seats
   (Không có showtimeId)
   
2. Kết quả: TẤT CẢ ghế đều isAvailableForShowtime = true
3. Bug: Ghế đã đặt vẫn hiển thị xanh
4. Fix: Luôn luôn truyền showtimeId
```

---

## 📞 **Liên Hệ**

Nếu còn thắc mắc về logic ghế, liên hệ:
- **Backend Team**: Trung
- **Docs**: `/docs/screens/04-Booking-Flow.md`
- **API Tests**: `/tests/BookingFlow.http`

---

**Ngày tạo**: November 5, 2025  
**Version**: 1.0  
**Status**: ✅ Active
