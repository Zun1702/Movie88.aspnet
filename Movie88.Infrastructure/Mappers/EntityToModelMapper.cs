using AutoMapper;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Mappers;

public class EntityToModelMapper : Profile
{
    public EntityToModelMapper()
    {
        // Entity -> Model mappings
        CreateMap<Movie, MovieModel>();
        CreateMap<Promotion, PromotionModel>();
        
        CreateMap<Customer, CustomerModel>()
            .ForMember(dest => dest.Fullname, 
                opt => opt.MapFrom(src => src.User != null ? src.User.Fullname : string.Empty))
            .ForMember(dest => dest.Email, 
                opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
            .ForMember(dest => dest.Phone, 
                opt => opt.MapFrom(src => src.User != null ? src.User.Phone : null))
            .ForMember(dest => dest.Createdat, 
                opt => opt.MapFrom(src => src.User != null && src.User.Createdat.HasValue ? src.User.Createdat.Value : DateTime.MinValue));
        
        // Model -> Entity mappings (for repository Add/Update)
        CreateMap<CustomerModel, Customer>()
            .ForMember(dest => dest.User, opt => opt.Ignore()) // Don't map navigation property
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

        // Booking related mappings
        CreateMap<Booking, BookingModel>();
        CreateMap<Showtime, ShowtimeModel>();
        CreateMap<Auditorium, AuditoriumModel>();
        CreateMap<Cinema, CinemaModel>();
        CreateMap<Seat, SeatModel>();
        CreateMap<Bookingseat, BookingSeatModel>();
        CreateMap<Combo, ComboModel>();
        CreateMap<Bookingcombo, BookingComboModel>();
        CreateMap<Voucher, VoucherModel>();
        
        // Review mappings
        CreateMap<Review, ReviewModel>();
        CreateMap<ReviewModel, Review>()
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Movie, opt => opt.Ignore());
    }
}
