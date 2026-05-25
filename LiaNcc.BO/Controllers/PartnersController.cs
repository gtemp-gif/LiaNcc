using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Controllers
{
    public class PartnersController : BaseController
    {
        private readonly IPartnersApiClient _partnersApiClient;
        private readonly ILanguagesApiClient _languagesApiClient;
        private readonly ILocalizedContentsApiClient _localizedContentsApiClient;

        private readonly List<string> _translatableKeys = new List<string> { "Name" };

        private readonly IApplicationLoggerService _logger;

        public PartnersController(
            IPartnersApiClient partnersApiClient,
            IApplicationLoggerService applicationLogger,
            ILanguagesApiClient languagesApiClient,
            ILocalizedContentsApiClient localizedContentsApiClient)
        {
            _partnersApiClient = partnersApiClient;
            _logger = applicationLogger;
            _languagesApiClient = languagesApiClient;
            _localizedContentsApiClient = localizedContentsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var partners = await _partnersApiClient.GetAllAsync();
            return View(partners);
        }

        public async Task<IActionResult> Create()
        {
            var model = new LiaNcc.BO.Models.Partners.PartnerViewModel();
            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Partner", null, _translatableKeys);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LiaNcc.BO.Models.Partners.PartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var partner = new Partner
                {
                    Name = model.Name,
                    WebsiteUrl = model.WebsiteUrl,
                    LogoUrl = model.LogoUrl,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder
                };
                var createdPartner = await _partnersApiClient.CreateAsync(partner);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Partner", createdPartner.Id);
                TempData["SuccessMessage"] = "Partner creato.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var partner = await _partnersApiClient.GetByIdAsync(id);
            if (partner == null) return NotFound();

            var model = new LiaNcc.BO.Models.Partners.PartnerViewModel
            {
                Id = partner.Id,
                Name = partner.Name,
                WebsiteUrl = partner.WebsiteUrl,
                LogoUrl = partner.LogoUrl,
                IsActive = partner.IsActive,
                SortOrder = partner.SortOrder
            };

            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Partner", id, _translatableKeys);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LiaNcc.BO.Models.Partners.PartnerViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var partner = await _partnersApiClient.GetByIdAsync(id);
                if (partner == null) return NotFound();

                partner.Name = model.Name;
                partner.WebsiteUrl = model.WebsiteUrl;
                partner.LogoUrl = model.LogoUrl;
                partner.IsActive = model.IsActive;
                partner.SortOrder = model.SortOrder;

                await _partnersApiClient.UpdateAsync(id, partner);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Partner", id);
                await _logger.LogInfoAsync("Partners", "UpdatePartner", $"Partner {partner.Name} updated via BO", id, "Partner");
                TempData["SuccessMessage"] = "Partner aggiornato.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _partnersApiClient.DeleteAsync(id);
            TempData["SuccessMessage"] = "Partner eliminato.";
            return RedirectToAction(nameof(Index));
        }
    }
}
