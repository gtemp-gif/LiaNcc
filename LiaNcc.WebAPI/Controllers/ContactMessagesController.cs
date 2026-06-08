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

            await _logger.LogInformationAsync(
                "Contact",
                "CreateMessage",
                $"Contact message from {message.FullName} created",
                "Contact",
                "ContactMessage",
                message.Id,
                ApplicationEventType.Contact
            );

            try
            {
                await _mailService.SendContactNotificationAsync(message);
                await _mailService.SendContactCustomerConfirmationAsync(message);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    "Contact",
                    "EmailNotificationError",
                    $"Errore invio email per messaggio {message.Id}",
                    ex,
                    null,
                    "Contact",
                    "ContactMessage",
                    message.Id
                );

                // Puoi decidere se restituire comunque OK oppure errore.
                // Per ora consiglio OK, perché il messaggio è stato salvato.
            }

            return Ok(message);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _contactMessageRepository.MarkAsReadAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/reply")]
        public async Task<IActionResult> Reply(Guid id, [FromForm] ReplyMessageRequest request, List<IFormFile>? attachmentsFiles)
        {
            var msg = await _contactMessageRepository.GetByIdAsync(id);
            if (msg == null) return NotFound();

            var attachments = new List<(string FileName, byte[] Content, string ContentType)>();
            if (attachmentsFiles != null)
            {
                foreach (var file in attachmentsFiles)
                {
                    using var ms = new System.IO.MemoryStream();
                    await file.CopyToAsync(ms);
                    attachments.Add((file.FileName, ms.ToArray(), file.ContentType));
                }
            }

            await _mailService.SendReplyEmailAsync(msg.Email, request.Subject, request.Body, attachments, "ContactMessage", id);

            await _logger.LogInformationAsync(
                "Contact",
                "ReplyMessage",
                $"Reply sent to {msg.Email} for message {id}",
                "Contact",
                "ContactMessage",
                id,
                ApplicationEventType.Email
            );

            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactMessage(Guid id)
        {
            await _contactMessageRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
