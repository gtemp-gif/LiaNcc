using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Requests;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/contact-messages")]
    [Authorize(Roles = "Admin,Operator")]
    public class ContactMessagesController : ControllerBase
    {
        private readonly IContactMessageRepository _contactMessageRepository;

        public ContactMessagesController(IContactMessageRepository contactMessageRepository)
        {
            _contactMessageRepository = contactMessageRepository;
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
        public async Task<ActionResult<ContactMessage>> CreateContactMessage(CreateContactMessageRequest request)
        {
            var message = new ContactMessage
            {
                FullName = request.FullName,
                Email = request.Email,
                Message = request.Message,
                IsRead = false
            };

            await _contactMessageRepository.CreateAsync(message);
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
