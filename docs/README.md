# 📖 README - docs Folder

> Tài liệu hướng dẫn phát triển API theo từng màn hình cho dự án Movie88.

---

## 📂 Cấu trúc tài liệu

### 📋 Core Documentation

#### 1. [Development-Roadmap.md](./Development-Roadmap.md) - Quick Reference
- ✅ Phase 1 completed summary
- 🚀 Next phase tasks
- 📅 Timeline estimates
- 📊 Overall progress (6/41 endpoints - 14.6%)

#### 2. [Testing-Guide.md](./Testing-Guide.md) - Swagger UI Testing
- 🔧 Setup instructions
- ✅ Sample requests/responses
- 🛠 Common issues & solutions
- 📊 Test results log

---

### 📱 Screen-by-Screen Documentation

> **7 nhóm màn hình, mỗi nhóm 1 file riêng để dễ theo dõi**

#### ✅ [01-Authentication.md](./screens/01-Authentication.md) - COMPLETED
- **Status**: ✅ 6/6 endpoints (100%)
- **Screens**: Login, Register, ForgotPassword
- **Completed**: Nov 3, 2025
- **What's inside**: JWT implementation, BCrypt, RefreshToken logic

---

#### 🚀 [02-Home.md](./screens/02-Home.md) - NEXT TARGET
- **Status**: ❌ 0/5 endpoints (0%)
- **Screens**: HomeFragment
- **Priority**: P0 - Critical
- **What's inside**: Movie listing, Promotions, Search
- **Estimated**: 4-6 hours

---

#### 🎬 [03-Movie-Details.md](./screens/03-Movie-Details.md)
- **Status**: ❌ 0/4 endpoints (0%)
- **Screens**: MovieDetailActivity
- **Priority**: P0 - Critical
- **What's inside**: Movie details, Showtimes, Reviews
- **Dependencies**: Home Screen (Movie entity)

---

#### 🎫 [04-Booking-Flow.md](./screens/04-Booking-Flow.md)
- **Status**: ❌ 0/10 endpoints (0%)
- **Screens**: SelectCinema, SelectSeat, SelectCombo
- **Priority**: P0 - Critical for revenue
- **What's inside**: 
  - Cinema selection (4 endpoints)
  - Seat selection (4 endpoints)
  - Combo selection (2 endpoints)
- **Estimated**: 12-16 hours

---

#### 💳 [05-Payment.md](./screens/05-Payment.md)
- **Status**: ❌ 0/7 endpoints (0%)
- **Screens**: BookingSummary, VNPayWebView, PaymentResult
- **Priority**: P0 - Critical for revenue
- **What's inside**: VNPay integration, Voucher system
- **Complexity**: High (external API)
- **Estimated**: 10-14 hours

---

#### 👤 [06-Profile-History.md](./screens/06-Profile-History.md)
- **Status**: ❌ 0/7 endpoints (0%)
- **Screens**: ProfileFragment, EditProfile, BookingHistory
- **Priority**: P1 - Important for UX
- **What's inside**: Profile management, Avatar upload, History
- **Estimated**: 8-10 hours

---

#### � [07-Search.md](./screens/07-Search.md)
- **Status**: ❌ 0/2 endpoints (0%)
- **Screens**: SearchMovieActivity
- **Priority**: P1-P2 - Nice to have
- **What's inside**: Advanced search, Filters, Sorting
- **Note**: Reuses Home Screen APIs
- **Estimated**: 4-6 hours

---

## 🎯 Workflow Recommended

### Khi bắt đầu màn hình mới:

1. **📋 Check API-Checklist-By-Screen.md**
   - Xem màn hình cần những endpoints gì
   - Check entities và DTOs required
   - Xác định priority

2. **🗺 Update Development-Roadmap.md**
   - Mark current phase as "in progress"
   - Create task checklist
   - Estimate completion date

3. **💻 Implement Code**
   - Create DTOs
   - Create Repositories
   - Create Services
   - Create Controllers

4. **🧪 Test with Testing-Guide.md**
   - Follow sample requests
   - Test all cases (success, error, edge)
   - Document test results
   - Update checklist

5. **✅ Mark Complete**
   - Update progress in API-Checklist
   - Update roadmap status
   - Commit code with clear message
   - Move to next screen

---

## 📊 Current Status

**Phase Completed**: 1/6 (Authentication)  
**Endpoints Completed**: 6/41 (14.6%)  
**Current Phase**: Phase 2 - Home Screen  
**Next Milestone**: Complete Home APIs (5 endpoints)  

---

## 🔗 Related Documents

### Backend Documentation
- [API_List.md](../docs-server(backend)/API_List.md) - Full 111 endpoints list (reference only)

### Frontend Documentation
- [API-Screen-Mapping-Summary.md](../docs-FrontEnd/API-Screen-Mapping-Summary.md) - Android screens mapping

---

## 💡 Tips

### 1. Focus on Screens, Not All APIs
- Không cần build 111 endpoints
- Chỉ build 41 endpoints theo màn hình Android
- Ưu tiên P0 (Critical) trước

### 2. Test Immediately
- Đừng để test sau khi làm xong nhiều màn hình
- Test ngay sau khi implement 1 controller
- Dễ debug và fix bugs sớm

### 3. Follow Clean Architecture
- Domain Models để tránh circular dependency
- Repository pattern với mappers
- Service layer cho business logic
- Controller chỉ handle HTTP requests

### 4. Reuse Code
- Nhiều màn hình dùng chung endpoints
- Tạo base classes cho common logic
- Share DTOs khi có thể

---

## 📞 Support

Nếu gặp vấn đề:
1. Check Testing-Guide "Common Issues" section
2. Review previous implementation (Auth phase)
3. Check Swagger UI error messages
4. Review connection string và database status

---

**Created**: November 3, 2025  
**Maintained by**: Development Team  
**Last Updated**: November 3, 2025
