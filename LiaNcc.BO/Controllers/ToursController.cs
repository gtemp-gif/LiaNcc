using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Models.Tours;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LiaNcc.BO.Controllers
{
    public class ToursController : BaseController
    {
        private readonly IToursApiClient _toursApiClient;
        private readonly IVehiclesApiClient _vehiclesApiClient;
        private readonly IFilesApiClient _filesApiClient;
        private readonly ILanguagesApiClient _languagesApiClient;
        private readonly ILocalizedContentsApiClient _localizedContentsApiClient;

        private readonly List<string> _translatableKeys = new List<string>
        {
            "Name", "Description", "HeroTitle", "HeroSubtitle",
            "ExperienceDescription", "MeetingPoint"
        };

        private readonly IApplicationLoggerService _logger;

        public ToursController(
            IToursApiClient toursApiClient,
            IApplicationLoggerService applicationLogger,
            IVehiclesApiClient vehiclesApiClient,
            IFilesApiClient filesApiClient,
            ILanguagesApiClient languagesApiClient,
            ILocalizedContentsApiClient localizedContentsApiClient)
        {
            _toursApiClient = toursApiClient;
            _logger = applicationLogger;
            _vehiclesApiClient = vehiclesApiClient;
            _filesApiClient = filesApiClient;
            _languagesApiClient = languagesApiClient;
            _localizedContentsApiClient = localizedContentsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var tours = await _toursApiClient.GetAllToursAsync();
            return View(tours);
        }

        public async Task<IActionResult> Create()
        {
            var model = new TourViewModel
            {
                AvailableCategories = await GetCategoriesSelectList(),
                AvailableVehicles = await GetVehiclesSelectList(),
                IsActive = true
            };
            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Tour", null, _translatableKeys);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TourViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.CoverImageFile != null)
                {
                    var upload = await _filesApiClient.UploadFilesAsync(new List<Microsoft.AspNetCore.Http.IFormFile> { model.CoverImageFile }, "tours", "Tours", null, "Cover");
                    if (upload?.UploadedFiles.Count > 0) model.CoverImageUrl = upload.UploadedFiles[0].Url;
                }

                if (model.ExperienceImageFile != null)
                {
                    var upload = await _filesApiClient.UploadFilesAsync(new List<Microsoft.AspNetCore.Http.IFormFile> { model.ExperienceImageFile }, "tours", "Tours", null, "Experience");
                    if (upload?.UploadedFiles.Count > 0) model.ExperienceImageUrl = upload.UploadedFiles[0].Url;
                }

                var tour = new Tour
                {
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    Slug = model.Slug,
                    Price = model.Price,
                    CoverImageUrl = model.CoverImageUrl,
                    Description = model.Description,
                    HeroTitle = model.HeroTitle,
                    HeroSubtitle = model.HeroSubtitle,
                    ExperienceDescription = model.ExperienceDescription,
                    ExperienceImageUrl = model.ExperienceImageUrl,
                    DurationDays = model.DurationDays,
                    DurationHours = model.DurationHours,
                    MeetingPoint = model.MeetingPoint,
                    VehicleId = model.VehicleId,
                    IsFeatured = model.IsFeatured,
                    IsBookable = model.IsBookable,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder
                };

                var createdTour = await _toursApiClient.CreateTourAsync(tour);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Tour", createdTour.Id);
                await _logger.LogInfoAsync("Tours", "CreateTour", $"Tour {createdTour.Name} created via BO", createdTour.Id, "Tour");

                // Gallery upload
                if (model.NewGalleryImages != null && model.NewGalleryImages.Any())
                {
                    var galleryUpload = await _filesApiClient.UploadFilesAsync(model.NewGalleryImages, "tours", "Tours", createdTour.Id, "Gallery");
                    if (galleryUpload == null || galleryUpload.UploadedFiles.Count == 0)
                    {
                        TempData["WarningMessage"] = "Tour creato, ma si è verificato un errore durante il caricamento della galleria.";
                    }
                }

                TempData["SuccessMessage"] = "Tour creato con successo.";
                return RedirectToAction(nameof(Index));
            }

            model.AvailableCategories = await GetCategoriesSelectList();
            model.AvailableVehicles = await GetVehiclesSelectList();
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var tour = await _toursApiClient.GetTourByIdAsync(id);
            if (tour == null) return NotFound();

            var model = new TourViewModel
            {
                Id = tour.Id,
                CategoryId = tour.CategoryId,
                Name = tour.Name,
                Slug = tour.Slug,
                Price = tour.Price,
                CoverImageUrl = tour.CoverImageUrl,
                Description = tour.Description,
                HeroTitle = tour.HeroTitle,
                HeroSubtitle = tour.HeroSubtitle,
                ExperienceDescription = tour.ExperienceDescription,
                ExperienceImageUrl = tour.ExperienceImageUrl,
                DurationDays = tour.DurationDays,
                DurationHours = tour.DurationHours,
                MeetingPoint = tour.MeetingPoint,
                VehicleId = tour.VehicleId,
                IsFeatured = tour.IsFeatured,
                IsBookable = tour.IsBookable,
                IsActive = tour.IsActive,
                SortOrder = tour.SortOrder,
                ExistingGalleryImages = tour.GalleryImages,
                AvailableCategories = await GetCategoriesSelectList(),
                AvailableVehicles = await GetVehiclesSelectList()
            };

            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "Tour", id, _translatableKeys);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TourViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                bool uploadError = false;
                try
                {
                    if (model.CoverImageFile != null)
                    {
                        var upload = await _filesApiClient.UploadFilesAsync(new List<Microsoft.AspNetCore.Http.IFormFile> { model.CoverImageFile }, "tours", "Tours", id, "Cover");
                        if (upload?.UploadedFiles.Count > 0) model.CoverImageUrl = upload.UploadedFiles[0].Url;
                    }

                    if (model.ExperienceImageFile != null)
                    {
                        var upload = await _filesApiClient.UploadFilesAsync(new List<Microsoft.AspNetCore.Http.IFormFile> { model.ExperienceImageFile }, "tours", "Tours", id, "Experience");
                        if (upload?.UploadedFiles.Count > 0) model.ExperienceImageUrl = upload.UploadedFiles[0].Url;
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogErrorAsync("Tours", "EditUpload", "Error uploading main images during tour edit", ex,null, id, "Tour");
                    uploadError = true;
                }

                var tour = new Tour
                {
                    Id = id,
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    Slug = model.Slug,
                    Price = model.Price,
                    CoverImageUrl = model.CoverImageUrl,
                    Description = model.Description,
                    HeroTitle = model.HeroTitle,
                    HeroSubtitle = model.HeroSubtitle,
                    ExperienceDescription = model.ExperienceDescription,
                    ExperienceImageUrl = model.ExperienceImageUrl,
                    DurationDays = model.DurationDays,
                    DurationHours = model.DurationHours,
                    MeetingPoint = model.MeetingPoint,
                    VehicleId = model.VehicleId,
                    IsFeatured = model.IsFeatured,
                    IsBookable = model.IsBookable,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder
                };

                await _toursApiClient.UpdateTourAsync(id, tour);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "Tour", id);
                await _logger.LogInfoAsync("Tours", "UpdateTour", $"Tour {tour.Name} updated via BO", id, "Tour");

                // Gallery upload
                if (model.NewGalleryImages != null && model.NewGalleryImages.Any())
                {
                    try
                    {
                        var galleryUpload = await _filesApiClient.UploadFilesAsync(model.NewGalleryImages, "tours", "Tours", id, "Gallery");
                        if (galleryUpload == null || galleryUpload.UploadedFiles.Count == 0)
                        {
                            uploadError = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogErrorAsync("Tours", "EditGalleryUpload", "Error uploading gallery images during tour edit", ex,null, id, "Tour");
                        uploadError = true;
                    }
                }

                if (uploadError)
                {
                    TempData["WarningMessage"] = "Tour salvato, ma si è verificato un errore durante il caricamento di alcune immagini.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Tour aggiornato con successo.";
                }
                return RedirectToAction(nameof(Index));
            }

            model.AvailableCategories = await GetCategoriesSelectList();
            model.AvailableVehicles = await GetVehiclesSelectList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _toursApiClient.DeleteTourAsync(id);
            TempData["SuccessMessage"] = "Tour eliminato con successo.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryImage(Guid imageId)
        {
            try
            {
                await _toursApiClient.DeleteGalleryImageAsync(imageId);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoriesSelectList()
        {
            var categories = await _toursApiClient.GetCategoriesAsync();
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).OrderBy(c => c.Text);
        }

        private async Task<IEnumerable<SelectListItem>> GetVehiclesSelectList()
        {
            var vehicles = await _vehiclesApiClient.GetAllVehiclesAsync();
            return vehicles.Where(v => v.IsActive).Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = $"{v.Name} ({v.CategoryName})"
            }).OrderBy(v => v.Text);
        }
    }
}
