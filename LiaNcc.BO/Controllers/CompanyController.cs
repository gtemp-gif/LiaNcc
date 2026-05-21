using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ICompanyApiClient _companyApiClient;
        private readonly ILanguagesApiClient _languagesApiClient;
        private readonly ILocalizedContentsApiClient _localizedContentsApiClient;

        private readonly System.Collections.Generic.List<string> _translatableKeys = new System.Collections.Generic.List<string> { "Name", "Address" };

        public CompanyController(
            ICompanyApiClient companyApiClient,
            ILanguagesApiClient languagesApiClient,
            ILocalizedContentsApiClient localizedContentsApiClient)
        {
            _companyApiClient = companyApiClient;
            _languagesApiClient = languagesApiClient;
            _localizedContentsApiClient = localizedContentsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var company = await _companyApiClient.GetFirstCompanyProfileAsync();
            if (company == null)
            {
                return RedirectToAction(nameof(Create));
            }
            return View("Details", company);
        }

        public IActionResult Create()
        {
            return View(new CompanyProfile());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyProfile profile)
        {
            if (ModelState.IsValid)
            {
                await _companyApiClient.CreateAsync(profile);
                TempData["SuccessMessage"] = "Profilo aziendale salvato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var profile = await _companyApiClient.GetByIdAsync(id);
            if (profile == null) return NotFound();

            var model = new LiaNcc.BO.Models.Company.CompanyProfileViewModel
            {
                Id = profile.Id,
                Name = profile.Name,
                VatNumber = profile.VatNumber,
                Address = profile.Address,
                City = profile.City,
                ZipCode = profile.ZipCode,
                Country = profile.Country
            };

            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "CompanyProfile", id, _translatableKeys);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LiaNcc.BO.Models.Company.CompanyProfileViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var profile = await _companyApiClient.GetByIdAsync(id);
                if (profile == null) return NotFound();

                profile.Name = model.Name;
                profile.VatNumber = model.VatNumber;
                profile.Address = model.Address;
                profile.City = model.City;
                profile.ZipCode = model.ZipCode;
                profile.Country = model.Country;

                await _companyApiClient.UpdateAsync(id, profile);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "CompanyProfile", id);

                TempData["SuccessMessage"] = "Profilo aziendale aggiornato.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
