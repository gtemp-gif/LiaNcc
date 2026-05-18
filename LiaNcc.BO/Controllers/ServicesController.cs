using System;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class ServicesController : BaseController
    {
        private readonly IServicesApiClient _servicesApiClient;

        public ServicesController(IServicesApiClient servicesApiClient)
        {
            _servicesApiClient = servicesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _servicesApiClient.GetAllAsync();
            return View(services);
        }

        public IActionResult Create()
        {
            return View(new Service());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelState.IsValid)
            {
                await _servicesApiClient.CreateAsync(service);
                TempData["SuccessMessage"] = "Servizio creato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var service = await _servicesApiClient.GetByIdAsync(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Service service)
        {
            if (id != service.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _servicesApiClient.UpdateAsync(id, service);
                TempData["SuccessMessage"] = "Servizio aggiornato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _servicesApiClient.DeleteAsync(id);
            TempData["SuccessMessage"] = "Servizio eliminato con successo.";
            return RedirectToAction(nameof(Index));
        }
    }
}
