# 🎬 HƯỚNG DẪN: Màn Hình Chọn Ghế (Seat Selection Screen)

**Dành cho**: Frontend Developers (Android Team)  
**Platform**: Android Studio (Java + XML)  
**Screen**: `SelectSeatActivity`  
**Ngày tạo**: November 5, 2025  
**Version**: 2.0 - Flat Pricing

---

## 📋 **MỤC ĐÍCH MÀN HÌNH**

Màn hình này cho phép user:
1. ✅ Xem sơ đồ ghế của phòng chiếu
2. ✅ Chọn ghế muốn đặt (có thể chọn nhiều ghế, tối đa 10)
3. ✅ Thấy rõ ghế nào còn trống, ghế nào đã có người đặt
4. ✅ Xác nhận và tiếp tục sang màn hình chọn combo

**⚠️ LƯU Ý QUAN TRỌNG**: 
- **TẤT CẢ ghế đều có cùng giá** = `showtime.Price`
- Ghế VIP chỉ để UI đẹp (màu vàng), **giá vẫn bằng ghế thường**
- Giá được lấy từ màn trước (màn chọn suất chiếu) và truyền qua Intent

---

## 🎯 **LUỒNG HOẠT ĐỘNG**

```
User chọn suất chiếu (màn trước)
         ↓
    Nhận data qua Intent:
    - showtimeId
    - auditoriumId  
    - ticketPrice (lấy từ showtime.Price)
    - movieTitle, cinemaName, showtimeName
         ↓
    Vào màn chọn ghế
         ↓
[1] Hiển thị loading
         ↓
[2] Gọi API lấy danh sách ghế
    GET /api/auditoriums/{auditoriumId}/seats?showtimeId={showtimeId}
         ↓
[3] Parse response và render sơ đồ ghế
         ↓
[4] User click chọn ghế
         ↓
[5] Validate:
    - Ghế có available không?
    - Đã chọn tối đa 10 ghế chưa?
         ↓
[6] Cập nhật UI:
    - Đổi màu ghế selected
    - Bottom sheet hiển thị ghế đã chọn
    - Tính total = ticketPrice * số ghế
         ↓
[7] User bấm "Tiếp tục"
         ↓
[8] Gọi API tạo booking
    POST /api/bookings/create
         ↓
[9] Chuyển sang màn chọn combo (truyền bookingId)
```

---

## 📡 **API ENDPOINTS**

### **1. GET /api/auditoriums/{auditoriumId}/seats**

**Mục đích**: Lấy danh sách ghế và trạng thái booking cho suất chiếu

#### **Request**:
```http
GET /api/auditoriums/1/seats?showtimeId=42
```

**⚠️ QUAN TRỌNG**: **BẮT BUỘC** phải truyền `showtimeId` để backend biết ghế nào đã đặt cho suất chiếu này.

#### **Response**:
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
        "auditoriumid": 1,
        "row": "A",
        "number": 1,
        "seattype": "Standard",
        "isAvailableForShowtime": true
      },
      {
        "seatid": 5,
        "auditoriumid": 1,
        "row": "A",
        "number": 5,
        "seattype": "VIP",
        "isAvailableForShowtime": false
      }
    ]
  }
}
```

#### **Field Mapping**:

| Field API | Type | Ý nghĩa |
|-----------|------|---------|
| `seatid` | int | ID ghế (dùng khi tạo booking) |
| `row` | string | Hàng ghế (A, B, C, D...) |
| `number` | int | Số ghế trong hàng (1, 2, 3...) |
| `seattype` | string | Loại ghế: `"Standard"`, `"VIP"`, `"Couple"` |
| `isAvailableForShowtime` | boolean | **Ghế còn trống cho suất chiếu này** (computed field) |

**⚠️ Chú ý**: 
- `isAvailableForShowtime` = `true` → Ghế còn trống, cho phép chọn
- `isAvailableForShowtime` = `false` → Ghế đã đặt, không cho chọn
- Field này **KHÔNG phải** từ database, mà được backend tính toán dựa trên `showtimeId`

---

### **2. POST /api/bookings/create**

**Mục đích**: Tạo booking với danh sách ghế đã chọn

#### **Request**:
```http
POST /api/bookings/create
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "showtimeid": 42,
  "seatids": [1, 2, 5, 6]
}
```

#### **Response**:
```json
{
  "success": true,
  "statusCode": 201,
  "data": {
    "bookingid": 156,
    "bookingcode": null,
    "customerid": 3,
    "showtimeid": 42,
    "status": "Pending",
    "totalamount": 400000,
    "seats": [
      {
        "seatid": 1,
        "row": "A",
        "number": 1,
        "type": "Standard",
        "price": 100000
      },
      {
        "seatid": 2,
        "row": "A",
        "number": 2,
        "type": "Standard",
        "price": 100000
      }
    ]
  }
}
```

**Lưu ý**: 
- `bookingcode` = `null` vì chưa thanh toán
- `totalamount` = `ticketPrice * số ghế`
- Booking sẽ tự động cancel sau **15 phút** nếu không thanh toán

---

## 🎨 **UI/UX DESIGN**

### **Layout Structure**:
```
┌─────────────────────────────────────────┐
│ [<]  Chọn Ghế                           │ ← Toolbar
├─────────────────────────────────────────┤
│ 🎬 Avatar 3 - The Way of Water          │
│ 📍 CGV Vincom Center                     │
│ 🕐 10:00 - 2D | 5/11/2025               │
├─────────────────────────────────────────┤
│                                          │
│          [🖥️  MÀN HÌNH  🖥️]             │ ← Screen indicator
│                                          │
│    1  2  3  4  5  6  7  8  9  10        │
│ A [🟢][🟢][🔴][🔴][🟢][🟢][🟢][🟢][🟢][🟢] │
│ B [🟢][💙][💙][🟢][🟢][🟢][🔴][🟢][🟢][🟢] │
│ C [🟢][🟢][🟢][🟢][🟢][🟢][🟢][🟢][🟢][🟢] │
│ D [💛][💛][💛][💛][💛][💛][💛][💛][💛][💛] │ ← VIP row (giá = thường)
│ E [🟢][🟢][🟢][🟢][🟢][🟢][🟢][🟢][🟢][🟢] │
│                                          │
│ ┌──────────────────────────────────┐   │
│ │ 🟢 Còn trống  🔴 Đã đặt           │   │ ← Legend
│ │ 💙 Đang chọn  💛 VIP              │   │
│ └──────────────────────────────────┘   │
│                                          │
├─────────────────────────────────────────┤
│ 💺 Ghế đã chọn: B2, B3                  │ ← Bottom Sheet
│ 💰 Tổng tiền: 200,000đ                  │
│ [      Tiếp tục (2 ghế)      ]          │ ← Button
└─────────────────────────────────────────┘
```

### **Màu Sắc Ghế**:

| Icon | Trạng thái | Màu | Mã màu | Điều kiện |
|------|------------|-----|--------|-----------|
| 🟢 | Available | Xanh lá | `#4CAF50` | `isAvailableForShowtime == true && seattype != "VIP"` |
| 💛 | VIP Available | Vàng | `#FFC107` | `isAvailableForShowtime == true && seattype == "VIP"` |
| 🔴 | Booked | Đỏ | `#F44336` | `isAvailableForShowtime == false` |
| 💙 | Selected | Xanh dương | `#2196F3` | User đã chọn (state local) |

**⚠️ Chú ý**: Ghế VIP chỉ khác màu UI (vàng thay vì xanh lá), **giá vẫn bằng ghế thường**.

---

## 💻 **IMPLEMENTATION CODE**

### **1. Data Models (Java POJOs)**

```java
// API Response Models
public class AuditoriumSeatsResponse {
    private boolean success;
    private int statusCode;
    private AuditoriumSeatsData data;
    
    // Getters and Setters
    public boolean isSuccess() { return success; }
    public void setSuccess(boolean success) { this.success = success; }
    public int getStatusCode() { return statusCode; }
    public void setStatusCode(int statusCode) { this.statusCode = statusCode; }
    public AuditoriumSeatsData getData() { return data; }
    public void setData(AuditoriumSeatsData data) { this.data = data; }
}

public class AuditoriumSeatsData {
    private int auditoriumid;
    private String name;
    private int seatscount;
    private List<SeatDTO> seats;
    
    // Getters and Setters
    public int getAuditoriumid() { return auditoriumid; }
    public void setAuditoriumid(int auditoriumid) { this.auditoriumid = auditoriumid; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public int getSeatscount() { return seatscount; }
    public void setSeatscount(int seatscount) { this.seatscount = seatscount; }
    public List<SeatDTO> getSeats() { return seats; }
    public void setSeats(List<SeatDTO> seats) { this.seats = seats; }
}

public class SeatDTO {
    private int seatid;
    private int auditoriumid;
    private String row;
    private int number;
    private String seattype;  // "Standard", "VIP", "Couple"
    private boolean isAvailableForShowtime;  // ⚠️ Field computed từ backend
    
    // Getters and Setters
    public int getSeatid() { return seatid; }
    public void setSeatid(int seatid) { this.seatid = seatid; }
    public int getAuditoriumid() { return auditoriumid; }
    public void setAuditoriumid(int auditoriumid) { this.auditoriumid = auditoriumid; }
    public String getRow() { return row; }
    public void setRow(String row) { this.row = row; }
    public int getNumber() { return number; }
    public void setNumber(int number) { this.number = number; }
    public String getSeattype() { return seattype; }
    public void setSeattype(String seattype) { this.seattype = seattype; }
    public boolean isAvailableForShowtime() { return isAvailableForShowtime; }
    public void setAvailableForShowtime(boolean availableForShowtime) { 
        isAvailableForShowtime = availableForShowtime; 
    }
}

// UI Model (dùng trong Activity/Adapter)
public class SeatUIModel {
    private int seatId;
    private String row;
    private int number;
    private String displayName;  // "A1", "B5"
    private String seatType;     // "Standard", "VIP", "Couple"
    private SeatState state;     // AVAILABLE, BOOKED, SELECTED
    private double price;        // ⚠️ Luôn bằng ticketPrice (flat pricing)
    
    public SeatUIModel(int seatId, String row, int number, String seatType, 
                       SeatState state, double price) {
        this.seatId = seatId;
        this.row = row;
        this.number = number;
        this.displayName = row + number;
        this.seatType = seatType;
        this.state = state;
        this.price = price;
    }
    
    // Getters and Setters
    public int getSeatId() { return seatId; }
    public String getRow() { return row; }
    public int getNumber() { return number; }
    public String getDisplayName() { return displayName; }
    public String getSeatType() { return seatType; }
    public SeatState getState() { return state; }
    public void setState(SeatState state) { this.state = state; }
    public double getPrice() { return price; }
}

// Enums
public enum SeatState {
    AVAILABLE,   // 🟢 Còn trống
    BOOKED,      // 🔴 Đã đặt
    SELECTED     // 💙 Đang chọn
}

// Request Model
public class CreateBookingRequest {
    private int showtimeid;
    private List<Integer> seatids;
    
    public CreateBookingRequest(int showtimeid, List<Integer> seatids) {
        this.showtimeid = showtimeid;
        this.seatids = seatids;
    }
    
    public int getShowtimeid() { return showtimeid; }
    public void setShowtimeid(int showtimeid) { this.showtimeid = showtimeid; }
    public List<Integer> getSeatids() { return seatids; }
    public void setSeatids(List<Integer> seatids) { this.seatids = seatids; }
}
```

---

### **2. Retrofit API Service**

```java
public interface BookingApiService {
    @GET("/api/auditoriums/{auditoriumId}/seats")
    Call<AuditoriumSeatsResponse> getAuditoriumSeats(
        @Path("auditoriumId") int auditoriumId,
        @Query("showtimeId") int showtimeId  // ⚠️ BẮT BUỘC
    );
    
    @POST("/api/bookings/create")
    Call<CreateBookingResponse> createBooking(
        @Header("Authorization") String token,
        @Body CreateBookingRequest request
    );
}
```

---

### **3. Activity Logic (Core)**

```java
public class SelectSeatActivity extends AppCompatActivity {
    
    // Views
    private RecyclerView rvSeats;
    private ProgressBar progressBar;
    private TextView tvSelectedSeats;
    private TextView tvTotalPrice;
    private MaterialButton btnContinue;
    
    // Data from Intent
    private int showtimeId;
    private int auditoriumId;
    private double ticketPrice;  // ⚠️ Từ showtime.Price (flat pricing)
    private String movieTitle;
    private String cinemaName;
    private String showtimeName;
    
    // State
    private List<SeatUIModel> allSeats = new ArrayList<>();
    private List<SeatUIModel> selectedSeats = new ArrayList<>();
    private SeatAdapter seatAdapter;
    private BookingApiService apiService;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_seat);
        
        initViews();
        getIntentData();
        setupUI();
        loadSeats();
    }
    
    private void getIntentData() {
        Intent intent = getIntent();
        showtimeId = intent.getIntExtra("SHOWTIME_ID", 0);
        auditoriumId = intent.getIntExtra("AUDITORIUM_ID", 0);
        ticketPrice = intent.getDoubleExtra("TICKET_PRICE", 0.0);  // ⚠️ Quan trọng
        movieTitle = intent.getStringExtra("MOVIE_TITLE");
        cinemaName = intent.getStringExtra("CINEMA_NAME");
        showtimeName = intent.getStringExtra("SHOWTIME_NAME");
    }
    
    private void loadSeats() {
        progressBar.setVisibility(View.VISIBLE);
        
        // ⚠️ PHẢI truyền showtimeId
        apiService.getAuditoriumSeats(auditoriumId, showtimeId)
            .enqueue(new Callback<AuditoriumSeatsResponse>() {
                @Override
                public void onResponse(Call<AuditoriumSeatsResponse> call, 
                                     Response<AuditoriumSeatsResponse> response) {
                    progressBar.setVisibility(View.GONE);
                    
                    if (response.isSuccessful() && response.body() != null) {
                        AuditoriumSeatsResponse data = response.body();
                        if (data.isSuccess()) {
                            processSeats(data.getData().getSeats());
                        } else {
                            showError("Không thể tải danh sách ghế");
                        }
                    } else {
                        showError("Lỗi server: " + response.code());
                    }
                }
                
                @Override
                public void onFailure(Call<AuditoriumSeatsResponse> call, Throwable t) {
                    progressBar.setVisibility(View.GONE);
                    showError("Lỗi kết nối: " + t.getMessage());
                }
            });
    }
    
    private void processSeats(List<SeatDTO> seats) {
        allSeats.clear();
        
        for (SeatDTO seat : seats) {
            // Xác định state dựa trên isAvailableForShowtime
            SeatState state;
            if (!seat.isAvailableForShowtime()) {
                state = SeatState.BOOKED;  // Đã đặt
            } else {
                state = SeatState.AVAILABLE;  // Còn trống
            }
            
            // ⚠️ FLAT PRICING: Tất cả ghế đều cùng giá
            SeatUIModel uiModel = new SeatUIModel(
                seat.getSeatid(),
                seat.getRow(),
                seat.getNumber(),
                seat.getSeattype(),
                state,
                ticketPrice  // ← Giá đồng nhất
            );
            
            allSeats.add(uiModel);
        }
        
        displaySeats();
    }
    
    private void displaySeats() {
        // Group seats by row (A, B, C...)
        Map<String, List<SeatUIModel>> seatsByRow = new HashMap<>();
        for (SeatUIModel seat : allSeats) {
            String row = seat.getRow();
            if (!seatsByRow.containsKey(row)) {
                seatsByRow.put(row, new ArrayList<>());
            }
            seatsByRow.get(row).add(seat);
        }
        
        // Sort rows and seats
        List<String> sortedRows = new ArrayList<>(seatsByRow.keySet());
        Collections.sort(sortedRows);
        
        List<Object> adapterItems = new ArrayList<>();
        for (String row : sortedRows) {
            adapterItems.add(row);  // Row header
            
            List<SeatUIModel> rowSeats = seatsByRow.get(row);
            rowSeats.sort((s1, s2) -> Integer.compare(s1.getNumber(), s2.getNumber()));
            adapterItems.addAll(rowSeats);
        }
        
        seatAdapter.submitList(adapterItems);
    }
    
    private void handleSeatClick(SeatUIModel seat) {
        switch (seat.getState()) {
            case AVAILABLE:
                // Check limit
                if (selectedSeats.size() >= 10) {
                    showError("Chỉ được chọn tối đa 10 ghế");
                    return;
                }
                
                // Add to selected
                seat.setState(SeatState.SELECTED);
                selectedSeats.add(seat);
                break;
                
            case SELECTED:
                // Remove from selected
                seat.setState(SeatState.AVAILABLE);
                selectedSeats.remove(seat);
                break;
                
            case BOOKED:
                showError("Ghế " + seat.getDisplayName() + " đã có người đặt");
                return;
        }
        
        seatAdapter.notifyDataSetChanged();
        updateBottomSheet();
    }
    
    private void updateBottomSheet() {
        if (selectedSeats.isEmpty()) {
            tvSelectedSeats.setText("Chưa chọn ghế");
            tvTotalPrice.setText("0đ");
            btnContinue.setEnabled(false);
        } else {
            // Display selected seats
            StringBuilder seatNames = new StringBuilder();
            for (int i = 0; i < selectedSeats.size(); i++) {
                seatNames.append(selectedSeats.get(i).getDisplayName());
                if (i < selectedSeats.size() - 1) {
                    seatNames.append(", ");
                }
            }
            tvSelectedSeats.setText(seatNames.toString());
            
            // ⚠️ FLAT PRICING: total = ticketPrice * số ghế
            double total = ticketPrice * selectedSeats.size();
            tvTotalPrice.setText(formatCurrency(total));
            
            btnContinue.setEnabled(true);
            btnContinue.setText("Tiếp tục (" + selectedSeats.size() + " ghế)");
        }
    }
    
    private void createBooking() {
        if (selectedSeats.isEmpty()) {
            showError("Vui lòng chọn ít nhất 1 ghế");
            return;
        }
        
        progressBar.setVisibility(View.VISIBLE);
        btnContinue.setEnabled(false);
        
        // Prepare seat IDs
        List<Integer> seatIds = new ArrayList<>();
        for (SeatUIModel seat : selectedSeats) {
            seatIds.add(seat.getSeatId());
        }
        
        CreateBookingRequest request = new CreateBookingRequest(showtimeId, seatIds);
        
        apiService.createBooking("Bearer " + getAccessToken(), request)
            .enqueue(new Callback<CreateBookingResponse>() {
                @Override
                public void onResponse(Call<CreateBookingResponse> call,
                                     Response<CreateBookingResponse> response) {
                    progressBar.setVisibility(View.GONE);
                    btnContinue.setEnabled(true);
                    
                    if (response.isSuccessful() && response.body() != null) {
                        CreateBookingResponse data = response.body();
                        if (data.isSuccess()) {
                            navigateToComboSelection(data.getData().getBookingid());
                        } else {
                            showError("Không thể tạo booking");
                        }
                    } else {
                        showError("Lỗi server: " + response.code());
                    }
                }
                
                @Override
                public void onFailure(Call<CreateBookingResponse> call, Throwable t) {
                    progressBar.setVisibility(View.GONE);
                    btnContinue.setEnabled(true);
                    showError("Lỗi kết nối: " + t.getMessage());
                }
            });
    }
    
    private void navigateToComboSelection(int bookingId) {
        Intent intent = new Intent(this, SelectComboActivity.class);
        intent.putExtra("BOOKING_ID", bookingId);
        intent.putExtra("MOVIE_TITLE", movieTitle);
        intent.putExtra("CINEMA_NAME", cinemaName);
        intent.putExtra("SHOWTIME_NAME", showtimeName);
        startActivity(intent);
        finish();
    }
    
    private String formatCurrency(double amount) {
        NumberFormat formatter = NumberFormat.getCurrencyInstance(new Locale("vi", "VN"));
        return formatter.format(amount);
    }
    
    private void showError(String message) {
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show();
    }
    
    private String getAccessToken() {
        // Get from SharedPreferences
        SharedPreferences prefs = getSharedPreferences("MyApp", MODE_PRIVATE);
        return prefs.getString("access_token", "");
    }
}
```

---

### **4. SeatAdapter (RecyclerView)**

```java
public class SeatAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    
    private static final int TYPE_HEADER = 0;
    private static final int TYPE_SEAT = 1;
    
    private Context context;
    private List<Object> items = new ArrayList<>();
    private OnSeatClickListener listener;
    
    public interface OnSeatClickListener {
        void onSeatClick(SeatUIModel seat);
    }
    
    public SeatAdapter(Context context, OnSeatClickListener listener) {
        this.context = context;
        this.listener = listener;
    }
    
    public void submitList(List<Object> newItems) {
        items.clear();
        items.addAll(newItems);
        notifyDataSetChanged();
    }
    
    @Override
    public int getItemViewType(int position) {
        return (items.get(position) instanceof String) ? TYPE_HEADER : TYPE_SEAT;
    }
    
    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        LayoutInflater inflater = LayoutInflater.from(context);
        
        if (viewType == TYPE_HEADER) {
            View view = inflater.inflate(R.layout.item_seat_row_header, parent, false);
            return new HeaderViewHolder(view);
        } else {
            View view = inflater.inflate(R.layout.item_seat, parent, false);
            return new SeatViewHolder(view);
        }
    }
    
    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        Object item = items.get(position);
        
        if (holder instanceof HeaderViewHolder) {
            ((HeaderViewHolder) holder).bind((String) item);
        } else if (holder instanceof SeatViewHolder) {
            ((SeatViewHolder) holder).bind((SeatUIModel) item, listener);
        }
    }
    
    @Override
    public int getItemCount() {
        return items.size();
    }
    
    // Header ViewHolder (Row: A, B, C...)
    static class HeaderViewHolder extends RecyclerView.ViewHolder {
        TextView tvRow;
        
        public HeaderViewHolder(@NonNull View itemView) {
            super(itemView);
            tvRow = itemView.findViewById(R.id.tv_row);
        }
        
        public void bind(String row) {
            tvRow.setText(row);
        }
    }
    
    // Seat ViewHolder
    static class SeatViewHolder extends RecyclerView.ViewHolder {
        View ivSeat;
        TextView tvSeatNumber;
        
        public SeatViewHolder(@NonNull View itemView) {
            super(itemView);
            ivSeat = itemView.findViewById(R.id.iv_seat);
            tvSeatNumber = itemView.findViewById(R.id.tv_seat_number);
        }
        
        public void bind(SeatUIModel seat, OnSeatClickListener listener) {
            tvSeatNumber.setText(String.valueOf(seat.getNumber()));
            
            // Set background based on state and type
            int backgroundRes;
            boolean isClickable = true;
            
            switch (seat.getState()) {
                case AVAILABLE:
                    // VIP chỉ khác màu vàng, giá vẫn bằng thường
                    if ("VIP".equalsIgnoreCase(seat.getSeatType())) {
                        backgroundRes = R.drawable.bg_seat_vip;  // 💛 Vàng
                    } else {
                        backgroundRes = R.drawable.bg_seat_available;  // 🟢 Xanh lá
                    }
                    break;
                    
                case SELECTED:
                    backgroundRes = R.drawable.bg_seat_selected;  // 💙 Xanh dương
                    break;
                    
                case BOOKED:
                    backgroundRes = R.drawable.bg_seat_booked;  // 🔴 Đỏ
                    isClickable = false;
                    break;
                    
                default:
                    backgroundRes = R.drawable.bg_seat_available;
                    break;
            }
            
            ivSeat.setBackgroundResource(backgroundRes);
            
            // Set click listener
            if (isClickable) {
                itemView.setOnClickListener(v -> {
                    if (listener != null) {
                        listener.onSeatClick(seat);
                    }
                });
                itemView.setAlpha(1.0f);
            } else {
                itemView.setOnClickListener(null);
                itemView.setAlpha(0.5f);
            }
        }
    }
}
```

---

### **5. XML Layouts (Simplified)**

#### **activity_select_seat.xml**

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.coordinatorlayout.widget.CoordinatorLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent">

    <androidx.core.widget.NestedScrollView
        android:layout_width="match_parent"
        android:layout_height="match_parent">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical">

            <!-- Toolbar -->
            <com.google.android.material.appbar.MaterialToolbar
                android:id="@+id/toolbar"
                android:layout_width="match_parent"
                android:layout_height="?attr/actionBarSize"
                app:navigationIcon="@drawable/ic_back"
                app:title="Chọn Ghế" />

            <!-- Movie Info -->
            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="vertical"
                android:padding="16dp">

                <TextView
                    android:id="@+id/tv_movie_title"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:textSize="18sp"
                    android:textStyle="bold" />

                <TextView
                    android:id="@+id/tv_cinema_name"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:textSize="14sp" />

                <TextView
                    android:id="@+id/tv_showtime_info"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:textSize="14sp" />
            </LinearLayout>

            <!-- Screen Indicator -->
            <TextView
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_gravity="center"
                android:text="🖥️ MÀN HÌNH 🖥️"
                android:textSize="16sp"
                android:padding="16dp" />

            <!-- Seats Grid -->
            <FrameLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content">

                <androidx.recyclerview.widget.RecyclerView
                    android:id="@+id/rv_seats"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:padding="8dp" />

                <ProgressBar
                    android:id="@+id/progress_bar"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:layout_gravity="center"
                    android:visibility="gone" />
            </FrameLayout>

            <!-- Legend -->
            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:padding="16dp"
                android:gravity="center">

                <TextView
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="🟢 Còn trống"
                    android:textSize="12sp" />

                <TextView
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="🔴 Đã đặt"
                    android:textSize="12sp" />

                <TextView
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="💙 Đang chọn"
                    android:textSize="12sp" />

                <TextView
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="💛 VIP"
                    android:textSize="12sp" />
            </LinearLayout>

            <View
                android:layout_width="match_parent"
                android:layout_height="180dp" />
        </LinearLayout>
    </androidx.core.widget.NestedScrollView>

    <!-- Bottom Sheet -->
    <com.google.android.material.card.MaterialCardView
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_gravity="bottom"
        app:cardElevation="8dp"
        app:cardCornerRadius="16dp">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:padding="16dp">

            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal">

                <TextView
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="💺 Ghế đã chọn: "
                    android:textSize="14sp" />

                <TextView
                    android:id="@+id/tv_selected_seats"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="Chưa chọn"
                    android:textSize="14sp"
                    android:textStyle="bold" />
            </LinearLayout>

            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:layout_marginTop="8dp">

                <TextView
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="💰 Tổng tiền: "
                    android:textSize="14sp" />

                <TextView
                    android:id="@+id/tv_total_price"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="0đ"
                    android:textSize="16sp"
                    android:textStyle="bold"
                    android:textColor="@color/primary" />
            </LinearLayout>

            <com.google.android.material.button.MaterialButton
                android:id="@+id/btn_continue"
                android:layout_width="match_parent"
                android:layout_height="56dp"
                android:layout_marginTop="16dp"
                android:text="Tiếp tục"
                android:textSize="16sp"
                android:enabled="false"
                app:cornerRadius="8dp" />
        </LinearLayout>
    </com.google.android.material.card.MaterialCardView>

</androidx.coordinatorlayout.widget.CoordinatorLayout>
```

#### **item_seat.xml**

```xml
<?xml version="1.0" encoding="utf-8"?>
<FrameLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="32dp"
    android:layout_height="32dp"
    android:layout_margin="2dp">

    <View
        android:id="@+id/iv_seat"
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:background="@drawable/bg_seat_available" />

    <TextView
        android:id="@+id/tv_seat_number"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="center"
        android:textSize="10sp"
        android:textStyle="bold"
        android:textColor="@android:color/white" />
</FrameLayout>
```

#### **item_seat_row_header.xml**

```xml
<?xml version="1.0" encoding="utf-8"?>
<TextView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:id="@+id/tv_row"
    android:layout_width="32dp"
    android:layout_height="32dp"
    android:layout_margin="2dp"
    android:gravity="center"
    android:textSize="14sp"
    android:textStyle="bold" />
```

---

### **6. Drawables (Colors)**

```xml
<!-- res/drawable/bg_seat_available.xml -->
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="#4CAF50" />
    <corners android:radius="4dp" />
</shape>

<!-- res/drawable/bg_seat_vip.xml -->
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="#FFC107" />
    <corners android:radius="4dp" />
</shape>

<!-- res/drawable/bg_seat_booked.xml -->
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="#F44336" />
    <corners android:radius="4dp" />
</shape>

<!-- res/drawable/bg_seat_selected.xml -->
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="#2196F3" />
    <corners android:radius="4dp" />
</shape>
```

---

## 🎯 **BUSINESS RULES**

### **1. Validation Rules**

| Rule | Mô tả | Error Message |
|------|-------|---------------|
| Min 1 seat | Phải chọn ít nhất 1 ghế | "Vui lòng chọn ít nhất 1 ghế" |
| Max 10 seats | Tối đa 10 ghế mỗi booking | "Chỉ được chọn tối đa 10 ghế" |
| Available only | Chỉ chọn ghế `isAvailableForShowtime = true` | "Ghế {name} đã có người đặt" |
| Same showtime | Tất cả ghế cùng 1 suất chiếu | Auto-handled |

### **2. Price Calculation (Flat Pricing)**

```java
// ⚠️ Backend sử dụng FLAT PRICING
// Tất cả ghế đều có cùng giá = showtime.Price

// Example:
// ticketPrice = 100,000đ (từ showtime.Price)
// User chọn 3 ghế (Standard, VIP, Couple)
// Total = 100,000 * 3 = 300,000đ

private void updateTotalPrice() {
    int selectedCount = selectedSeats.size();
    double total = ticketPrice * selectedCount;  // ← Đơn giản
    
    tvTotalPrice.setText(formatCurrency(total));
}
```

**Lưu ý**: 
- Ghế VIP chỉ khác màu UI (💛 vàng), giá vẫn = ghế thường
- Backend tính: `totalAmount = showtime.Price * seatIds.Count`
- Frontend chỉ cần: `total = ticketPrice * selectedSeats.size()`

### **3. Seat Selection Flow**

```
User click ghế
     ↓
[Check state]
     ↓
├─ AVAILABLE:
│  ├─ Check: selectedSeats.size() < 10?
│  ├─ Yes → Add to selected, setState(SELECTED)
│  └─ No → Show toast "Chỉ được chọn tối đa 10 ghế"
│
├─ SELECTED:
│  └─ Remove from selected, setState(AVAILABLE)
│
└─ BOOKED:
   └─ Show toast "Ghế {name} đã có người đặt"
     ↓
Update UI: adapter.notifyDataSetChanged()
     ↓
Update Bottom Sheet: 
- Display selected seat names
- Calculate total = ticketPrice * count
- Enable/disable button
```

---

## 🚨 **ERROR HANDLING**

### **1. Common API Errors**

```java
@Override
public void onResponse(Call<AuditoriumSeatsResponse> call, 
                     Response<AuditoriumSeatsResponse> response) {
    progressBar.setVisibility(View.GONE);
    
    if (response.isSuccessful() && response.body() != null) {
        AuditoriumSeatsResponse data = response.body();
        if (data.isSuccess()) {
            processSeats(data.getData().getSeats());
        } else {
            showError("Không thể tải danh sách ghế");
        }
    } else {
        switch (response.code()) {
            case 404:
                showError("Không tìm thấy phòng chiếu");
                break;
            case 500:
                showError("Lỗi server, vui lòng thử lại");
                break;
            default:
                showError("Lỗi: " + response.code());
                break;
        }
    }
}

@Override
public void onFailure(Call<AuditoriumSeatsResponse> call, Throwable t) {
    progressBar.setVisibility(View.GONE);
    
    if (t instanceof UnknownHostException) {
        showError("Không có kết nối internet");
    } else if (t instanceof SocketTimeoutException) {
        showError("Kết nối quá chậm, vui lòng thử lại");
    } else {
        showError("Lỗi kết nối: " + t.getMessage());
    }
}
```

### **2. Common Issues**

| Issue | Nguyên nhân | Giải pháp |
|-------|-------------|-----------|
| All seats show available | Thiếu `showtimeId` trong API call | Luôn truyền `showtimeId` parameter |
| Can't select seat | Seat state = `BOOKED` | Show message "Ghế đã đặt" |
| 500 error on create | Ghế đã bị book bởi user khác | Reload seats và thử lại |
| Wrong total price | Tính toán sai | Verify: `total = ticketPrice * count` |

---

## 🧪 **TESTING CHECKLIST**

### **Manual Test Cases**

#### **Test Case 1: Load seats thành công**
```
1. Vào màn chọn ghế với showtimeId=42
2. Verify: API được gọi với đúng showtimeId
3. Verify: Ghế hiển thị đúng màu sắc:
   - 🟢 Available (Standard)
   - 💛 Available (VIP) - giá vẫn bằng thường
   - 🔴 Booked
4. Verify: Không crash, không lỗi
```

#### **Test Case 2: Chọn ghế available**
```
1. Click ghế available (màu xanh lá)
2. Verify: Ghế chuyển sang màu xanh dương (selected)
3. Verify: Bottom sheet hiển thị tên ghế (VD: "A1")
4. Verify: Total price = ticketPrice * 1
5. Verify: Button "Tiếp tục" enabled
```

#### **Test Case 3: Chọn ghế VIP**
```
1. Click ghế VIP (màu vàng)
2. Verify: Ghế chuyển sang màu xanh dương (selected)
3. Verify: Bottom sheet hiển thị đúng tên ghế
4. Verify: ⚠️ Price vẫn bằng ghế thường (flat pricing)
5. Verify: Total = ticketPrice * count (không có multiplier)
```

#### **Test Case 4: Bỏ chọn ghế**
```
1. Chọn ghế A1 (màu xanh dương)
2. Click lại ghế A1
3. Verify: Ghế chuyển lại màu ban đầu (xanh lá hoặc vàng)
4. Verify: Bottom sheet cập nhật (xóa ghế A1)
5. Verify: Total price giảm
```

#### **Test Case 5: Click ghế booked**
```
1. Click ghế màu đỏ (booked)
2. Verify: Toast "Ghế {name} đã có người đặt"
3. Verify: Ghế vẫn màu đỏ
4. Verify: Không thêm vào selectedSeats
```

#### **Test Case 6: Chọn quá 10 ghế**
```
1. Chọn 10 ghế available
2. Verify: Button enabled
3. Click ghế thứ 11
4. Verify: Toast "Chỉ được chọn tối đa 10 ghế"
5. Verify: Ghế thứ 11 không được chọn
```

#### **Test Case 7: Tạo booking thành công**
```
1. Chọn 2 ghế (A1, A2)
2. Click "Tiếp tục"
3. Verify: API POST /api/bookings/create được gọi
4. Verify: Request body đúng format:
   {
     "showtimeid": 42,
     "seatids": [1, 2]
   }
5. Verify: Navigate sang màn chọn combo với bookingId
```

---

## 📞 **FAQ**

### Q1: Tại sao phải truyền `showtimeId` khi gọi API?
**A**: Backend cần `showtimeId` để tính toán field `isAvailableForShowtime` cho từng ghế. Nếu không truyền, backend không biết ghế nào đã đặt cho suất chiếu này.

### Q2: Field `isAvailableForShowtime` khác `seats.isavailable` như thế nào?
**A**: 
- `seats.isavailable` (database): Ghế có hỏng không (broken)?
- `isAvailableForShowtime` (computed): Ghế còn trống cho **suất chiếu cụ thể** này không?

Xem chi tiết: `/docs/frontend/SEAT-AVAILABILITY-GUIDE.md`

### Q3: Ghế VIP có giá khác ghế thường không?
**A**: KHÔNG. Backend hiện tại dùng **flat pricing** (giá đồng nhất). Tất cả ghế = `showtime.Price`. Ghế VIP chỉ khác màu UI (vàng thay vì xanh lá) để user biết đó là ghế VIP, nhưng giá vẫn bằng thường.

### Q4: User có thể chọn ghế đã đặt không?
**A**: KHÔNG. Khi user click ghế có `isAvailableForShowtime = false`, chỉ hiển thị toast "Ghế đã đặt", không cho chọn.

### Q5: Booking có hết hạn không?
**A**: CÓ. Booking ở trạng thái `Pending` sẽ tự động cancel sau **15 phút** nếu không thanh toán. Background service chạy mỗi 5 phút để auto-cancel.

### Q6: Làm sao lấy `ticketPrice`?
**A**: Từ màn trước (màn chọn suất chiếu). Khi user chọn suất chiếu, backend trả về `showtime.Price`. Frontend truyền qua Intent:

```java
// Màn chọn suất chiếu
Intent intent = new Intent(this, SelectSeatActivity.class);
intent.putExtra("SHOWTIME_ID", showtime.getShowtimeid());
intent.putExtra("AUDITORIUM_ID", showtime.getAuditoriumid());
intent.putExtra("TICKET_PRICE", showtime.getPrice());  // ← Quan trọng
startActivity(intent);
```

---

## 🔗 **RELATED DOCS**

- 📘 [Seat Availability Guide](./SEAT-AVAILABILITY-GUIDE.md) - Hiểu rõ 2 loại isAvailable
- 📘 [Full Implementation](./SELECT-SEAT-IMPLEMENTATION-JAVA.md) - Code chi tiết với XML layouts
- 🔍 [Backend Code](../backend/) - Booking flow implementation

---

**Ngày tạo**: November 5, 2025  
**Version**: 2.0 - Flat Pricing  
**Status**: ✅ Active  
**Contact**: Backend Team - Trung
