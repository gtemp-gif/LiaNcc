using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiaNcc.BO.Controllers.Base
{
    [Authorize]
    public abstract class BaseController : Controller
    {
    }
}
