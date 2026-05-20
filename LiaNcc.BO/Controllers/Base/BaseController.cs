using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers.Base
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected async Task PrepareLocalizationAsync(LiaNcc.BO.Services.Interfaces.ILanguagesApiClient languagesApiClient, LiaNcc.BO.Services.Interfaces.ILocalizedContentsApiClient localizedContentsApiClient, LiaNcc.BO.Models.Common.ILocalizedViewModel model, string entityName, Guid? entityId, List<string> keys)
        {
            var allLanguages = await languagesApiClient.GetAllAsync();
            var activeLanguages = allLanguages.Where(l => l.IsActive && !l.IsDefault).ToList();

            IEnumerable<LiaNcc.Models.Entities.LocalizedContent> existingTranslations = new List<LiaNcc.Models.Entities.LocalizedContent>();
            if (entityId.HasValue)
            {
                existingTranslations = await localizedContentsApiClient.GetByEntityAsync(entityName, entityId.Value);
            }

            foreach (var lang in activeLanguages)
            {
                var tab = new LiaNcc.BO.Models.Localization.LocalizationTabViewModel
                {
                    LanguageCode = lang.Code,
                    LanguageName = lang.Name
                };

                foreach (var key in keys)
                {
                    var val = existingTranslations.FirstOrDefault(t => t.LanguageCode == lang.Code && t.ContentKey == key)?.ContentValue;
                    tab.Values.Add(key, val);
                }

                model.Translations.Add(tab);
            }
        }

        protected async Task SaveLocalizationAsync(LiaNcc.BO.Services.Interfaces.ILocalizedContentsApiClient localizedContentsApiClient, List<LiaNcc.BO.Models.Localization.LocalizationTabViewModel> translations, string entityName, Guid entityId)
        {
            if (translations == null) return;

            var batch = new List<LiaNcc.Models.Entities.LocalizedContent>();
            foreach (var tab in translations)
            {
                foreach (var kvp in tab.Values)
                {
                    batch.Add(new LiaNcc.Models.Entities.LocalizedContent
                    {
                        EntityName = entityName,
                        EntityId = entityId,
                        LanguageCode = tab.LanguageCode,
                        ContentKey = kvp.Key,
                        ContentValue = kvp.Value
                    });
                }
            }

            if (batch.Any())
            {
                await localizedContentsApiClient.UpsertBatchAsync(batch);
            }
        }
    }
}
