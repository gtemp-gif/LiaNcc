using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly LiaNccDbContext _context;

        public CompanyRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyProfile?> GetCompanyProfileAsync()
        {
            return await _context.CompanyProfiles.AsNoTracking()
                .Include(p => p.CompanyContacts)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CompanyContact>> GetCompanyContactsAsync()
        {
            var profile = await _context.CompanyProfiles.AsNoTracking().FirstOrDefaultAsync();
            if (profile == null) return new List<CompanyContact>();

            return await _context.CompanyContacts.AsNoTracking()
                .Where(c => c.CompanyId == profile.Id)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<CompanyContact?> GetContactByIdAsync(Guid id)
        {
            return await _context.CompanyContacts.FindAsync(id);
        }

        public async Task<CompanyProfile> CreateOrUpdateProfileAsync(CompanyProfile profile)
        {
            var existing = await _context.CompanyProfiles.FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.Name = profile.Name;
                existing.VatNumber = profile.VatNumber;
                existing.Address = profile.Address;
                existing.City = profile.City;
                existing.ZipCode = profile.ZipCode;
                existing.Country = profile.Country;
                existing.Latitude = profile.Latitude;
                existing.Longitude = profile.Longitude;
                existing.GoogleMapsUrl = profile.GoogleMapsUrl;
                existing.AboutTitle = profile.AboutTitle;
                existing.AboutDescription = profile.AboutDescription;
                existing.AboutImageUrl = profile.AboutImageUrl;
                existing.UpdatedAt = DateTime.UtcNow;
                _context.CompanyProfiles.Update(existing);
                await _context.SaveChangesAsync();
                return existing;
            }

            _context.CompanyProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<CompanyContact> CreateContactAsync(CompanyContact contact)
        {
            _context.CompanyContacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task UpdateContactAsync(CompanyContact contact)
        {
            contact.UpdatedAt = DateTime.UtcNow;
            _context.CompanyContacts.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContactAsync(Guid id)
        {
            var contact = await _context.CompanyContacts.FindAsync(id);
            if (contact != null)
            {
                _context.CompanyContacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }
    }
}
