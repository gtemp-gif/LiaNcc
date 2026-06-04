using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiaNcc.FE.Services.Interfaces
{
    public interface IApiClient<T, TKey> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string? culture = null);
        Task<T?> GetByIdAsync(TKey id, string? culture = null);
    }
}
