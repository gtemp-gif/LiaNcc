using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/localization")]
    [Authorize]
    public class LocalizedContentsController : ControllerBase
    {
        private readonly ILocalizedContentRepository _localizedContentRepository;

        public LocalizedContentsController(ILocalizedContentRepository localizedContentRepository)
        {
            _localizedContentRepository = localizedContentRepository;
        }

        [AllowAnonymous]
        [HttpGet("entity/{entityName}/{entityId}")]
        public async Task<ActionResult<IEnumerable<LocalizedContent>>> GetByEntity(string entityName, Guid entityId, [FromQuery] string? language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return Ok(await _localizedContentRepository.GetByEntityAsync(entityName, entityId));
            }
            return Ok(await _localizedContentRepository.GetByEntityAsync(entityName, entityId, language));
        }

        [AllowAnonymous]
        [HttpGet("static/{contentKey}")]
        public async Task<ActionResult<LocalizedContent>> GetStaticContent(string contentKey, [FromQuery] string language = "it")
        {
            var content = await _localizedContentRepository.GetStaticContentAsync(contentKey, language);
            if (content == null) return NotFound();
            return Ok(content);
        }

        [AllowAnonymous]
        [HttpGet("static")]
        public async Task<ActionResult<IEnumerable<LocalizedContent>>> GetStaticContents([FromQuery] string language = "it")
        {
            return Ok(await _localizedContentRepository.GetStaticContentsAsync(language));
        }

        [HttpPost]
        public async Task<ActionResult<LocalizedContent>> CreateLocalizedContent(LocalizedContent content)
        {
            await _localizedContentRepository.CreateAsync(content);
            return Ok(content);
        }

        [HttpPost("upsert-batch")]
        public async Task<IActionResult> UpsertBatch(IEnumerable<LocalizedContent> items)
        {
            await _localizedContentRepository.UpsertBatchAsync(items);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocalizedContent(Guid id, LocalizedContent content)
        {
            if (id != content.Id) return BadRequest();
            await _localizedContentRepository.UpdateAsync(content);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocalizedContent(Guid id)
        {
            await _localizedContentRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
