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
    [Route("api/call-to-actions")]
    [Authorize]
    public class CallToActionsController : ControllerBase
    {
        private readonly ICallToActionRepository _callToActionRepository;
        private readonly LiaNcc.WebAPI.Services.IApplicationLoggerService _logger;

        public CallToActionsController(ICallToActionRepository callToActionRepository, LiaNcc.WebAPI.Services.IApplicationLoggerService logger)
        {
            _callToActionRepository = callToActionRepository;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CallToAction>>> GetCallToActions()
        {
            return Ok(await _callToActionRepository.GetActiveAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<CallToAction>> GetCallToAction(Guid id)
        {
            var cta = await _callToActionRepository.GetByIdAsync(id);
            if (cta == null) return NotFound();
            return Ok(cta);
        }

        [AllowAnonymous]
        [HttpGet("page/{pageId}")]
        public async Task<ActionResult<IEnumerable<CallToAction>>> GetByPage(Guid pageId)
        {
            return Ok(await _callToActionRepository.GetByPageAsync(pageId));
        }

        [AllowAnonymous]
        [HttpGet("section/{sectionId}")]
        public async Task<ActionResult<IEnumerable<CallToAction>>> GetBySection(Guid sectionId)
        {
            return Ok(await _callToActionRepository.GetBySectionAsync(sectionId));
        }

        [HttpPost]
        public async Task<ActionResult<CallToAction>> CreateCallToAction(CallToAction cta)
        {
            await _callToActionRepository.CreateAsync(cta);
            return Ok(cta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCallToAction(Guid id, CallToAction cta)
        {
            if (id != cta.Id) return BadRequest();
            await _callToActionRepository.UpdateAsync(cta);
            await _logger.LogInfoAsync("CMS", "UpdateCTA", $"CTA {cta.Label} updated", id, "CallToAction");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCallToAction(Guid id)
        {
            await _callToActionRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
