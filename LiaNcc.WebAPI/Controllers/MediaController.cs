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
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Operator")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaRepository _mediaRepository;

        public MediaController(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MediaAsset>>> GetMedia()
        {
            return Ok(await _mediaRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MediaAsset>> GetMedia(Guid id)
        {
            var media = await _mediaRepository.GetByIdAsync(id);
            if (media == null) return NotFound();
            return Ok(media);
        }

        [AllowAnonymous]
        [HttpGet("entity/{entityName}/{entityId}")]
        public async Task<ActionResult<IEnumerable<EntityMedia>>> GetMediaForEntity(string entityName, Guid entityId)
        {
            return Ok(await _mediaRepository.GetMediaForEntityAsync(entityName, entityId));
        }

        [HttpPost]
        public async Task<ActionResult<MediaAsset>> CreateMedia(MediaAsset mediaAsset)
        {
            await _mediaRepository.CreateAsync(mediaAsset);
            return Ok(mediaAsset);
        }

        [HttpPost("entity")]
        public async Task<ActionResult<EntityMedia>> AddMediaToEntity(EntityMedia entityMedia)
        {
            await _mediaRepository.AddMediaToEntityAsync(entityMedia);
            return Ok(entityMedia);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedia(Guid id)
        {
            await _mediaRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("entity/{id}")]
        public async Task<IActionResult> RemoveMediaFromEntity(Guid id)
        {
            await _mediaRepository.RemoveMediaFromEntityAsync(id);
            return NoContent();
        }
    }
}
