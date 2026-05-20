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

        public ToursController(IToursApiClient toursApiClient, IVehiclesApiClient vehiclesApiClient, IFilesApiClient filesApiClient)
        {
            _toursApiClient = toursApiClient;
            _vehiclesApiClient = vehiclesApiClient;
            _filesApiClient = filesApiClient;
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
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder
                };

                await _toursApiClient.CreateTourAsync(tour);
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
                IsActive = tour.IsActive,
                SortOrder = tour.SortOrder,
                AvailableCategories = await GetCategoriesSelectList(),
                AvailableVehicles = await GetVehiclesSelectList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TourViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
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
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder
                };

                await _toursApiClient.UpdateTourAsync(id, tour);
                TempData["SuccessMessage"] = "Tour aggiornato con successo.";
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
