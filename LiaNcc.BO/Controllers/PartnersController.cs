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

        public PartnersController(IPartnersApiClient partnersApiClient)
        {
            _partnersApiClient = partnersApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var partners = await _partnersApiClient.GetAllAsync();
            return View(partners);
        }

        public IActionResult Create()
        {
            return View(new Partner());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Partner partner)
        {
            if (ModelState.IsValid)
            {
                await _partnersApiClient.CreateAsync(partner);
                TempData["SuccessMessage"] = "Partner creato.";
                return RedirectToAction(nameof(Index));
            }
            return View(partner);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var partner = await _partnersApiClient.GetByIdAsync(id);
            if (partner == null) return NotFound();
            return View(partner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Partner partner)
        {
            if (id != partner.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _partnersApiClient.UpdateAsync(id, partner);
                TempData["SuccessMessage"] = "Partner aggiornato.";
                return RedirectToAction(nameof(Index));
            }
            return View(partner);
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
