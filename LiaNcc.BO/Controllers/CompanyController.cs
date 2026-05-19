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

        public CompanyController(ICompanyApiClient companyApiClient)
        {
            _companyApiClient = companyApiClient;
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
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CompanyProfile profile)
        {
            if (id != profile.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _companyApiClient.UpdateAsync(id, profile);
                TempData["SuccessMessage"] = "Profilo aziendale aggiornato.";
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }
    }
}
