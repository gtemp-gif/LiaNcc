using System;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IContactMessagesApiClient : IApiClient<ContactMessage, Guid>
    {
        Task MarkAsReadAsync(Guid id);
        Task ReplyAsync(Guid id, string subject, string body, List<Microsoft.AspNetCore.Http.IFormFile>? attachments = null);
    }
}
