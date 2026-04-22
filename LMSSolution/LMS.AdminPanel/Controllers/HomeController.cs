using Microsoft.AspNetCore.Mvc;

namespace LMS.AdminPanel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
