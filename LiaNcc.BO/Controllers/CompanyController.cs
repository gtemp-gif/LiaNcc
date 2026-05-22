using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiaNcc.BO.Controllers.Base;
using LiaNcc.BO.Services.Interfaces;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ICompanyApiClient _companyApiClient;
        private readonly ILanguagesApiClient _languagesApiClient;
        private readonly ILocalizedContentsApiClient _localizedContentsApiClient;
        private readonly IFilesApiClient _filesApiClient;

        private readonly System.Collections.Generic.List<string> _translatableKeys = new System.Collections.Generic.List<string> { "Name", "Address", "AboutTitle", "AboutDescription" };

        public CompanyController(
            ICompanyApiClient companyApiClient,
            ILanguagesApiClient languagesApiClient,
            ILocalizedContentsApiClient localizedContentsApiClient,
            IFilesApiClient filesApiClient)
        {
            _companyApiClient = companyApiClient;
            _languagesApiClient = languagesApiClient;
            _localizedContentsApiClient = localizedContentsApiClient;
            _filesApiClient = filesApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var company = await _companyApiClient.GetFirstCompanyProfileAsync();
            if (company == null)
            {
                return RedirectToAction(nameof(Create));
            }
            // Fetch contacts separately if needed, but repository already includes them
            return View("Details", company);
        }

        public IActionResult Create()
        {
            return View(new CompanyProfile());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyProfile profile)
        {
            if (ModelState.IsValid)
            {
                await _companyApiClient.CreateAsync(profile);
                TempData["SuccessMessage"] = "Profilo aziendale salvato con successo.";
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var profile = await _companyApiClient.GetByIdAsync(id);
            if (profile == null) return NotFound();

            var model = new LiaNcc.BO.Models.Company.CompanyProfileViewModel
            {
                Id = profile.Id,
                Name = profile.Name,
                VatNumber = profile.VatNumber,
                Address = profile.Address,
                City = profile.City,
                ZipCode = profile.ZipCode,
                Country = profile.Country,
                Latitude = profile.Latitude,
                Longitude = profile.Longitude,
                GoogleMapsUrl = profile.GoogleMapsUrl,
                AboutTitle = profile.AboutTitle,
                AboutDescription = profile.AboutDescription,
                AboutImageUrl = profile.AboutImageUrl,
                Contacts = profile.CompanyContacts.OrderBy(c => c.SortOrder).ToList()
            };

            await PrepareLocalizationAsync(_languagesApiClient, _localizedContentsApiClient, model, "CompanyProfile", id, _translatableKeys);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LiaNcc.BO.Models.Company.CompanyProfileViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var profile = await _companyApiClient.GetByIdAsync(id);
                if (profile == null) return NotFound();

                if (model.AboutImageFile != null)
                {
                    var upload = await _filesApiClient.UploadFilesAsync(new System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile> { model.AboutImageFile }, "company", "CompanyProfile", id, "AboutImage");
                    if (upload?.UploadedFiles.Count > 0)
                    {
                        model.AboutImageUrl = upload.UploadedFiles[0].Url;
                    }
                }

                profile.Name = model.Name;
                profile.VatNumber = model.VatNumber;
                profile.Address = model.Address;
                profile.City = model.City;
                profile.ZipCode = model.ZipCode;
                profile.Country = model.Country;
                profile.Latitude = model.Latitude;
                profile.Longitude = model.Longitude;
                profile.GoogleMapsUrl = model.GoogleMapsUrl;
                profile.AboutTitle = model.AboutTitle;
                profile.AboutDescription = model.AboutDescription;
                profile.AboutImageUrl = model.AboutImageUrl;

                await _companyApiClient.UpdateAsync(id, profile);
                await SaveLocalizationAsync(_localizedContentsApiClient, model.Translations, "CompanyProfile", id);

                TempData["SuccessMessage"] = "Profilo aziendale aggiornato.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContact(Guid companyId, string type, string value, bool isPrimary, int sortOrder)
        {
            var contact = new CompanyContact
            {
                CompanyId = companyId,
                Type = type,
                Value = value,
                IsPrimary = isPrimary,
                SortOrder = sortOrder
            };

            await _companyApiClient.CreateCompanyContactAsync(contact);
            return RedirectToAction(nameof(Edit), new { id = companyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContact(Guid id, Guid companyId)
        {
            await _companyApiClient.DeleteCompanyContactAsync(id);
            return RedirectToAction(nameof(Edit), new { id = companyId });
        }
    }
}
