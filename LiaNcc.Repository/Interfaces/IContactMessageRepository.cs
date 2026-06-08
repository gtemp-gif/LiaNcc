using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Requests;

namespace LiaNcc.Repository.Interfaces
{
    public interface IContactMessageRepository
    {
        Task<IEnumerable<ContactMessage>> GetAllAsync(ContactMessageFilterRequest? filter = null);
        Task<IEnumerable<ContactMessage>> GetUnreadAsync();
        Task<ContactMessage?> GetByIdAsync(Guid id);
        Task<ContactMessage> CreateAsync(ContactMessage message);
        Task UpdateAsync(ContactMessage message);
        Task MarkAsReadAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
