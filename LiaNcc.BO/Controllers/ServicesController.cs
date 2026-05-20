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
        private readonly IFilesApiClient _filesApiClient;

        public ServicesController(IServicesApiClient servicesApiClient, IFilesApiClient filesApiClient)
        {
            _servicesApiClient = servicesApiClient;
            _filesApiClient = filesApiClient;
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
        public async Task<IActionResult> Create(Service service, Microsoft.AspNetCore.Http.IFormFile? coverImageFile)
        {
            if (ModelState.IsValid)
            {
                // 1. Create the service first to get its ID
                var createdService = await _servicesApiClient.CreateAsync(service);

                // 2. If a cover image is provided, upload it and associate it with the created service
                if (coverImageFile != null)
                {
                    var uploadResponse = await _filesApiClient.UploadFilesAsync(
                        new System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile> { coverImageFile },
                        "services",
                        "Services",
                        createdService.Id,
                        "Cover");

                    if (uploadResponse?.UploadedFiles.Count > 0)
                    {
                        // 3. Update the service with the returned URL
                        createdService.CoverImageUrl = uploadResponse.UploadedFiles[0].Url;
                        await _servicesApiClient.UpdateAsync(createdService.Id, createdService);
                    }
                }

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
        public async Task<IActionResult> Edit(Guid id, Service service, Microsoft.AspNetCore.Http.IFormFile? coverImageFile)
        {
            if (id != service.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (coverImageFile != null)
                {
                    var uploadResponse = await _filesApiClient.UploadFilesAsync(
                        new System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile> { coverImageFile },
                        "services",
                        "Services",
                        id,
                        "Cover");

                    if (uploadResponse?.UploadedFiles.Count > 0)
                    {
                        service.CoverImageUrl = uploadResponse.UploadedFiles[0].Url;
                    }
                }

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
