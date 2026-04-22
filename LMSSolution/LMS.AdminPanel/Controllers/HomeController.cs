using Microsoft.AspNetCore.Mvc;

namespace LMS.AdminPanel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            return RedirectToAction("Index", "Dashboard");
        }
    }
}
