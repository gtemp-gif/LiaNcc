using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface ILocalizedContentsApiClient
    {
        Task<IEnumerable<LocalizedContent>> GetByEntityAsync(string entityName, Guid entityId);
        Task UpsertBatchAsync(IEnumerable<LocalizedContent> items);
    }
}
