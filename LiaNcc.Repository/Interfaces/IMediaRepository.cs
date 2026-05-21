using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface IMediaRepository
    {
        Task<IEnumerable<MediaAsset>> GetAllAsync();
        Task<MediaAsset?> GetByIdAsync(Guid id);
        Task<MediaAsset?> GetByFileNameAsync(string fileName);
        Task<IEnumerable<EntityMedia>> GetMediaForEntityAsync(string entityName, Guid entityId);
        Task<MediaAsset> CreateAsync(MediaAsset mediaAsset);
        Task UpdateAsync(MediaAsset mediaAsset);
        Task DeleteAsync(Guid id);
        Task<EntityMedia> AddMediaToEntityAsync(EntityMedia entityMedia);
        Task RemoveMediaFromEntityAsync(Guid entityMediaId);
    }
}
