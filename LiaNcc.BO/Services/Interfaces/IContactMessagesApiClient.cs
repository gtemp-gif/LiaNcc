using System;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Requests;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IContactMessagesApiClient
    {
        Task<IEnumerable<ContactMessage>> GetAllAsync(ContactMessageFilterRequest? filter = null);
        Task MarkAsReadAsync(Guid id);
        Task ReplyAsync(Guid id, string subject, string body, List<Microsoft.AspNetCore.Http.IFormFile>? attachments = null);
        Task DeleteAsync(Guid id);
    }
}
