# 🚂 Railway Deployment Guide for Movie88 API

## 📋 Prerequisites

1. **Railway Account**: Sign up at [railway.app](https://railway.app)
2. **GitHub Repository**: https://github.com/Zun1702/Movie88.aspnet.git
3. **Database**: PostgreSQL (can use Railway's PostgreSQL or keep Supabase)

---

## 🚀 Deployment Steps

### Step 1: Create New Project on Railway

1. Go to [railway.app](https://railway.app)
2. Click **"New Project"**
3. Select **"Deploy from GitHub repo"**
4. Choose **`Zun1702/Movie88.aspnet`** repository
5. Railway will automatically detect the Dockerfile

### Step 2: Configure Environment Variables

Click on your service → **Variables** tab → Add the following:

#### Required Environment Variables:

```bash
# Database Connection
ConnectionStrings__DefaultConnection=Host=your-host;Port=5432;Database=your-db;Username=your-user;Password=your-password;SSL Mode=Require;Trust Server Certificate=true

# JWT Settings
JwtSettings__Secret=pLnfNs/Fznpw1dPg5K3TcBk6vs47knb+L4ns1LZQV2J3Gf2y2eSw+iefsemj6LZy
JwtSettings__Issuer=Movie88API
JwtSettings__Audience=Movie88Client
JwtSettings__AccessTokenExpirationMinutes=60
JwtSettings__RefreshTokenExpirationDays=7

# ASP.NET Environment
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Railway Port (automatically set by Railway)
PORT=8080
```

#### 🔗 Using Supabase Database (Recommended):

```bash
ConnectionStrings__DefaultConnection=Host=aws-1-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bkhexjwlqvjwfnncnaun;Password=Yeah@17022004;SSL Mode=Require;Trust Server Certificate=true;Timeout=30;Command Timeout=30;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=10
```

#### 🔗 Using Railway PostgreSQL (Alternative):

1. Click **"+ New"** → **"Database"** → **"Add PostgreSQL"**
2. Railway will auto-create `DATABASE_URL` variable
3. Convert it to format:
```bash
ConnectionStrings__DefaultConnection=Host=${{PGHOST}};Port=${{PGPORT}};Database=${{PGDATABASE}};Username=${{PGUSER}};Password=${{PGPASSWORD}};SSL Mode=Require;Trust Server Certificate=true
```

### Step 3: Deploy

1. Railway will automatically start building from Dockerfile
2. Wait 5-10 minutes for build to complete
3. Once deployed, Railway will provide a public URL like:
   ```
   https://movie88-aspnet-production.up.railway.app
   ```

### Step 4: Verify Deployment

Test the health endpoint:
```bash
curl https://your-railway-url.railway.app/health
```

Expected response:
```json
{
  "status": "Healthy",
  "timestamp": "2025-11-04T10:30:00Z"
}
```

---

## 🔧 Configuration Details

### Dockerfile Configuration

The `Dockerfile` at root uses:
- **Build Stage**: .NET SDK 8.0 to compile the app
- **Runtime Stage**: .NET ASP.NET 8.0 (lighter, production-ready)
- **Port**: 8080 (Railway default)
- **Entry Point**: `dotnet Movie88.WebApi.dll`

### Railway Configuration Files

**railway.json** or **railway.toml**:
```json
{
  "build": {
    "builder": "DOCKERFILE",
    "dockerfilePath": "Dockerfile"
  },
  "deploy": {
    "startCommand": "dotnet Movie88.WebApi.dll",
    "restartPolicyType": "ON_FAILURE",
    "restartPolicyMaxRetries": 10
  }
}
```

---

## 🌐 Custom Domain (Optional)

1. Go to **Settings** → **Domains**
2. Click **"Generate Domain"** for free Railway subdomain
3. Or add your custom domain (e.g., `api.movie88.com`)

---

## 📊 Monitoring & Logs

### View Logs:
1. Click on your service
2. Go to **"Deployments"** tab
3. Click on latest deployment
4. View real-time logs

### Common Issues:

**Problem**: Build fails with "restore failed"
```bash
Solution: Check .csproj files are correctly copied in Dockerfile
```

**Problem**: Container exits immediately
```bash
Solution: Check environment variables are set correctly
```

**Problem**: Database connection timeout
```bash
Solution: Verify PostgreSQL connection string format
Solution: Check SSL Mode and Trust Server Certificate settings
```

**Problem**: Port binding error
```bash
Solution: Ensure ASPNETCORE_URLS=http://+:8080
Solution: Railway auto-sets PORT=8080
```

---

## 🔒 Security Best Practices

### ✅ Do:
- Use Railway's environment variables for secrets
- Generate unique JWT Secret for production
- Use HTTPS for all endpoints (Railway auto-provides)
- Keep `appsettings.Development.json` out of git

### ❌ Don't:
- Hardcode database passwords in Dockerfile
- Commit JWT secrets to GitHub
- Use Development environment in production

---

## 🔄 CI/CD with Railway

Railway automatically redeploys when you push to GitHub:

```bash
# Make changes locally
git add .
git commit -m "Update API endpoints"
git push origin main

# Railway will automatically:
# 1. Detect the push
# 2. Build new Docker image
# 3. Deploy to production
# 4. Health check
# 5. Route traffic to new version
```

---

## 📈 Scaling Options

Railway offers:
- **Vertical Scaling**: Increase CPU/Memory in Settings
- **Horizontal Scaling**: Add replicas (Pro plan)
- **Auto-scaling**: Based on traffic (Pro plan)

---

## 💰 Pricing

**Free Tier**:
- $5 free credit/month
- Enough for development/testing
- Sleeps after inactivity

**Hobby Plan** ($5/month):
- Always on
- Custom domains
- More resources

**Pro Plan** ($20/month):
- Priority support
- High availability
- Advanced metrics

---

## 🧪 Testing Deployed API

### Using cURL:

```bash
# Health Check
curl https://your-app.railway.app/health

# Register User
curl -X POST https://your-app.railway.app/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123",
    "fullname": "Test User",
    "phonenumber": "0123456789"
  }'

# Login
curl -X POST https://your-app.railway.app/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123"
  }'

# Get Movies
curl https://your-app.railway.app/api/movies
```

### Using Postman:

1. Import `tests/Movies.http` endpoints
2. Replace `https://localhost:7238` with Railway URL
3. Test all endpoints

---

## 📞 Support

- **Railway Docs**: https://docs.railway.app
- **Railway Discord**: https://discord.gg/railway
- **Project Issues**: https://github.com/Zun1702/Movie88.aspnet/issues

---

## ✅ Deployment Checklist

- [x] Dockerfile created at root
- [x] railway.json/railway.toml configured
- [x] .dockerignore optimized
- [x] Environment variables documented
- [ ] Railway account created
- [ ] GitHub repo connected to Railway
- [ ] Environment variables set in Railway
- [ ] Database connection tested
- [ ] API endpoints verified
- [ ] Custom domain configured (optional)
- [ ] Monitoring enabled

---

**Last Updated**: November 4, 2025  
**Railway Config Version**: 1.0  
**Dockerfile Version**: .NET 8.0
