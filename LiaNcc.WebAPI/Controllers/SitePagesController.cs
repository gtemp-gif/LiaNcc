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
    [Route("api/site-pages")]
    [Authorize(Roles = "Admin,Operator")]
    public class SitePagesController : ControllerBase
    {
        private readonly ISitePageRepository _sitePageRepository;

        public SitePagesController(ISitePageRepository sitePageRepository)
        {
            _sitePageRepository = sitePageRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SitePage>>> GetSitePages()
        {
            return Ok(await _sitePageRepository.GetActivePagesAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<SitePage>> GetSitePage(Guid id)
        {
            var page = await _sitePageRepository.GetByIdAsync(id);
            if (page == null) return NotFound();
            return Ok(page);
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<SitePage>> GetSitePageBySlug(string slug)
        {
            var page = await _sitePageRepository.GetBySlugAsync(slug);
            if (page == null) return NotFound();
            return Ok(page);
        }

        [AllowAnonymous]
        [HttpGet("{id}/full")]
        public async Task<ActionResult<SitePage>> GetSitePageFull(Guid id)
        {
            var page = await _sitePageRepository.GetPageWithSectionsAsync(id);
            if (page == null) return NotFound();
            return Ok(page);
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}/full")]
        public async Task<ActionResult<SitePage>> GetSitePageFullBySlug(string slug)
        {
            var page = await _sitePageRepository.GetPageWithSectionsBySlugAsync(slug);
            if (page == null) return NotFound();
            return Ok(page);
        }

        [HttpPost]
        public async Task<ActionResult<SitePage>> CreateSitePage(SitePage sitePage)
        {
            await _sitePageRepository.CreateAsync(sitePage);
            return Ok(sitePage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSitePage(Guid id, SitePage sitePage)
        {
            if (id != sitePage.Id) return BadRequest();
            await _sitePageRepository.UpdateAsync(sitePage);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSitePage(Guid id)
        {
            await _sitePageRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
