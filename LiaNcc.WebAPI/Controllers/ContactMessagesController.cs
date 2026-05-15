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
    [Route("api/[controller]")]
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
            var messages = await _contactMessageRepository.GetAllAsync();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactMessage>> GetContactMessage(Guid id)
        {
            var message = await _contactMessageRepository.GetByIdAsync(id);
            if (message == null) return NotFound();
            return Ok(message);
        }

        // Public endpoint to send contact messages
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ContactMessage>> CreateContactMessage(CreateContactMessageRequest request)
        {
            var message = new ContactMessage
            {
                FullName = request.FullName,
                Email = request.Email,
                Message = request.Message
            };

            await _contactMessageRepository.CreateAsync(message);
            return CreatedAtAction(nameof(GetContactMessage), new { id = message.Id }, message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContactMessage(Guid id, ContactMessage message)
        {
            if (id != message.Id) return BadRequest();

            var existing = await _contactMessageRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _contactMessageRepository.UpdateAsync(message);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactMessage(Guid id)
        {
            var existing = await _contactMessageRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _contactMessageRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
