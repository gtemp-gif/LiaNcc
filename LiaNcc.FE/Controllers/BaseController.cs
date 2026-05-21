using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LiaNcc.FE.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string CurrentCulture => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    }
}
