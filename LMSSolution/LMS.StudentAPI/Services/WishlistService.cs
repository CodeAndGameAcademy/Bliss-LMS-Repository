using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using LMS.StudentAPI.DTOs.WishList;
using LMS.StudentAPI.Exceptions;
using LMS.StudentAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.StudentAPI.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;

        public WishlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Guid userId, CreateWishlistDto dto)
        {
            // Check already exists
            var exists = await _context.Wishlists
                .AnyAsync(x => x.UserId == userId && x.CourseId == dto.CourseId);

            if (exists)
                throw new BadRequestException("Wishlist already in wishlist");

            var wishlist = new Wishlist
            {
                UserId = userId,
                CourseId = dto.CourseId
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MyWishlistDto>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .Where(x => x.UserId == userId)
                .Include(x => x.Course)
                .Select(x => new MyWishlistDto
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    Title = x.Course.Title,
                    Thumbnail = x.Course.BaseUrl + "/" + x.Course.Thumbnail,
                    InstructorName = x.Course.Instructor.Name,
                    Price = x.Course.Price,
                    FinalPrice = x.Course.FinalPrice,
                    DiscountPercentage = x.Course.DiscountPercentage
                })
                .ToListAsync();
        }

        public async Task RemoveAsync(Guid userId, Guid courseId)
        {
            var wishlist = await _context.Wishlists
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);

            if (wishlist == null)
                throw new BadRequestException("Wishlist item not found");

            // Hard Delete
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveIfExistsAsync(Guid userId, Guid courseId)
        {
            var wishlist = await _context.Wishlists
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);

            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();
            }
        }
    }
}
