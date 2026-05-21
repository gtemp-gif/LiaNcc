using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Models.Vehicles;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.DTOs.Vehicles;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly IVehiclesApiClient _vehiclesApiClient;
        private readonly IFilesApiClient _filesApiClient;
        private readonly ILanguagesApiClient _languagesApiClient;
        private readonly ILocalizedContentsApiClient _localizedContentsApiClient;

        private readonly List<string> _translatableKeys = new List<string> { "Name", "Title", "Description" };

        public VehiclesController(
            IVehiclesApiClient vehiclesApiClient,
            IFilesApiClient filesApiClient,
            ILanguagesApiClient languagesApiClient,
            ILocalizedContentsApiClient localizedContentsApiClient)
        {
            _vehiclesApiClient = vehiclesApiClient;
            _filesApiClient = filesApiClient;
            _languagesApiClient = languagesApiClient;
            _localizedContentsApiClient = localizedContentsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehiclesApiClient.GetAllVehiclesAsync();
            return View(vehicles);
        }

        public async Task<IActionResult> Create()
        {
            var model = new VehicleViewModel
            {
                AvailableCategories = (await _vehiclesApiClient.GetActiveCategoriesAsync()).ToList(),
                IsActive = true,
                IsBookable = true
            };
            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Vehicle", null, _translatableKeys);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleViewModel model, List<string> FeatureNames, List<string> FeatureIcons, List<int> FeatureSortOrders)
        {
            if (ModelState.IsValid)
            {
                var request = new VehicleUpsertRequest
                {
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    Title = model.Title,
                    Description = model.Description,
                    Seats = model.Seats,
                    Luggages = model.Luggages,
                    IsFeatured = model.IsFeatured,
                    IsBookable = model.IsBookable,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder,
                    Features = new List<VehicleFeatureDto>()
                };

                for (int i = 0; i < FeatureNames.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(FeatureNames[i]))
                    {
                        request.Features.Add(new VehicleFeatureDto
                        {
                            Name = FeatureNames[i],
                            Icon = FeatureIcons.Count > i ? FeatureIcons[i] : null,
                            SortOrder = FeatureSortOrders.Count > i ? FeatureSortOrders[i] : 0
                        });
                    }
                }

                var vehicle = await _vehiclesApiClient.CreateVehicleAsync(request);

                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Vehicle", vehicle.Id);

                if (model.NewGalleryImages != null && model.NewGalleryImages.Count > 0)
                {
                    await _filesApiClient.UploadFilesAsync(model.NewGalleryImages, "vehicles", "Vehicles", vehicle.Id, "Gallery");
                }

                TempData["SuccessMessage"] = "Veicolo creato con successo.";
                return RedirectToAction(nameof(Index));
            }

            model.AvailableCategories = (await _vehiclesApiClient.GetActiveCategoriesAsync()).ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var vehicle = await _vehiclesApiClient.GetVehicleByIdAsync(id);
            if (vehicle == null) return NotFound();

            var model = new VehicleViewModel
            {
                Id = vehicle.Id,
                CategoryId = vehicle.CategoryId,
                Name = vehicle.Name,
                Title = vehicle.Title,
                Description = vehicle.Description,
                Seats = vehicle.Seats,
                Luggages = vehicle.Luggages,
                IsFeatured = vehicle.IsFeatured,
                IsBookable = vehicle.IsBookable,
                IsActive = vehicle.IsActive,
                SortOrder = vehicle.SortOrder,
                AvailableCategories = (await _vehiclesApiClient.GetActiveCategoriesAsync()).ToList(),
                Features = vehicle.Features,
                ExistingGalleryImages = vehicle.GalleryImages
            };

            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Vehicle", id, _translatableKeys);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VehicleViewModel model, List<string> FeatureNames, List<string> FeatureIcons, List<int> FeatureSortOrders)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var request = new VehicleUpsertRequest
                {
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    Title = model.Title,
                    Description = model.Description,
                    Seats = model.Seats,
                    Luggages = model.Luggages,
                    IsFeatured = model.IsFeatured,
                    IsBookable = model.IsBookable,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder,
                    Features = new List<VehicleFeatureDto>()
                };

                for (int i = 0; i < FeatureNames.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(FeatureNames[i]))
                    {
                        request.Features.Add(new VehicleFeatureDto
                        {
                            Name = FeatureNames[i],
                            Icon = FeatureIcons.Count > i ? FeatureIcons[i] : null,
                            SortOrder = FeatureSortOrders.Count > i ? FeatureSortOrders[i] : 0
                        });
                    }
                }

                await _vehiclesApiClient.UpdateVehicleAsync(id, request);

                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Vehicle", id);

                if (model.NewGalleryImages != null && model.NewGalleryImages.Count > 0)
                {
                    await _filesApiClient.UploadFilesAsync(model.NewGalleryImages, "vehicles", "Vehicles", id, "Gallery");
                }

                TempData["SuccessMessage"] = "Veicolo aggiornato con successo.";
                return RedirectToAction(nameof(Index));
            }

            model.AvailableCategories = (await _vehiclesApiClient.GetActiveCategoriesAsync()).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _vehiclesApiClient.DeleteVehicleAsync(id);
            TempData["SuccessMessage"] = "Veicolo eliminato con successo.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGalleryImage(Guid imageId)
        {
            await _vehiclesApiClient.DeleteGalleryImageAsync(imageId);
            return Ok();
        }
    }
}
