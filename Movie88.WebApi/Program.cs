using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movie88.Application.Configuration;
using Movie88.Infrastructure;
using Movie88.WebApi.Extensions;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure HttpClient for Email Service
builder.Services.AddHttpClient();

// Configure Database
builder.Services.ConfigurePersistenceApp(builder.Configuration);

// Configure MediatR and Application Services
builder.Services.ConfigureApplicationApp();

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? ""))
    };
});

builder.Services.AddAuthorization();

// Configure JSON serialization
builder.Services.AddMvc()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Movie88 API", Version = "v1" });
    options.CustomSchemaIds(type => type.ToString());
    
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // lowercase
        BearerFormat = "JWT"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.ConfigureCorsPolicy();


builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",       // dev
                "https://localhost:5173",      // dev
                "https://movie88-reactvitejs.vercel.app"   // production vercel frontend
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Create database if not exists
CreateDatabase(app);

// Allow CORS
app.UseCors("FrontendCors");

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for Railway deployment
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie88 API v1");
    // Use default route "swagger" so it works at /swagger
    // c.RoutePrefix = string.Empty; // This makes it available at root /
});

// Disable HTTPS redirection for Railway (Railway handles HTTPS at edge)
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void CreateDatabase(WebApplication app)
{
    //var serviceScope = app.Services.CreateScope();
    //var dataContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
    //dataContext?.Database.EnsureCreated();
}