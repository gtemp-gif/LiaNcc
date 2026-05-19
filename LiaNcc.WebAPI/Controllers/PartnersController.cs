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

        public PartnersController(IPartnerRepository partnerRepository)
        {
            _partnerRepository = partnerRepository;
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
            return Ok(partner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartner(Guid id, Partner partner)
        {
            if (id != partner.Id) return BadRequest();
            await _partnerRepository.UpdateAsync(partner);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartner(Guid id)
        {
            await _partnerRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
