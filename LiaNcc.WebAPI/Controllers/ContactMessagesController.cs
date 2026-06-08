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
    [Route("api/contactmessages")]
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
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetContactMessages([FromQuery] ContactMessageFilterRequest filter)
        {
            return Ok(await _contactMessageRepository.GetAllAsync(filter));
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                area: "Contact",
                action: "CreateContactMessage",
                message: "Contact message created",
                controller: "ContactMessages",
                entityName: "ContactMessage",
                entityId: message.Id,
                eventType: ApplicationEventType.Create.ToString(),
                additionalData: request
            );

            try
            {
                await _mailService.SendContactNotificationAsync(message);
                await _logger.LogInformationAsync("Contact", "ContactAdminEmailSent", $"Admin notification sent for message {message.Id}", "ContactMessages", "ContactMessage", message.Id);

                await _mailService.SendContactCustomerConfirmationAsync(message);
                await _logger.LogInformationAsync("Contact", "ContactCustomerEmailSent", $"Customer confirmation sent for message {message.Id}", "ContactMessages", "ContactMessage", message.Id);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    area: "Mail",
                    action: "ContactEmailFailed",
                    message: $"Errore invio email per messaggio {message.Id}",
                    exception: ex,
                    controller: "ContactMessages",
                    entityName: "ContactMessage",
                    entityId: message.Id,
                    eventType: "Exception",
                    additionalData: request
                );
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
                "ContactMessages",
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
