using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IContactMessageRepository
    {
        Task<IEnumerable<ContactMessage>> GetAllAsync();
        Task<ContactMessage?> GetByIdAsync(Guid id);
        Task<ContactMessage> CreateAsync(ContactMessage message);
        Task UpdateAsync(ContactMessage message);
        Task DeleteAsync(Guid id);
    }
}
