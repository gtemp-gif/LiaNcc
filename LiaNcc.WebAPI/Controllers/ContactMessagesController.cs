using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.Entities;
using LiaNcc.Models.Enums;
using LiaNcc.Repository.Interfaces;
using LiaNcc.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/contact-messages")]
    [Authorize]
    public class ContactMessagesController : ControllerBase
    {
        private readonly IContactMessageRepository _contactMessageRepository;
        private readonly IMailService _mailService;
        private readonly IApplicationLoggerService _logger;

        public ContactMessagesController(
            IContactMessageRepository contactMessageRepository,
            IMailService mailService,
            IApplicationLoggerService logger)
        {
            _contactMessageRepository = contactMessageRepository;
            _mailService = mailService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetContactMessages()
        {
            return Ok(await _contactMessageRepository.GetAllAsync());
        }

        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetUnreadMessages()
        {
            return Ok(await _contactMessageRepository.GetUnreadAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactMessage>> GetContactMessage(Guid id)
        {
            var msg = await _contactMessageRepository.GetByIdAsync(id);
            if (msg == null) return NotFound();
            return Ok(msg);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ContactMessage>> CreateContactMessage(ContactMessageCreateRequest request)
        {
            var message = new ContactMessage
            {
                FullName = request.FullName,
                Email = request.Email,
                Message = request.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _contactMessageRepository.CreateAsync(message);
            await _logger.LogInformationAsync("Contact", "CreateMessage", $"Contact message from {message.FullName} created", "Contact", "ContactMessage", message.Id, ApplicationEventType.Contact);

            // Invia email asincronamente
            _ = Task.Run(async () =>
            {
                try
                {
                    await _mailService.SendContactNotificationAsync(message);
                    await _mailService.SendContactCustomerConfirmationAsync(message);
                }
                catch (Exception ex)
                {
                    // L'errore è già loggato dentro MailService, ma logghiamo anche qui per contesto
                    await _logger.LogErrorAsync("Contact", "EmailNotificationError", $"Errore invio email per messaggio {message.Id}", ex, null, "Contact", "ContactMessage", message.Id);
                }
            });

            return Ok(message);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _contactMessageRepository.MarkAsReadAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactMessage(Guid id)
        {
            await _contactMessageRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
