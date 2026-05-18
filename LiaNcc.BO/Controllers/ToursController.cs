using System;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class ToursController : BaseController
    {
        private readonly IToursApiClient _toursApiClient;

        public ToursController(IToursApiClient toursApiClient)
        {
            _toursApiClient = toursApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var tours = await _toursApiClient.GetAllAsync();
            return View(tours);
        }

        public IActionResult Create()
        {
            return View(new Tour());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tour tour)
        {
            if (ModelState.IsValid)
            {
                await _toursApiClient.CreateAsync(tour);
                TempData["SuccessMessage"] = "Tour creato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var tour = await _toursApiClient.GetByIdAsync(id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Tour tour)
        {
            if (id != tour.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _toursApiClient.UpdateAsync(id, tour);
                TempData["SuccessMessage"] = "Tour aggiornato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _toursApiClient.DeleteAsync(id);
            TempData["SuccessMessage"] = "Tour eliminato con successo.";
            return RedirectToAction(nameof(Index));
        }
    }
}
