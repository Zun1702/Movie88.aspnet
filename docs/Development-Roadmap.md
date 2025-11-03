# 🎯 Development Roadmap - Quick Reference

> Roadmap ngắn gọn để track progress và next steps cho từng phase.

---

## ✅ Phase 1: Authentication (COMPLETED - Nov 3, 2025)

### Completed Features
- ✅ Login/Register/Logout
- ✅ JWT Token Management (Access + Refresh)
- ✅ Password Hashing (BCrypt)
- ✅ Change Password
- ✅ Forgot Password (placeholder)
- ✅ Clean Architecture setup
- ✅ Supabase Singapore region
- ✅ `public.refresh_tokens` table

### Files Created
```
Movie88.Application/
  ├── DTOs/Auth/ (6 DTOs)
  ├── Services/ (JwtService, AuthService, PasswordHashingService)
  └── Interfaces/ (IJwtService, IAuthService, IPasswordHashingService)

Movie88.Infrastructure/
  ├── Entities/UserRefreshToken.cs
  ├── Repositories/ (UserRepository, RefreshTokenRepository, UnitOfWork)
  └── Mappers/ (UserMapper, RefreshTokenMapper)

Movie88.WebApi/
  └── Controllers/AuthController.cs (6 endpoints)
```

### Test Results
- ✅ POST `/api/auth/register` - Works, creates Customer (roleId=3)
- ✅ POST `/api/auth/login` - Returns JWT + RefreshToken
- ✅ POST `/api/auth/refresh-token` - Safe parsing with TryParse
- ✅ POST `/api/auth/change-password` - Revokes all tokens
- ✅ POST `/api/auth/logout` - Revokes refresh token
- ✅ POST `/api/auth/forgot-password` - Placeholder returns true

---

## 🚀 Phase 2: Home Screen (NEXT - Target: Nov 4-5, 2025)

### Goals
Build core movie browsing features for HomeFragment.

### Entities Required
1. **Movie** (check if exists in Supabase)
   ```sql
   - movieid (PK)
   - title, overview, posterurl, backdropurl, trailerurl
   - rating, genre, duration, agerating
   - releasedate, createdat, updatedat
   ```

2. **Promotion** (check if exists)
   ```sql
   - promotionid (PK)
   - title, description, imageurl
   - discountpercentage, startdate, enddate
   - isactive
   ```

### API Endpoints to Build
| Method | Endpoint | Controller | Status |
|--------|----------|------------|--------|
| GET | `/api/movies` | MoviesController | ❌ |
| GET | `/api/movies/now-showing` | MoviesController | ❌ |
| GET | `/api/movies/coming-soon` | MoviesController | ❌ |
| GET | `/api/movies/search` | MoviesController | ❌ |
| GET | `/api/promotions/active` | PromotionsController | ❌ |

### Tasks Checklist
- [ ] Check Supabase for Movie & Promotion tables
- [ ] Create DTOs: `MovieResponseDTO`, `MovieListResponseDTO`, `PromotionResponseDTO`
- [ ] Create Repositories: `IMovieRepository`, `IPromotionRepository`
- [ ] Create Services: `IMovieService`, `IPromotionService`
- [ ] Create Controllers: `MoviesController`, `PromotionsController`
- [ ] Test with Swagger UI
- [ ] Test pagination (page, pageSize parameters)
- [ ] Test search functionality

### Expected File Structure
```
Movie88.Application/
  ├── DTOs/Movie/
  │   ├── MovieResponseDTO.cs
  │   └── MovieListResponseDTO.cs
  ├── DTOs/Promotion/
  │   └── PromotionResponseDTO.cs
  ├── Interfaces/
  │   ├── IMovieService.cs
  │   └── IPromotionService.cs
  └── Services/
      ├── MovieService.cs
      └── PromotionService.cs

Movie88.Domain/
  ├── Interfaces/
  │   ├── IMovieRepository.cs
  │   └── IPromotionRepository.cs
  └── Models/
      ├── MovieModel.cs
      └── PromotionModel.cs

Movie88.Infrastructure/
  ├── Repositories/
  │   ├── MovieRepository.cs
  │   └── PromotionRepository.cs
  └── Mappers/
      ├── MovieMapper.cs
      └── PromotionMapper.cs

Movie88.WebApi/
  └── Controllers/
      ├── MoviesController.cs
      └── PromotionsController.cs
```

---

## 🎬 Phase 3: Movie Details (Target: Nov 6-7, 2025)

### Entities Required
- Showtime (showtimeid, movieid, auditoriumid, starttime, price)
- Review (reviewid, userid, movieid, rating, comment)
- Auditorium (auditoriumid, cinemaid, name, capacity)
- Cinema (cinemaid, name, address, city)

### Endpoints (4)
- GET `/api/movies/{id}`
- GET `/api/movies/{id}/showtimes`
- GET `/api/reviews/movie/{movieId}`
- POST `/api/reviews`

---

## 🎫 Phase 4: Booking Flow (Target: Nov 8-12, 2025)

### Sub-phases
1. **Select Cinema** (4 endpoints)
2. **Select Seat** (4 endpoints)
3. **Select Combo** (2 endpoints)

### Total: 10 endpoints

---

## 💳 Phase 5: Payment (Target: Nov 13-15, 2025)

### Features
- VNPay integration
- Voucher system
- Payment confirmation

### Total: 7 endpoints

---

## 👤 Phase 6: Profile & History (Target: Nov 16-18, 2025)

### Features
- User profile management
- Avatar upload
- Booking history

### Total: 7 endpoints

---

## 📊 Overall Progress

| Phase | Endpoints | Status | Completion |
|-------|-----------|--------|------------|
| 1. Authentication | 6 | ✅ Done | 100% |
| 2. Home | 5 | 🚀 Next | 0% |
| 3. Movie Details | 4 | ⏳ Pending | 0% |
| 4. Booking Flow | 10 | ⏳ Pending | 0% |
| 5. Payment | 7 | ⏳ Pending | 0% |
| 6. Profile | 7 | ⏳ Pending | 0% |
| **TOTAL** | **41** | **6/41** | **14.6%** |

---

## 🎯 Current Status

**✅ Just Completed**: Authentication system (6 endpoints)  
**🚀 Next Up**: Home Screen - Movies & Promotions (5 endpoints)  
**📅 Timeline**: Aiming for 1 phase every 2-3 days  
**🎯 MVP Target**: Complete all 6 phases by Nov 18, 2025

---

**Last Updated**: November 3, 2025  
**Next Milestone**: Phase 2 - Home Screen APIs
