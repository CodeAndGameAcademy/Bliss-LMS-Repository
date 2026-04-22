using Microsoft.AspNetCore.Mvc;

namespace LMS.AdminPanel.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("Dashboard");
        }
    }
}
