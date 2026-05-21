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
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<CompanyProfile>> GetCompany()
        {
            var profile = await _companyRepository.GetCompanyProfileAsync();
            if (profile == null)
            {
                // Return an empty profile if not found to avoid 404 in BO
                return Ok(new CompanyProfile { Name = "LiaNcc" });
            }
            return Ok(profile);
        }

        [AllowAnonymous]
        [HttpGet("contacts")]
        public async Task<ActionResult<IEnumerable<CompanyContact>>> GetContacts()
        {
            return Ok(await _companyRepository.GetCompanyContactsAsync());
        }

        [HttpPut]
        public async Task<ActionResult<CompanyProfile>> UpdateCompany(CompanyProfile profile)
        {
            var updated = await _companyRepository.CreateOrUpdateProfileAsync(profile);
            return Ok(updated);
        }

        [HttpPost("contacts")]
        public async Task<ActionResult<CompanyContact>> CreateContact(CompanyContact contact)
        {
            await _companyRepository.CreateContactAsync(contact);
            return Ok(contact);
        }

        [HttpPut("contacts/{id}")]
        public async Task<IActionResult> UpdateContact(Guid id, CompanyContact contact)
        {
            if (id != contact.Id) return BadRequest();
            await _companyRepository.UpdateContactAsync(contact);
            return NoContent();
        }

        [HttpDelete("contacts/{id}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            await _companyRepository.DeleteContactAsync(id);
            return NoContent();
        }
    }
}
