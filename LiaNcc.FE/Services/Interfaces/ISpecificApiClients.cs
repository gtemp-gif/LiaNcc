using LiaNcc.Models.DTOs.Dashboard;
using LiaNcc.Models.DTOs.Tours;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiaNcc.FE.Services.Interfaces
{
    public interface IServicesApiClient : IApiClient<Service, Guid>
    {
        Task<IEnumerable<Service>> GetActiveAsync(string? culture = null);
        Task<IEnumerable<Service>> GetFeaturedAsync(string? culture = null);
    }

    public interface IVehiclesApiClient : IApiClient<VehicleDto, Guid>
    {
        Task<IEnumerable<VehicleDto>> GetActiveAsync(string? culture = null);
        Task<IEnumerable<VehicleDto>> GetFeaturedAsync(string? culture = null);
    }

    public interface IToursApiClient : IApiClient<TourDto, Guid>
    {
        Task<IEnumerable<TourDto>> GetActiveAsync(string? culture = null);
        Task<IEnumerable<TourDto>> GetFeaturedAsync(string? culture = null);
        Task<Tour?> GetDetailAsync(Guid id, string? culture = null);
        Task<Tour?> GetDetailBySlugAsync(string slug, string? culture = null);
        Task<IEnumerable<TourGalleryImageDto>> GetTourGalleryAsync(Guid id);
    }

    public interface IPartnersApiClient : IApiClient<Partner, Guid>
    {
        Task<IEnumerable<Partner>> GetActiveAsync();
    }

    public interface ICompanyApiClient
    {
        Task<CompanyProfile?> GetCompanyProfileAsync(string? culture = null);
        Task<List<CompanyContactDto>> GetContactsAsync();
    }

    public interface ISitePagesApiClient : IApiClient<SitePage, Guid>
    {
        Task<SitePage?> GetBySlugAsync(string slug, string? culture = null);
        Task<SitePage?> GetFullBySlugAsync(string slug, string? culture = null);
    }

    public interface IContactMessagesApiClient
    {
        Task CreateAsync(LiaNcc.Models.DTOs.Requests.ContactMessageCreateRequest request);
        Task<IEnumerable<ContactMessage>> GetAllAsync();
        Task<ContactMessage?> GetByIdAsync(Guid id);
        Task MarkAsReadAsync(Guid id);
        Task DeleteAsync(Guid id);
    }

    public interface IBookingsApiClient
    {
        Task CreateAsync(LiaNcc.Models.DTOs.Requests.BookingCreateRequest request);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task UpdateStatusAsync(Guid id, string status);
        Task<IEnumerable<Service>> GetServiceTypesAsync(string? culture = null);
        Task<IEnumerable<BookingPassengerOption>> GetPassengerOptionsAsync(string? culture = null);
    }
}
