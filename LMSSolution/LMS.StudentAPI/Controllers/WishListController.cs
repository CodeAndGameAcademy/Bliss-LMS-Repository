using LMS.StudentAPI.DTOs.WishList;
using LMS.StudentAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.StudentAPI.Controllers
{
    [Route("api/v1/wishlist")]
    [Authorize(Roles = "STUDENT")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishListController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateWishlistDto dto)
        {
            await _wishlistService.AddAsync(GetUserId(), dto);
            return Ok(new { message = "Course added to wishlist" });
        }

        [HttpDelete("{courseId:guid}")]
        public async Task<IActionResult> Remove(Guid courseId)
        {
            await _wishlistService.RemoveAsync(GetUserId(), courseId);
            return Ok(new { message = "Course removed from wishlist" });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var data = await _wishlistService.GetByUserIdAsync(GetUserId());
            return Ok(data);
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirst("userId")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            return Guid.Parse(userId);
        }
    }
}
