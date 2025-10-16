// Repository/UserRepository.cs
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.DTO;
using Plantpedia.Enum;
using Plantpedia.Helper;
using Plantpedia.Models;

namespace Plantpedia.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // ======= READS =======
    public Task<UserLoginData?> GetUserLoginDataByUsernameAsync(string username) =>
        UserCompiled.LoginByUsername(_db, username);

    public async Task<bool> IsGmailExistsAsync(string email) =>
        await _db.UserLoginDatas.AnyAsync(x => x.Email == email);

    public Task<UserAccount?> GetUserByIdAsync(int userId) => UserCompiled.ById(_db, userId);

    public async Task<UserAccount?> GetUserByUsernameAsync(string username) =>
        (await UserCompiled.LoginByUsername(_db, username))?.User;

    public Task<UserAccount?> GetUserAccountByIdAsync(int userId) =>
        _db.UserAccounts.FirstOrDefaultAsync(u => u.UserId == userId);

    public Task<UserLoginData?> GetUserLoginDataByEmailAsync(string email) =>
        UserCompiled.LoginByEmail(_db, email.Trim().ToLowerInvariant());

    public Task<int> GetUserCountAsync() => _db.UserAccounts.CountAsync();

    public async Task<PagedResult<AdminUserListItemDto>> GetUsersPagedAsync(AdminUserQuery q)
    {
        var baseQ = _db.UserAccounts.AsNoTracking().Include(u => u.LoginData).ApplyAdminFilter(q);

        var total = await baseQ.CountAsync();

        var items = await baseQ
            .OrderByDescending(u => u.CreatedAt)
            .ProjectTo<AdminUserListItemDto>(_mapper.ConfigurationProvider)
            .PaginateAsync(q.Page, q.PageSize);

        return new PagedResult<AdminUserListItemDto> { Items = items, Total = total };
    }

    public async Task<UserAccount?> GetUserWithLoginAsync(int userId) =>
        await _db
            .UserAccounts.Include(u => u.LoginData)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId);

    // ======= WRITES =======
    public async Task AddUserAccountAsync(UserAccount user)
    {
        _db.UserAccounts.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task AddUserLoginDataAsync(UserLoginData loginData)
    {
        _db.UserLoginDatas.Add(loginData);
        await _db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();

    public async Task<UserAccount> RegisterNewUserAsync(string name, string email, string password)
    {
        var ua = AuthHelper.CreateUserAccount(name);
        var (salt, hash) = PasswordHelper.HashPassword(password);
        var login = new UserLoginData
        {
            UserId = ua.UserId,
            Username = email,
            Email = email,
            Role = Role.user,
            PasswordSalt = salt,
            PasswordHash = hash,
            CreatedAt = DateTime.UtcNow,
        };

        using var tx = await _db.Database.BeginTransactionAsync();
        _db.UserAccounts.Add(ua);
        await _db.SaveChangesAsync(); // có UserId
        login.UserId = ua.UserId;
        _db.UserLoginDatas.Add(login);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return ua;
    }

    public async Task<int> CreateUserAsync(UserAccount account, UserLoginData login)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        _db.UserAccounts.Add(account);
        await _db.SaveChangesAsync(); // lấy Id
        login.UserId = account.UserId;
        _db.UserLoginDatas.Add(login);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return account.UserId;
    }

    public async Task<bool> UpdateUserAsync(UserAccount account)
    {
        account.UpdatedAt = DateTime.UtcNow;
        _db.UserAccounts.Update(account);
        return (await _db.SaveChangesAsync()) > 0;
    }

    public async Task<bool> SoftDeleteAsync(int userId)
    {
        var user = await _db.UserAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
        if (user is null)
            return false;
        user.Status = UserStatus.Deleted;
        user.DeletedAt = DateTime.UtcNow;
        return (await _db.SaveChangesAsync()) > 0;
    }

    public async Task<bool> RestoreAsync(int userId)
    {
        var user = await _db.UserAccounts.FirstOrDefaultAsync(x => x.UserId == userId);
        if (user is null)
            return false;
        user.Status = UserStatus.Active;
        user.DeletedAt = null;
        return (await _db.SaveChangesAsync()) > 0;
    }

    // ======= Activity (tuỳ chọn: nên tách sang IUserActivityRepository) =======
    public async Task LogActivityAsync(
        int userId,
        ActivityType type,
        string? refId,
        object? metadata
    )
    {
        var json = metadata is null ? null : System.Text.Json.JsonSerializer.Serialize(metadata);
        _db.UserActivities.Add(
            new UserActivity
            {
                UserId = userId,
                Type = type,
                RefId = refId,
                Metadata = json,
            }
        );
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<UserActivityItemDto>> GetRecentAsync(int userId, int take = 50)
    {
        take = Math.Clamp(take, 1, 200);

        var rows = await _db
            .UserActivities.AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(take)
            .Select(a => new
            {
                a.ActivityId,
                a.Type,
                a.RefId,
                a.Metadata,
                a.CreatedAt,
            })
            .ToListAsync();

        var list = new List<UserActivityItemDto>(rows.Count);

        foreach (var r in rows)
        {
            string desc = "";
            string? ip = null;

            if (!string.IsNullOrWhiteSpace(r.Metadata))
            {
                try
                {
                    using var doc = JsonDocument.Parse(r.Metadata);
                    var root = doc.RootElement;

                    if (
                        root.TryGetProperty("ip", out var ipProp)
                        && ipProp.ValueKind == JsonValueKind.String
                    )
                        ip = ipProp.GetString();

                    switch (r.Type)
                    {
                        case ActivityType.Search:
                            if (
                                root.TryGetProperty("keyword", out var kw)
                                && kw.ValueKind == JsonValueKind.String
                            )
                                desc = $"Tìm kiếm: '{kw.GetString()}'";
                            break;

                        case ActivityType.Comment:
                            if (
                                root.TryGetProperty("content", out var cmt)
                                && cmt.ValueKind == JsonValueKind.String
                            )
                                desc = $"Bình luận {r.RefId}: {cmt.GetString()}";
                            else
                                desc = $"Bình luận {r.RefId}";
                            break;

                        case ActivityType.Reaction:
                            if (
                                root.TryGetProperty("emoji", out var emo)
                                && emo.ValueKind == JsonValueKind.String
                            )
                                desc = $"Phản ứng {r.RefId}: {emo.GetString()}";
                            else
                                desc = $"Phản ứng {r.RefId}";
                            break;
                    }
                }
                catch
                {
                    // ignore parse errors -> fallbacks below
                }
            }

            if (string.IsNullOrEmpty(desc))
            {
                desc = r.Type switch
                {
                    ActivityType.Search => "Tìm kiếm",
                    ActivityType.Comment => $"Bình luận {r.RefId}",
                    ActivityType.Reaction => $"Phản ứng {r.RefId}",
                    _ => "Hoạt động",
                };
            }

            list.Add(
                new UserActivityItemDto
                {
                    ActivityId = r.ActivityId,
                    Type = r.Type.ToString(),
                    Description = desc,
                    CreatedAt = r.CreatedAt,
                }
            );
        }

        return list;
    }
}
