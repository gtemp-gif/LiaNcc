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
    [Route("api/page-sections")]
    [Authorize(Roles = "Admin,Operator")]
    public class PageSectionsController : ControllerBase
    {
        private readonly IPageSectionRepository _pageSectionRepository;

        public PageSectionsController(IPageSectionRepository pageSectionRepository)
        {
            _pageSectionRepository = pageSectionRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PageSection>>> GetPageSections()
        {
            return Ok(await _pageSectionRepository.GetActiveSectionsAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<PageSection>> GetPageSection(Guid id)
        {
            var section = await _pageSectionRepository.GetByIdAsync(id);
            if (section == null) return NotFound();
            return Ok(section);
        }

        [AllowAnonymous]
        [HttpGet("page/{pageId}")]
        public async Task<ActionResult<IEnumerable<PageSection>>> GetPageSectionsByPage(Guid pageId)
        {
            return Ok(await _pageSectionRepository.GetSectionsByPageAsync(pageId));
        }

        [HttpPost]
        public async Task<ActionResult<PageSection>> CreatePageSection(PageSection pageSection)
        {
            await _pageSectionRepository.CreateAsync(pageSection);
            return Ok(pageSection);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePageSection(Guid id, PageSection pageSection)
        {
            if (id != pageSection.Id) return BadRequest();
            await _pageSectionRepository.UpdateAsync(pageSection);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePageSection(Guid id)
        {
            await _pageSectionRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
