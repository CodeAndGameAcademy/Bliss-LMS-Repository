using LMS.AdminPanel.Exceptions;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.AdminPanel.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Users
                .IgnoreQueryFilters()
                .Where(x => x.Role != Role.ADMIN).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var entity = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Student not found");

                entity.DeletedAt = DateTime.UtcNow;
                entity.IsActive = false;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var entity = await _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null);

                if (entity == null)
                    throw new NotFoundException("Student not found or not deleted");

                entity.DeletedAt = null;
                entity.IsActive = true;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
