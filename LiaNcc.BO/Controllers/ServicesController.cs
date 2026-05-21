using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Models.Services;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class ServicesController : BaseController
    {
        private readonly IServicesApiClient _servicesApiClient;
        private readonly IFilesApiClient _filesApiClient;
        private readonly ILanguagesApiClient _languagesApiClient;
        private readonly ILocalizedContentsApiClient _localizedContentsApiClient;

        private readonly List<string> _translatableKeys = new List<string> { "Name", "Description" };

        public ServicesController(
            IServicesApiClient servicesApiClient,
            IFilesApiClient filesApiClient,
            ILanguagesApiClient languagesApiClient,
            ILocalizedContentsApiClient localizedContentsApiClient)
        {
            _servicesApiClient = servicesApiClient;
            _filesApiClient = filesApiClient;
            _languagesApiClient = languagesApiClient;
            _localizedContentsApiClient = localizedContentsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _servicesApiClient.GetAllAsync();
            return View(services);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ServiceViewModel();
            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Service", null, _translatableKeys);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Name = model.Name,
                    Icon = model.Icon,
                    IsFeatured = model.IsFeatured,
                    IsBookable = model.IsBookable,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder,
                    Description = model.Description
                };

                var createdService = await _servicesApiClient.CreateAsync(service);

                if (model.CoverImageFile != null)
                {
                    var uploadResponse = await _filesApiClient.UploadFilesAsync(
                        new List<Microsoft.AspNetCore.Http.IFormFile> { model.CoverImageFile },
                        "services",
                        "Services",
                        createdService.Id,
                        "Cover");

                    if (uploadResponse?.UploadedFiles.Count > 0)
                    {
                        createdService.CoverImageUrl = uploadResponse.UploadedFiles[0].Url;
                        await _servicesApiClient.UpdateAsync(createdService.Id, createdService);
                    }
                }

                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Service", createdService.Id);

                TempData["SuccessMessage"] = "Servizio creato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var service = await _servicesApiClient.GetByIdAsync(id);
            if (service == null) return NotFound();

            var model = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Icon = service.Icon,
                IsFeatured = service.IsFeatured,
                IsBookable = service.IsBookable,
                IsActive = service.IsActive,
                CoverImageUrl = service.CoverImageUrl,
                SortOrder = service.SortOrder,
                Description = service.Description
            };

            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Service", id, _translatableKeys);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ServiceViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var service = await _servicesApiClient.GetByIdAsync(id);
                if (service == null) return NotFound();

                service.Name = model.Name;
                service.Icon = model.Icon;
                service.IsFeatured = model.IsFeatured;
                service.IsBookable = model.IsBookable;
                service.IsActive = model.IsActive;
                service.SortOrder = model.SortOrder;
                service.Description = model.Description;

                if (model.CoverImageFile != null)
                {
                    var uploadResponse = await _filesApiClient.UploadFilesAsync(
                        new List<Microsoft.AspNetCore.Http.IFormFile> { model.CoverImageFile },
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
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Service", id);

                TempData["SuccessMessage"] = "Servizio aggiornato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
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
