using System.Collections.Generic;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.DTOs.Tours;

namespace LiaNcc.FE.Models
{
    public class HomeViewModel
    {
        public SitePage? Page { get; set; }
        public IEnumerable<Service> FeaturedServices { get; set; } = new List<Service>();
        public IEnumerable<VehicleDto> FeaturedVehicles { get; set; } = new List<VehicleDto>();
        public IEnumerable<TourDto> FeaturedTours { get; set; } = new List<TourDto>();
        public IEnumerable<Partner> Partners { get; set; } = new List<Partner>();
        public CompanyProfile? Company { get; set; }
    }

    public class FleetViewModel
    {
        public IEnumerable<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
    }

    public class ToursViewModel
    {
        public IEnumerable<TourDto> Tours { get; set; } = new List<TourDto>();
    }

    public class TourDetailViewModel
    {
        public Tour? Tour { get; set; }
    }
}
