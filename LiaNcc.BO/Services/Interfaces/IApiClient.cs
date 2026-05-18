using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IApiClient<T, TKey> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(TKey id);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(TKey id, T entity);
        Task DeleteAsync(TKey id);
    }
}
