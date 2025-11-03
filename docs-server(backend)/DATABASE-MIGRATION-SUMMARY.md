# 🔄 Database Migration: SQL Server → PostgreSQL

## 📋 Tóm tắt thay đổi

Dự án đã được chuyển đổi hoàn toàn từ **SQL Server** sang **PostgreSQL** để phù hợp hơn với yêu cầu triển khai và chi phí.

---

## ✅ Files đã được cập nhật

### 1. **docs/Overview.md**
**Thay đổi:**
- ✅ Tech stack table: `SQL Server` → `PostgreSQL`
- ✅ Architecture diagram: `SQL Server Database` → `PostgreSQL Database`
- ✅ Phase 1 checklist: `SQL Server, .NET 8` → `PostgreSQL, .NET 8`
- ✅ Quick Start: Thêm hướng dẫn chạy script trong PostgreSQL

**Dòng thay đổi:** 20, 61, 140, 304-306

---

### 2. **docs/Architecture.md**
**Thay đổi:**
- ✅ Architecture diagram: `SQL Server Database` → `PostgreSQL Database`

**Dòng thay đổi:** 61

---

### 3. **docs/README.md**
**Thay đổi:**
- ✅ Database Setup section: 
  - `sqlcmd -S localhost -U sa -P YourPassword -i DatabaseScript.txt`
  - → `psql -U postgres -f DatabaseScript.txt`
- ✅ Thêm hướng dẫn sử dụng pgAdmin

**Dòng thay đổi:** 317-318

---

### 4. **docs/modules/PaymentAPI.md**
**Thay đổi:**
- ✅ Payments table schema chuyển sang PostgreSQL syntax:
  - `INT IDENTITY(1,1)` → `SERIAL`
  - `NVARCHAR(50)` → `VARCHAR(50)`
  - `DATETIME` → `TIMESTAMP`
  - `DEFAULT GETDATE()` → `DEFAULT CURRENT_TIMESTAMP`

**Dòng thay đổi:** 362-369

**Chi tiết thay đổi:**
```diff
- PaymentId INT IDENTITY(1,1) PRIMARY KEY,
+ PaymentId SERIAL PRIMARY KEY,

- Status NVARCHAR(50) DEFAULT 'Pending',
+ Status VARCHAR(50) DEFAULT 'Pending',

- TransactionCode NVARCHAR(255) NULL UNIQUE,
+ TransactionCode VARCHAR(255) NULL UNIQUE,

- PaymentTime DATETIME DEFAULT GETDATE(),
+ PaymentTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

- RefundTime DATETIME NULL,
+ RefundTime TIMESTAMP NULL,
```

---

### 5. **docs/modules/AdminAPI.md**
**Thay đổi:**
- ✅ User creation flow chuyển sang PostgreSQL syntax:
  - `GETDATE()` → `CURRENT_TIMESTAMP`
  - `SCOPE_IDENTITY()` → `RETURNING UserId INTO @UserId` (hoặc `RETURNING UserId`)
  - `1` (INT for boolean) → `TRUE` (BOOLEAN)

**Dòng thay đổi:** 654-657

**Chi tiết thay đổi:**
```diff
- INSERT INTO Users (Email, PasswordHash, Role, IsActive, CreatedAt)
- VALUES (@Email, @PasswordHash, @Role, 1, GETDATE());
- DECLARE @UserId INT = SCOPE_IDENTITY();

+ INSERT INTO Users (Email, PasswordHash, Role, IsActive, CreatedAt)
+ VALUES (@Email, @PasswordHash, @Role, TRUE, CURRENT_TIMESTAMP)
+ RETURNING UserId INTO @UserId;
```

---

## 🔑 Key Syntax Differences: SQL Server vs PostgreSQL

### Auto-increment Primary Key
```diff
- INT IDENTITY(1,1) PRIMARY KEY
+ SERIAL PRIMARY KEY
```

### String Types
```diff
- NVARCHAR(n)
+ VARCHAR(n)
```

### Boolean Values
```diff
- BIT or INT (1/0)
+ BOOLEAN (TRUE/FALSE)
```

### DateTime
```diff
- DATETIME
+ TIMESTAMP
```

### Current Date/Time
```diff
- GETDATE()
+ CURRENT_TIMESTAMP
```

### Get Last Inserted ID
```diff
- SCOPE_IDENTITY()
+ RETURNING column_name
+ Or: lastval() function
```

### String Concatenation
```diff
- 'Hello' + 'World'
+ 'Hello' || 'World'
```

### TOP clause
```diff
- SELECT TOP 10 * FROM table
+ SELECT * FROM table LIMIT 10
```

### IF...ELSE
```diff
- IF condition BEGIN ... END
+ IF condition THEN ... END IF;
```

---

## 📝 Connection String Changes

### Trước (SQL Server):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CinemaBookingDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
}
```

### Sau (PostgreSQL):
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=CinemaBookingDB;Username=postgres;Password=YourPassword;Port=5432;"
}
```

---

## 🔧 Required NuGet Package Changes

### Remove (SQL Server):
```bash
dotnet remove package Microsoft.EntityFrameworkCore.SqlServer
```

### Add (PostgreSQL):
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### Update DbContext Configuration:
```diff
// In Program.cs or Startup.cs

- builder.Services.AddDbContext<CinemaDbContext>(options =>
-     options.UseSqlServer(connectionString));

+ builder.Services.AddDbContext<CinemaDbContext>(options =>
+     options.UseNpgsql(connectionString));
```

---

## 🗄️ Database Script Status

### ✅ DatabaseScript.txt (Given folder)
File này **ĐÃ ĐÚNG** với PostgreSQL syntax:
- ✅ Sử dụng `SERIAL` cho auto-increment
- ✅ Sử dụng `VARCHAR` thay vì `NVARCHAR`
- ✅ Sử dụng `TIMESTAMP` thay vì `DATETIME`
- ✅ Sử dụng `BOOLEAN` thay vì `BIT`
- ✅ Sử dụng `CURRENT_TIMESTAMP` thay vì `GETDATE()`
- ✅ Sử dụng quoted identifiers cho reserved words: `"User"`, `"Row"`, `"Number"`

**Không cần chỉnh sửa file này!**

---

## 🚀 Migration Steps

### 1. Install PostgreSQL
```bash
# Windows (using Chocolatey)
choco install postgresql

# macOS (using Homebrew)
brew install postgresql

# Linux (Ubuntu/Debian)
sudo apt-get install postgresql postgresql-contrib
```

### 2. Start PostgreSQL Service
```bash
# Windows
net start postgresql-x64-14

# macOS/Linux
sudo systemctl start postgresql
```

### 3. Create Database
```bash
# Option 1: Using psql
psql -U postgres
CREATE DATABASE CinemaBookingDB;
\c CinemaBookingDB
\i path/to/DatabaseScript.txt
\q

# Option 2: Using pgAdmin (GUI)
# - Open pgAdmin
# - Create new database: CinemaBookingDB
# - Open Query Tool
# - Paste and execute DatabaseScript.txt
```

### 4. Update .NET Project
```bash
# 1. Remove SQL Server package
dotnet remove package Microsoft.EntityFrameworkCore.SqlServer

# 2. Add PostgreSQL package
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# 3. Update connection string in appsettings.json

# 4. Update DbContext registration in Program.cs

# 5. Run migrations
dotnet ef database update
```

### 5. Verify Connection
```bash
# Run the application
dotnet run

# Check Swagger
# https://localhost:5001/swagger

# Test health endpoint
# GET /api/health/database
```

---

## ⚠️ Important Notes

### 1. Case Sensitivity
PostgreSQL is **case-sensitive** for identifiers:
- Table names in quotes: `"User"` ≠ `User`
- Always use double quotes for mixed-case or reserved words

### 2. Reserved Keywords
PostgreSQL has different reserved keywords than SQL Server:
- `User` is reserved → Use `"User"`
- `Row`, `Number` are reserved → Use `"Row"`, `"Number"`

### 3. Sequence Management
PostgreSQL uses sequences for auto-increment:
```sql
-- Check current sequence value
SELECT currval('tablename_columnname_seq');

-- Reset sequence (if needed)
SELECT setval('tablename_columnname_seq', 1, false);
```

### 4. Boolean Handling in EF Core
```csharp
// SQL Server (bit: 0/1)
entity.IsActive = true;  // Stored as 1

// PostgreSQL (boolean)
entity.IsActive = true;  // Stored as TRUE
```

### 5. LINQ Differences
Most LINQ queries work identically, but some functions differ:
```csharp
// SQL Server
.Where(x => x.Name.Contains("test"))  // LIKE '%test%'

// PostgreSQL (same)
.Where(x => x.Name.Contains("test"))  // LIKE '%test%'

// Case-insensitive search
// SQL Server: COLLATE
// PostgreSQL: ILIKE operator (need custom)
```

---

## 📊 Performance Considerations

### Advantages of PostgreSQL:
1. ✅ **Free & Open Source** - No licensing costs
2. ✅ **Better JSON Support** - Native JSONB type
3. ✅ **Advanced Indexing** - GiST, GIN, BRIN indexes
4. ✅ **Full Text Search** - Built-in FTS
5. ✅ **MVCC** - Better concurrency handling
6. ✅ **Cross-platform** - Windows, Linux, macOS
7. ✅ **Cloud-friendly** - Easy deployment on AWS RDS, Azure Database, Google Cloud SQL

### Index Recommendations:
```sql
-- Create indexes for frequently queried columns
CREATE INDEX idx_showtimes_movieid ON Showtimes(MovieId);
CREATE INDEX idx_showtimes_starttime ON Showtimes(StartTime);
CREATE INDEX idx_bookings_customerid ON Bookings(CustomerId);
CREATE INDEX idx_bookings_status ON Bookings(Status);
CREATE INDEX idx_payments_bookingid ON Payments(BookingId);
CREATE INDEX idx_payments_transactioncode ON Payments(TransactionCode);
```

---

## ✅ Verification Checklist

- [x] DatabaseScript.txt sử dụng PostgreSQL syntax
- [x] Overview.md cập nhật mention PostgreSQL
- [x] Architecture.md cập nhật diagram
- [x] README.md cập nhật database setup instructions
- [x] PaymentAPI.md cập nhật table schema
- [x] AdminAPI.md cập nhật SQL examples
- [ ] appsettings.json cập nhật connection string
- [ ] Program.cs/Startup.cs thay `UseSqlServer()` → `UseNpgsql()`
- [ ] NuGet packages updated
- [ ] Test all API endpoints after migration
- [ ] Verify seed data works correctly
- [ ] Performance testing với PostgreSQL

---

## 🔍 Testing After Migration

```bash
# 1. Test database connection
dotnet ef database update

# 2. Seed data
dotnet run --seed

# 3. Run integration tests
dotnet test

# 4. Manual API testing
# - Test authentication (login/register)
# - Test booking flow
# - Test payment (VNPay)
# - Test admin operations

# 5. Performance testing
# - Concurrent bookings
# - Seat locking
# - Payment transactions
```

---

## 📚 References

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql EF Core Provider](https://www.npgsql.org/efcore/)
- [SQL Server to PostgreSQL Migration Guide](https://wiki.postgresql.org/wiki/Things_to_find_out_about_when_moving_from_Microsoft_SQL_Server_to_PostgreSQL)

---

**Migration Date**: October 30, 2025
**Status**: ✅ Documentation Updated
**Next Steps**: Update .NET project code and test thoroughly

---

🎉 **All documentation files have been successfully updated to PostgreSQL!**
