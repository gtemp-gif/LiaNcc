using System;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Controllers
{
    public class CMSController : BaseController
    {
        private readonly ISitePagesApiClient _sitePagesApiClient;

        public CMSController(ISitePagesApiClient sitePagesApiClient)
        {
            _sitePagesApiClient = sitePagesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var pages = await _sitePagesApiClient.GetAllAsync();
            return View(pages);
        }

        public IActionResult Create()
        {
            return View(new SitePage());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SitePage sitePage)
        {
            if (ModelState.IsValid)
            {
                await _sitePagesApiClient.CreateAsync(sitePage);
                TempData["SuccessMessage"] = "Pagina creata con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(sitePage);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var page = await _sitePagesApiClient.GetByIdAsync(id);
            if (page == null) return NotFound();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SitePage sitePage)
        {
            if (id != sitePage.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _sitePagesApiClient.UpdateAsync(id, sitePage);
                TempData["SuccessMessage"] = "Pagina aggiornata con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(sitePage);
        }
    }
}
