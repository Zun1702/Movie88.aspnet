using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Movie88.Application.Interfaces;
using Movie88.Application.Services;
using Movie88.Application.Shared;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Movie88.Application.Configuration
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplicationApp(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            // Register Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, ResendEmailService>();
            services.AddSingleton<IBookingCodeGenerator, BookingCodeGenerator>();
            
            // Movie Service
            services.AddScoped<IMovieService, MovieService>();
            
            // Promotion Service
            services.AddScoped<IPromotionService, PromotionService>();
            
            // Customer Service
            services.AddScoped<ICustomerService, CustomerService>();
            
            // Booking Service
            services.AddScoped<IBookingService, BookingService>();
            
            // Review Service
            services.AddScoped<IReviewService, ReviewService>();
            
            // Showtime Service
            services.AddScoped<IShowtimeService, ShowtimeService>();
            
            // Cinema Service
            services.AddScoped<ICinemaService, CinemaService>();
        }
    }
}
