using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return RedirectToPage("/Account/AccessDenied", new { area = "Identity", returnUrl });
        }
    }
}
