using System;
using System.Threading.Tasks;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers
{
    public class MediaController : BaseController
    {
        private readonly IMediaAssetsApiClient _mediaAssetsApiClient;

        private readonly IApplicationLoggerService _logger;

        public MediaController(IMediaAssetsApiClient mediaAssetsApiClient, IApplicationLoggerService applicationLogger)
        {
            _mediaAssetsApiClient = mediaAssetsApiClient;
            _logger = applicationLogger;
        }

        public async Task<IActionResult> Index()
        {
            var media = await _mediaAssetsApiClient.GetAllAsync();
            return View(media);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Seleziona un file valido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _mediaAssetsApiClient.UploadMediaAsync(file);
                TempData["SuccessMessage"] = "Media caricato con successo.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Errore durante il caricamento.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediaAssetsApiClient.DeleteAsync(id);
            TempData["SuccessMessage"] = "Media eliminato.";
            return RedirectToAction(nameof(Index));
        }
    }
}
