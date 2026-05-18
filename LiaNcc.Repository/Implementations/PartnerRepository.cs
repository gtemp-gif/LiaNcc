using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class PartnerRepository : IPartnerRepository
    {
        private readonly LiaNccDbContext _context;

        public PartnerRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Partner>> GetAllAsync()
        {
            return await _context.Partners.AsNoTracking().OrderBy(p => p.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<Partner>> GetActivePartnersAsync()
        {
            return await _context.Partners.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToListAsync();
        }

        public async Task<Partner?> GetByIdAsync(Guid id)
        {
            return await _context.Partners.FindAsync(id);
        }

        public async Task<Partner> CreateAsync(Partner partner)
        {
            _context.Partners.Add(partner);
            await _context.SaveChangesAsync();
            return partner;
        }

        public async Task UpdateAsync(Partner partner)
        {
            partner.UpdatedAt = DateTime.UtcNow;
            _context.Partners.Update(partner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner != null)
            {
                _context.Partners.Remove(partner);
                await _context.SaveChangesAsync();
            }
        }
    }
}
