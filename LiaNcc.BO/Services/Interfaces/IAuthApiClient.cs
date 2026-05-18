using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.DTOs.Responses;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IAuthApiClient
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
