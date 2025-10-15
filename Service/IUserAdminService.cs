using Plantpedia.DTO;
using Plantpedia.Enum;

namespace Plantpedia.Service
{
    public interface IUserAdminService
    {
        Task<PagedResult<AdminUserListItemDto>> ListAsync(AdminUserQuery q);
        Task<int> CreateAsync(
            string username,
            string email,
            string name,
            char gender,
            DateTime dob,
            string? avatarUrl,
            string password
        );
        Task<bool> UpdateAsync(
            int userId,
            string name,
            char gender,
            DateTime dob,
            string? avatarUrl,
            UserStatus status
        );
        Task<bool> SoftDeleteAsync(int userId);
        Task<bool> RestoreAsync(int userId);
    }
}
