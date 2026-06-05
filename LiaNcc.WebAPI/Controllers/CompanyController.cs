using LiaNcc.Models.DTOs.Dashboard;
using LiaNcc.Models.Entities;
using LiaNcc.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiaNcc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly LiaNcc.WebAPI.Services.IApplicationLoggerService _logger;

        public CompanyController(ICompanyRepository companyRepository, LiaNcc.WebAPI.Services.IApplicationLoggerService logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<CompanyProfile>> GetCompany([FromQuery] string? culture)
        {
            var profile = await _companyRepository.GetCompanyProfileAsync();
            if (profile == null)
            {
                // Return an empty profile if not found to avoid 404 in BO
                var newProfile = new CompanyProfile { Name = "LiaNcc" };
                return Ok(newProfile);
            }

            if (!string.IsNullOrEmpty(culture))
            {
                await LocalizeCompanyProfile(profile, culture);
            }

            return Ok(profile);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyProfile>> GetCompanyById(Guid id, [FromQuery] string? culture)
        {
            var profile = await _companyRepository.GetCompanyProfileAsync();
            if (profile == null)
            {
                var newProfile = new CompanyProfile { Id = id, Name = "LiaNcc" };
                return Ok(newProfile);
            }

            if (!string.IsNullOrEmpty(culture))
            {
                await LocalizeCompanyProfile(profile, culture);
            }

            return Ok(profile);
        }

        private async Task LocalizeCompanyProfile(CompanyProfile profile, string culture)
        {
            var localizationRepository = HttpContext.RequestServices.GetRequiredService<ILocalizedContentRepository>();
            var resolver = HttpContext.RequestServices.GetRequiredService<LiaNcc.WebAPI.Helpers.ILocalizationResolver>();

            var translations = await localizationRepository.GetByEntityAsync("CompanyProfile", profile.Id, culture);
            profile.AboutTitle = resolver.Resolve(translations, "AboutTitle", profile.AboutTitle ?? "", culture);
            profile.AboutDescription = resolver.Resolve(translations, "AboutDescription", profile.AboutDescription ?? "", culture);
        }

        [AllowAnonymous]
        [HttpGet("contacts")]
        public async Task<ActionResult<IEnumerable<CompanyContactDto>>> GetContacts()
        {
            try
            {
                var contacts = await _companyRepository.GetCompanyContactsAsync();

                var result = contacts.Select(c => new CompanyContactDto
                {
                    Id = c.Id,
                    CompanyId = c.CompanyId,
                    Type = c.Type,
                    Value = c.Value,
                    IsPrimary = c.IsPrimary,
                    SortOrder = c.SortOrder,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    "Company",
                    "GetContacts",
                    "Errore durante il recupero dei contatti aziendali",
                    ex,
                    500,
                    null,
                    "CompanyContact");

                return StatusCode(500, new
                {
                    message = "Errore durante il recupero dei contatti aziendali."
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult<CompanyProfile>> UpdateCompany(CompanyProfile profile)
        {
            var updated = await _companyRepository.CreateOrUpdateProfileAsync(profile);
            await _logger.LogInformationAsync("Company", "UpdateProfile", "Company profile updated", "Company", "CompanyProfile", updated.Id);
            return Ok(updated);
        }

        [HttpPost("contacts")]
        public async Task<ActionResult<CompanyContact>> CreateContact(CompanyContact contact)
        {
            await _companyRepository.CreateContactAsync(contact);
            await _logger.LogInformationAsync("Company", "CreateContact", $"Contact {contact.Type} added", "Company", "CompanyContact", contact.Id);
            return Ok(contact);
        }

        [HttpPut("contacts/{id}")]
        public async Task<IActionResult> UpdateContact(Guid id, CompanyContact contact)
        {
            if (id != contact.Id) return BadRequest();
            await _companyRepository.UpdateContactAsync(contact);
            await _logger.LogInformationAsync("Company", "UpdateContact", $"Contact {contact.Type} updated", "Company", "CompanyContact", contact.Id);
            return NoContent();
        }

        [HttpDelete("contacts/{id}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            await _companyRepository.DeleteContactAsync(id);
            await _logger.LogInformationAsync("Company", "DeleteContact", $"Contact {id} deleted", "Company", "CompanyContact", id);
            return NoContent();
        }
    }
}
