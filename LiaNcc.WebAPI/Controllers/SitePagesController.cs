using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/site-pages")]
    ///[Authorize(Roles = "Admin,Operator")]
    public class SitePagesController : ControllerBase
    {
        private readonly ISitePageRepository _sitePageRepository;
        private readonly ILocalizedContentRepository _localizationRepository;
        private readonly LiaNcc.WebAPI.Helpers.ILocalizationResolver _resolver;
        private readonly LiaNcc.WebAPI.Services.IApplicationLoggerService _logger;

        public SitePagesController(
            ISitePageRepository sitePageRepository,
            ILocalizedContentRepository localizationRepository,
            LiaNcc.WebAPI.Helpers.ILocalizationResolver resolver,
            LiaNcc.WebAPI.Services.IApplicationLoggerService logger)
        {
            _sitePageRepository = sitePageRepository;
            _localizationRepository = localizationRepository;
            _resolver = resolver;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SitePage>>> GetSitePages([FromQuery] string? culture)
        {
            var pages = await _sitePageRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(culture))
            {
                foreach (var page in pages) await LocalizePage(page, culture, false);
            }
            return Ok(pages);
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SitePage>>> GetActiveSitePages()
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
        public async Task<ActionResult<SitePage>> GetSitePageFull(Guid id, [FromQuery] string? culture)
        {
            var page = await _sitePageRepository.GetPageWithSectionsAsync(id);
            if (page == null) return NotFound();
            if (!string.IsNullOrEmpty(culture)) await LocalizePage(page, culture, true);
            return Ok(page);
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}/full")]
        public async Task<ActionResult<SitePage>> GetSitePageFullBySlug(string slug, [FromQuery] string? culture)
        {
            var page = await _sitePageRepository.GetPageWithSectionsBySlugAsync(slug);
            if (page == null) return NotFound();
            if (!string.IsNullOrEmpty(culture)) await LocalizePage(page, culture, true);
            return Ok(page);
        }

        private async Task LocalizePage(SitePage page, string culture, bool includeSections)
        {
            var translations = await _localizationRepository.GetByEntityAsync("SitePage", page.Id, culture);
            page.Name = _resolver.Resolve(translations, "Name", page.Name, culture);
            page.MetaTitle = _resolver.Resolve(translations, "MetaTitle", page.MetaTitle ?? "", culture);
            page.MetaDescription = _resolver.Resolve(translations, "MetaDescription", page.MetaDescription ?? "", culture);

            if (includeSections)
            {
                foreach (var section in page.PageSections)
                {
                    var sTranslations = await _localizationRepository.GetByEntityAsync("PageSection", section.Id, culture);
                    section.Name = _resolver.Resolve(sTranslations, "Name", section.Name, culture);
                    section.Title = _resolver.Resolve(sTranslations, "Title", section.Title ?? "", culture);
                    section.Description = _resolver.Resolve(sTranslations, "Description", section.Description ?? "", culture);

                    foreach (var cta in section.CallToActions)
                    {
                        var cTranslations = await _localizationRepository.GetByEntityAsync("CallToAction", cta.Id, culture);
                        cta.Label = _resolver.Resolve(cTranslations, "Label", cta.Label ?? "", culture);
                        cta.Title = _resolver.Resolve(cTranslations, "Title", cta.Title ?? "", culture);
                    }
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult<SitePage>> CreateSitePage(SitePage sitePage)
        {
            await _sitePageRepository.CreateAsync(sitePage);
            await _logger.LogInformationAsync("CMS", "CreatePage", $"Page {sitePage.Name} created", "CMS", "SitePage", sitePage.Id);
            return Ok(sitePage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSitePage(Guid id, SitePage sitePage)
        {
            if (id != sitePage.Id) return BadRequest();
            await _sitePageRepository.UpdateAsync(sitePage);
            await _logger.LogInformationAsync("CMS", "UpdatePage", $"Page {sitePage.Name} updated", "CMS", "SitePage", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSitePage(Guid id)
        {
            await _sitePageRepository.DeleteAsync(id);
            await _logger.LogInformationAsync("CMS", "DeletePage", $"Page {id} deleted", "CMS", "SitePage", id);
            return NoContent();
        }
    }
}
