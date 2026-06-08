using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class ContactMessageRepository : IContactMessageRepository
    {
        private readonly LiaNccDbContext _context;

        public ContactMessageRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync(ContactMessageFilterRequest? filter = null)
        {
            var query = _context.ContactMessages.AsNoTracking();

            if (filter != null)
            {
                if (filter.IsRead.HasValue)
                    query = query.Where(m => m.IsRead == filter.IsRead.Value);

                if (filter.FromDate.HasValue)
                    query = query.Where(m => m.CreatedAt >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(m => m.CreatedAt <= filter.ToDate.Value);

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    var search = filter.SearchText.ToLower();
                    query = query.Where(m => m.FullName.ToLower().Contains(search) ||
                                             m.Email.ToLower().Contains(search) ||
                                             m.Message.ToLower().Contains(search));
                }
            }

            return await query.OrderByDescending(m => m.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetUnreadAsync()
        {
            return await _context.ContactMessages.AsNoTracking().Where(m => !m.IsRead).OrderByDescending(m => m.CreatedAt).ToListAsync();
        }

        public async Task<ContactMessage?> GetByIdAsync(Guid id)
        {
            return await _context.ContactMessages.FindAsync(id);
        }

        public async Task<ContactMessage> CreateAsync(ContactMessage message)
        {
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task UpdateAsync(ContactMessage message)
        {
            _context.ContactMessages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}
