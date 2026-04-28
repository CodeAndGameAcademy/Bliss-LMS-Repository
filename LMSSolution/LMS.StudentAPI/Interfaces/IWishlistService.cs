using LMS.StudentAPI.DTOs.WishList;

namespace LMS.StudentAPI.Interfaces
{
    public interface IWishlistService
    {
        Task AddAsync(Guid userId, CreateWishlistDto dto);
        Task RemoveAsync(Guid userId, Guid courseId);

        Task<List<MyWishlistDto>> GetByUserIdAsync(Guid userId);

        // Called internally after enrollment
        Task RemoveIfExistsAsync(Guid userId, Guid courseId);
    }
}
