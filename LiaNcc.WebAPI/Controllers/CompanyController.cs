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
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [AllowAnonymous]
        [HttpGet("profile")]
        public async Task<ActionResult<CompanyProfile>> GetProfile()
        {
            var profile = await _companyRepository.GetCompanyProfileAsync();
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [AllowAnonymous]
        [HttpGet("contacts")]
        public async Task<ActionResult<IEnumerable<CompanyContact>>> GetContacts()
        {
            return Ok(await _companyRepository.GetCompanyContactsAsync());
        }

        [HttpPut("profile")]
        public async Task<ActionResult<CompanyProfile>> UpdateProfile(CompanyProfile profile)
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
