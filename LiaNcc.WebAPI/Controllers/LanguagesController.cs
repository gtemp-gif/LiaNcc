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
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguagesController(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
            return Ok(await _languageRepository.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("default")]
        public async Task<ActionResult<Language>> GetDefaultLanguage()
        {
            var language = await _languageRepository.GetDefaultLanguageAsync();
            if (language == null) return NotFound();
            return Ok(language);
        }

        [AllowAnonymous]
        [HttpGet("{code}")]
        public async Task<ActionResult<Language>> GetLanguage(string code)
        {
            var language = await _languageRepository.GetByCodeAsync(code);
            if (language == null) return NotFound();
            return Ok(language);
        }
    }
}
