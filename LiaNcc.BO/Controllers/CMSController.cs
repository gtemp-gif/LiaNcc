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
        private readonly ILanguagesApiClient _languagesApiClient;
        private readonly ILocalizedContentsApiClient _localizedContentsApiClient;

        private readonly List<string> _translatableKeys = new List<string> { "Name", "MetaTitle", "MetaDescription" };

        private readonly IApplicationLoggerService _logger;

        public CMSController(
            ISitePagesApiClient sitePagesApiClient,
            IApplicationLoggerService applicationLogger,
            ILanguagesApiClient languagesApiClient,
            ILocalizedContentsApiClient localizedContentsApiClient)
        {
            _sitePagesApiClient = sitePagesApiClient;
            _logger = applicationLogger;
            _languagesApiClient = languagesApiClient;
            _localizedContentsApiClient = localizedContentsApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var pages = await _sitePagesApiClient.GetAllAsync();
            return View(pages);
        }

        public async Task<IActionResult> Create()
        {
            var model = new LiaNcc.BO.Models.CMS.SitePageViewModel();
            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "SitePage", null, _translatableKeys);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LiaNcc.BO.Models.CMS.SitePageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sitePage = new SitePage
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    MetaTitle = model.MetaTitle,
                    MetaDescription = model.MetaDescription,
                    IsActive = model.IsActive
                };
                var createdPage = await _sitePagesApiClient.CreateAsync(sitePage);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "SitePage", createdPage.Id);
                await _logger.LogInfoAsync("CMS", "CreatePage", $"Page {createdPage.Name} created via BO", createdPage.Id, "SitePage");
                TempData["SuccessMessage"] = "Pagina creata con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var page = await _sitePagesApiClient.GetByIdAsync(id);
            if (page == null) return NotFound();

            var model = new LiaNcc.BO.Models.CMS.SitePageViewModel
            {
                Id = page.Id,
                Name = page.Name,
                Slug = page.Slug,
                MetaTitle = page.MetaTitle,
                MetaDescription = page.MetaDescription,
                IsActive = page.IsActive
            };

            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "SitePage", id, _translatableKeys);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LiaNcc.BO.Models.CMS.SitePageViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var page = await _sitePagesApiClient.GetByIdAsync(id);
                if (page == null) return NotFound();

                page.Name = model.Name;
                page.Slug = model.Slug;
                page.MetaTitle = model.MetaTitle;
                page.MetaDescription = model.MetaDescription;
                page.IsActive = model.IsActive;

                await _sitePagesApiClient.UpdateAsync(id, page);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "SitePage", id);
                await _logger.LogInfoAsync("CMS", "UpdatePage", $"Page {page.Name} updated via BO", id, "SitePage");
                TempData["SuccessMessage"] = "Pagina aggiornata con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
