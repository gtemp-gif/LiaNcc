using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Dashboard;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IDashboardApiClient
    {
        Task<DashboardSummaryDto?> GetSummaryAsync();
    }
}
