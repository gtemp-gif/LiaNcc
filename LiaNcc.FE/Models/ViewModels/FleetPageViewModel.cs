using LiaNcc.Models.Entities;

namespace LiaNcc.FE.Models.ViewModels
{
    public class FleetPageViewModel
    {
        public SitePage? CmsPage { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
