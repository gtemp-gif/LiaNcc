using System.Collections.Generic;
using LiaNcc.BO.Models.Localization;

namespace LiaNcc.BO.Models.Common
{
    public interface ILocalizedViewModel
    {
        List<LocalizationTabViewModel> Translations { get; set; }
    }
}
