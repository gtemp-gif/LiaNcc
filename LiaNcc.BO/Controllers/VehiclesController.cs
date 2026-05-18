using System;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly IVehiclesApiClient _vehiclesApiClient;

        public VehiclesController(IVehiclesApiClient vehiclesApiClient)
        {
            _vehiclesApiClient = vehiclesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehiclesApiClient.GetAllAsync();
            return View(vehicles);
        }

        public IActionResult Create()
        {
            return View(new Vehicle());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                await _vehiclesApiClient.CreateAsync(vehicle);
                TempData["SuccessMessage"] = "Veicolo creato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var vehicle = await _vehiclesApiClient.GetByIdAsync(id);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Vehicle vehicle)
        {
            if (id != vehicle.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _vehiclesApiClient.UpdateAsync(id, vehicle);
                TempData["SuccessMessage"] = "Veicolo aggiornato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _vehiclesApiClient.DeleteAsync(id);
            TempData["SuccessMessage"] = "Veicolo eliminato con successo.";
            return RedirectToAction(nameof(Index));
        }
    }
}
