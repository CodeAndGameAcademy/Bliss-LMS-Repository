using LMS.AdminPanel.Helpers;
using LMS.AdminPanel.ViewModels;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace LMS.AdminPanel.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.MobileNumber = model.MobileNumber.Trim();

            var user = _context.Users
                .FirstOrDefault(x => x.MobileNumber == model.MobileNumber);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(model);
            }

            // CHECK LOCKOUT
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                ModelState.AddModelError("", $"Account locked. Try again at {user.LockoutEnd}");
                return View(model);
            }

            // PASSWORD CHECK
            if (!PasswordHelper.Verify(model.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                // Add delay here (anti brute-force)
                await Task.Delay(1000);

                // LOCK AFTER 5 ATTEMPTS
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0;
                }

                _context.SaveChanges();

                ModelState.AddModelError("", "Invalid credentials");
                return View(model);
            }

            // SUCCESS LOGIN → RESET COUNTER
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            _context.SaveChanges();

            // SESSION
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
