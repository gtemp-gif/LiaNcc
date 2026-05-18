using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class MediaRepository : IMediaRepository
    {
        private readonly LiaNccDbContext _context;

        public MediaRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaAsset>> GetAllAsync()
        {
            return await _context.MediaAssets.AsNoTracking().ToListAsync();
        }

        public async Task<MediaAsset?> GetByIdAsync(Guid id)
        {
            return await _context.MediaAssets.FindAsync(id);
        }

        public async Task<IEnumerable<EntityMedia>> GetMediaForEntityAsync(string entityName, Guid entityId)
        {
            return await _context.EntityMedia.AsNoTracking()
                .Include(em => em.MediaAsset)
                .Where(em => em.EntityName == entityName && em.EntityId == entityId)
                .OrderBy(em => em.SortOrder)
                .ToListAsync();
        }

        public async Task<MediaAsset> CreateAsync(MediaAsset mediaAsset)
        {
            _context.MediaAssets.Add(mediaAsset);
            await _context.SaveChangesAsync();
            return mediaAsset;
        }

        public async Task UpdateAsync(MediaAsset mediaAsset)
        {
            _context.MediaAssets.Update(mediaAsset);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var media = await _context.MediaAssets.FindAsync(id);
            if (media != null)
            {
                _context.MediaAssets.Remove(media);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EntityMedia> AddMediaToEntityAsync(EntityMedia entityMedia)
        {
            _context.EntityMedia.Add(entityMedia);
            await _context.SaveChangesAsync();
            return entityMedia;
        }

        public async Task RemoveMediaFromEntityAsync(Guid entityMediaId)
        {
            var em = await _context.EntityMedia.FindAsync(entityMediaId);
            if (em != null)
            {
                _context.EntityMedia.Remove(em);
                await _context.SaveChangesAsync();
            }
        }
    }
}
