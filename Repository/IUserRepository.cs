using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public interface IUserRepository
    {
        Task<UserLoginData?> GetUserLoginDataByUsernameAsync(string username);
        Task<bool> IsUsernameExistsAsync(string username);
        Task AddUserAccountAsync(UserAccount user);
        Task AddUserLoginDataAsync(UserLoginData loginData);
        Task<UserAccount?> GetUserByIdAsync(int userId);
        Task<UserAccount?> GetUserByUsernameAsync(string username);
        Task<UserAccount?> GetUserAccountByIdAsync(int userId);
        Task SaveChangesAsync();
    }
}
