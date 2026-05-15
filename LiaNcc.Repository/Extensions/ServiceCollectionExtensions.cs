using LiaNcc.Repository.Implementations;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiaNcc.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLiaNccRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LiaNccDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<ITourRepository, TourRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

            return services;
        }
    }
}
