using Plantpedia.DTO;
using Plantpedia.Enum;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Repository;

namespace Plantpedia.Service
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserRepository _repo;

        public UserAdminService(IUserRepository repo)
        {
            _repo = repo;
        }

        public Task<PagedResult<AdminUserListItemDto>> ListAsync(AdminUserQuery q) =>
            _repo.GetUsersPagedAsync(q);

        public async Task<int> CreateAsync(
            string username,
            string email,
            string name,
            char gender,
            DateTime dob,
            string? avatarUrl,
            string password
        )
        {
            // Hash & salt
            var (salt, hash) = PasswordHelper.HashPassword(password);

            var account = new UserAccount
            {
                LastName = name,
                Gender = gender,
                DateOfBirth = dob,
                AvatarUrl = avatarUrl!,
                Status = UserStatus.Active,
            };
            var login = new UserLoginData
            {
                Username = username,
                Email = email,
                Role = Role.user,
                PasswordSalt = salt,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow,
            };
            return await _repo.CreateUserAsync(account, login);
        }

        public async Task<bool> UpdateAsync(
            int userId,
            string name,
            char gender,
            DateTime dob,
            string? avatarUrl,
            UserStatus status
        )
        {
            var user = await _repo.GetUserAccountByIdAsync(userId);
            if (user == null)
                return false;
            user.LastName = name;
            user.Gender = gender;
            user.DateOfBirth = dob;
            user.AvatarUrl = avatarUrl!;
            user.Status = status;
            return await _repo.UpdateUserAsync(user);
        }

        public Task<bool> SoftDeleteAsync(int userId) => _repo.SoftDeleteAsync(userId);

        public Task<bool> RestoreAsync(int userId) => _repo.RestoreAsync(userId);

        public Task<IReadOnlyList<UserActivityItemDto>> GetHistoryAsync(
            int userId,
            int take = 50
        ) => _repo.GetRecentAsync(userId, take);
    }
}
