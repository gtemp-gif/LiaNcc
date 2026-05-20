using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Dashboard;

namespace LiaNcc.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
