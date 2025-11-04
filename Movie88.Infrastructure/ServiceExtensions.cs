using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movie88.Application.Interfaces;
using Movie88.Application.Services;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Repositories;
using System.Reflection;

namespace Movie88.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistenceApp(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

            // Register AutoMapper - scan Infrastructure assembly for EntityToModelMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<Domain.Interfaces.IOtpTokenRepository, OtpTokenRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Movie Repository
            services.AddScoped<Domain.Interfaces.IMovieRepository, MovieRepository>();
            
            // Promotion Repository
            services.AddScoped<Domain.Interfaces.IPromotionRepository, PromotionRepository>();
            
            // Customer Repository
            services.AddScoped<Domain.Interfaces.ICustomerRepository, CustomerRepository>();
            
            // Booking Repository
            services.AddScoped<Domain.Interfaces.IBookingRepository, BookingRepository>();
            
            // Review Repository
            services.AddScoped<Domain.Interfaces.IReviewRepository, ReviewRepository>();
            
            // Showtime Repository
            services.AddScoped<Domain.Interfaces.IShowtimeRepository, ShowtimeRepository>();

            // Admin Repository
            services.AddScoped<IAdminService, AdminService>();
        }
    }
}
