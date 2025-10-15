using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.DTO;
using Plantpedia.Enum;
using Plantpedia.Helper;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserLoginData?> GetUserLoginDataByUsernameAsync(string username)
        {
            LoggerHelper.Info($"Attempting to get user login data for username: {username}.");
            try
            {
                var userLoginData = await _context
                    .UserLoginDatas.Include(uld => uld.User)
                    .FirstOrDefaultAsync(uld => uld.Username == username);

                if (userLoginData == null)
                {
                    LoggerHelper.Warn($"No user login data found for username: {username}.");
                }
                else
                {
                    LoggerHelper.Info(
                        $"Successfully found user login data for username: {username}."
                    );
                }

                return userLoginData;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"An error occurred while getting user login data for username: {username}."
                );
                throw;
            }
        }

        public async Task<bool> IsGmailExistsAsync(string email)
        {
            LoggerHelper.Info($"Checking if email '{email}' exists.");
            try
            {
                var exists = await _context.UserLoginDatas.AnyAsync(uld => uld.Email == email);
                LoggerHelper.Info($"Email '{email}' exists: {exists}.");
                return exists;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"An error occurred while checking for email: {email}.");
                throw;
            }
        }

        public async Task AddUserAccountAsync(UserAccount user)
        {
            if (user == null)
            {
                LoggerHelper.Error("Attempted to add a null user account.");
                throw new ArgumentNullException(nameof(user));
            }

            LoggerHelper.Info("Attempting to add a new user account.");
            try
            {
                _context.UserAccounts.Add(user);
                await _context.SaveChangesAsync();
                LoggerHelper.Info($"Successfully added new user account with ID: {user.UserId}.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while adding a new user account.");
                throw;
            }
        }

        public async Task AddUserLoginDataAsync(UserLoginData loginData)
        {
            if (loginData == null)
            {
                LoggerHelper.Error("Attempted to add null user login data.");
                throw new ArgumentNullException(nameof(loginData));
            }

            LoggerHelper.Info($"Attempting to add login data for username: {loginData.Username}.");
            try
            {
                _context.UserLoginDatas.Add(loginData);
                await _context.SaveChangesAsync();
                LoggerHelper.Info(
                    $"Successfully added login data for username: {loginData.Username}."
                );
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"An error occurred while adding login data for username: {loginData.Username}."
                );
                throw;
            }
        }

        public async Task<UserAccount?> GetUserByIdAsync(int userId)
        {
            LoggerHelper.Info($"Attempting to get user by ID: {userId}.");
            try
            {
                var user = await _context
                    .UserAccounts.Include(ua => ua.LoginData)
                    .FirstOrDefaultAsync(ua => ua.UserId == userId);

                if (user == null)
                {
                    LoggerHelper.Warn($"No user found with ID: {userId}.");
                }
                else
                {
                    LoggerHelper.Info($"Successfully found user with ID: {userId}.");
                }

                return user;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"An error occurred while getting user by ID: {userId}.");
                throw;
            }
        }

        public async Task<UserAccount?> GetUserByUsernameAsync(string username)
        {
            LoggerHelper.Info($"Attempting to get user by username: {username}.");
            try
            {
                var userLoginData = await _context
                    .UserLoginDatas.Include(uld => uld.User)
                    .FirstOrDefaultAsync(uld => uld.Username == username);

                if (userLoginData?.User == null)
                {
                    LoggerHelper.Warn($"No user found for username: {username}.");
                }
                else
                {
                    LoggerHelper.Info($"Successfully found user for username: {username}.");
                }

                return userLoginData?.User;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"An error occurred while getting user by username: {username}."
                );
                throw;
            }
        }

        public async Task<UserAccount?> GetUserAccountByIdAsync(int userId)
        {
            LoggerHelper.Info($"Attempting to get user account by ID: {userId}.");
            try
            {
                var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(ua =>
                    ua.UserId == userId
                );
                if (userAccount == null)
                {
                    LoggerHelper.Warn($"No user account found with ID: {userId}.");
                }
                else
                {
                    LoggerHelper.Info($"Successfully found user account with ID: {userId}.");
                }
                return userAccount;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"An error occurred while getting user account by ID: {userId}."
                );
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            LoggerHelper.Info("Attempting to save changes to the database.");
            try
            {
                await _context.SaveChangesAsync();
                LoggerHelper.Info("Successfully saved changes to the database.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while saving changes to the database.");
                throw;
            }
        }

        public async Task<UserAccount> RegisterNewUserAsync(
            string name,
            string email,
            string password
        )
        {
            LoggerHelper.Info($"Bắt đầu đăng ký người dùng mới: {email}");

            try
            {
                var userAccount = AuthHelper.CreateUserAccount(name);
                await AddUserAccountAsync(userAccount);
                LoggerHelper.Info($"Đã tạo UserAccount cho: {email} với ID: {userAccount.UserId}");
                await SaveChangesAsync();

                var loginData = AuthHelper.CreateUserLoginData(userAccount.UserId, email, password);
                await AddUserLoginDataAsync(loginData);
                LoggerHelper.Info($"Đã tạo UserLoginData cho: {email}");
                await SaveChangesAsync();

                LoggerHelper.Info($"Đăng ký thành công cho người dùng: {email}");
                return userAccount;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Lỗi khi đăng ký người dùng: {email}");
                throw;
            }
        }

        public async Task<UserLoginData?> GetUserLoginDataByEmailAsync(string email)
        {
            var e = email.Trim().ToLowerInvariant();
            return await _context
                .UserLoginDatas.Include(uld => uld.User)
                .FirstOrDefaultAsync(uld => uld.Email.ToLower() == e);
        }

        public async Task<int> GetUserCountAsync()
        {
            LoggerHelper.Info("Getting total user count.");
            try
            {
                var count = await _context.UserAccounts.CountAsync();
                LoggerHelper.Info($"Total user count: {count}");
                return count;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "An error occurred while getting user count.");
                throw;
            }
        }

        public async Task<PagedResult<AdminUserListItemDto>> GetUsersPagedAsync(AdminUserQuery q)
        {
            var users = _context
                .UserAccounts.AsNoTracking()
                .Include(u => u.LoginData)
                .Where(u => q.Status == null || u.Status == q.Status);

            if (!string.IsNullOrWhiteSpace(q.Q))
            {
                var k = q.Q.Trim().ToLower();
                users = users.Where(u =>
                    u.LastName.ToLower().Contains(k)
                    || u.LoginData.Username.ToLower().Contains(k)
                    || u.LoginData.Email.ToLower().Contains(k)
                );
            }

            var total = await users.CountAsync();

            var items = await users
                .OrderByDescending(u => u.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(u => new AdminUserListItemDto
                {
                    UserId = u.UserId,
                    Username = u.LoginData.Username,
                    Email = u.LoginData.Email,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    DateOfBirth = u.DateOfBirth,
                    AvatarUrl = u.AvatarUrl,
                    Status = u.Status,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LoginData.LastLoginAt,
                    CommentCount = u.PlantComments.Count(),
                    ReactionCount = u.PlantCommentReactions.Count(),
                    SearchCount = u.Activities.Count(a => a.Type == ActivityType.Search),
                })
                .ToListAsync();

            return new PagedResult<AdminUserListItemDto> { Items = items, Total = total };
        }

        public async Task<int> CreateUserAsync(UserAccount account, UserLoginData login)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            _context.UserAccounts.Add(account);
            await _context.SaveChangesAsync();
            login.UserId = account.UserId;
            _context.UserLoginDatas.Add(login);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return account.UserId;
        }

        public async Task<bool> UpdateUserAsync(UserAccount account)
        {
            account.UpdatedAt = DateTime.UtcNow;
            _context.UserAccounts.Update(account);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> SoftDeleteAsync(int userId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
                return false;
            user.Status = UserStatus.Deleted;
            user.DeletedAt = DateTime.UtcNow;
            _context.UserAccounts.Update(user);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> RestoreAsync(int userId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
                return false;
            user.Status = UserStatus.Active;
            user.DeletedAt = null;
            _context.UserAccounts.Update(user);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task LogActivityAsync(
            int userId,
            ActivityType type,
            string? refId,
            object? metadata
        )
        {
            var json =
                metadata == null ? null : System.Text.Json.JsonSerializer.Serialize(metadata);
            _context.UserActivities.Add(
                new UserActivity
                {
                    UserId = userId,
                    Type = type,
                    RefId = refId,
                    Metadata = json,
                }
            );
            await _context.SaveChangesAsync();
        }

        public async Task<UserAccount?> GetUserWithLoginAsync(int userId)
        {
            var q = _context
                .UserAccounts.Include(u => u.LoginData)
                // (tuỳ chọn) include thêm nếu cần hiển thị trong admin:
                //.Include(u => u.PlantComments)
                //.Include(u => u.PlantCommentReactions)
                //.Include(u => u.Activities)
                .AsNoTracking()
                .Where(u => u.UserId == userId);

            return await q.FirstOrDefaultAsync();
        }
    }
}
