using Plantpedia.DTO;
using Plantpedia.Enum;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public interface IUserRepository
    {
        Task<UserLoginData?> GetUserLoginDataByUsernameAsync(string username);
        Task<bool> IsGmailExistsAsync(string username);
        Task AddUserAccountAsync(UserAccount user);
        Task AddUserLoginDataAsync(UserLoginData loginData);
        Task<UserAccount?> GetUserByIdAsync(int userId);
        Task<UserAccount?> GetUserByUsernameAsync(string username);
        Task<UserAccount?> GetUserAccountByIdAsync(int userId);
        Task<UserAccount> RegisterNewUserAsync(string name, string email, string password);
        Task<UserLoginData?> GetUserLoginDataByEmailAsync(string email);
        Task<int> GetUserCountAsync();
        Task SaveChangesAsync();
        Task<PagedResult<AdminUserListItemDto>> GetUsersPagedAsync(AdminUserQuery q);
        Task<UserAccount?> GetUserWithLoginAsync(int userId);
        Task<int> CreateUserAsync(UserAccount account, UserLoginData login); // trả id
        Task<bool> UpdateUserAsync(UserAccount account);
        Task<bool> SoftDeleteAsync(int userId);
        Task<bool> RestoreAsync(int userId);

        // Log hành vi (dùng chung)
        Task LogActivityAsync(int userId, ActivityType type, string? refId, object? metadata);
    }
}
