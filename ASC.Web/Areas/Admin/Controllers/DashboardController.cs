using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASC.Web.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IOptions<ApplicationSettings> _settings;

        public DashboardController(ILogger<DashboardController> logger, IOptions<ApplicationSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public IActionResult Index()
        {
            ViewBag.Title = $"{_settings.Value.ApplicationTitle} - Admin Dashboard";
            return View();
        }
    }
}
