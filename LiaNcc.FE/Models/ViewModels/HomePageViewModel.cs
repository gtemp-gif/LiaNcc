using LiaNcc.Models.Entities;

namespace LiaNcc.FE.Models.ViewModels
{
    public class HomePageViewModel
    {
        public SitePage? CmsPage { get; set; }
        public IEnumerable<Service> FeaturedServices { get; set; } = new List<Service>();
        public IEnumerable<Vehicle> FeaturedVehicles { get; set; } = new List<Vehicle>();
        public IEnumerable<Tour> FeaturedTours { get; set; } = new List<Tour>();
        public IEnumerable<Partner> Partners { get; set; } = new List<Partner>();
        public CompanyProfile? CompanyProfile { get; set; }
    }
}
