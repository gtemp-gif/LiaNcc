using LiaNcc.Models.Entities;

namespace LiaNcc.FE.Models.ViewModels
{
    public class ToursPageViewModel
    {
        public SitePage? CmsPage { get; set; }
        public IEnumerable<Tour> Tours { get; set; } = new List<Tour>();
    }
}
