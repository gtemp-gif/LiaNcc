using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class EmailMessageRepository : IEmailMessageRepository
    {
        private readonly LiaNccDbContext _context;

        public EmailMessageRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(EmailMessage message)
        {
            _context.EmailMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailMessage message)
        {
            _context.EmailMessages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailMessage?> GetByIdAsync(Guid id)
        {
            return await _context.EmailMessages.FindAsync(id);
        }

        public async Task<IEnumerable<EmailMessage>> GetByRelatedEntityAsync(string entityName, Guid entityId)
        {
            return await _context.EmailMessages
                .Where(e => e.RelatedEntityName == entityName && e.RelatedEntityId == entityId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }
    }
}
