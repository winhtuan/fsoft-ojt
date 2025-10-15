using Plantpedia.Models;
using Plantpedia.ViewModel;

namespace Plantpedia.Service
{
    public interface IUserService
    {
        Task<UserAccount?> LoginAsync(string username, string password);
        Task<UserAccount?> GetUserByIdAsync(int userId);
        Task<UserAccount?> GetUserByUsernameAsync(string username);
        Task<bool> UpdateProfileAsync(
            int userId,
            string lastName,
            char gender,
            DateTime dateOfBirth,
            string avatarUrl
        );
        Task<UserAccount> RegisterAsync(RegisterViewModel model);
        Task<int> GetUserCountAsync();
        Task<bool> EmailExistsAsync(string email);
    }
}
