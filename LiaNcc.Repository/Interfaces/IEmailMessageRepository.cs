using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IEmailMessageRepository
    {
        Task CreateAsync(EmailMessage message);
        Task UpdateAsync(EmailMessage message);
        Task<EmailMessage?> GetByIdAsync(Guid id);
        Task<IEnumerable<EmailMessage>> GetByRelatedEntityAsync(string entityName, Guid entityId);
    }
}
