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
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly LiaNcc.WebAPI.Services.IApplicationLoggerService _logger;

        public PartnersController(IPartnerRepository partnerRepository, LiaNcc.WebAPI.Services.IApplicationLoggerService logger)
        {
            _partnerRepository = partnerRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Partner>>> GetPartners()
        {
            return Ok(await _partnerRepository.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Partner>>> GetActivePartners()
        {
            return Ok(await _partnerRepository.GetActivePartnersAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Partner>> GetPartner(Guid id)
        {
            var partner = await _partnerRepository.GetByIdAsync(id);
            if (partner == null) return NotFound();
            return Ok(partner);
        }

        [HttpPost]
        public async Task<ActionResult<Partner>> CreatePartner(Partner partner)
        {
            await _partnerRepository.CreateAsync(partner);
            await _logger.LogInformationAsync("Partners", "CreatePartner", $"Partner {partner.Name} created", "Partners", "Partner", partner.Id);
            return Ok(partner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartner(Guid id, Partner partner)
        {
            if (id != partner.Id) return BadRequest();
            await _partnerRepository.UpdateAsync(partner);
            await _logger.LogInformationAsync("Partners", "UpdatePartner", $"Partner {partner.Name} updated", "Partners", "Partner", partner.Id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartner(Guid id)
        {
            await _partnerRepository.DeleteAsync(id);
            await _logger.LogInformationAsync("Partners", "DeletePartner", $"Partner {id} deleted", "Partners", "Partner", id);
            return NoContent();
        }
    }
}
