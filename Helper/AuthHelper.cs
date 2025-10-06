using Plantpedia.Enum;
using Plantpedia.Models;

namespace Plantpedia.Helper;

public class AuthHelper
{
    public static UserAccount CreateUserAccount(string name)
    {
        return new UserAccount
        {
            LastName = name,
            Gender = 'U',
            DateOfBirth = DateTime.UtcNow,
            AvatarUrl = "https://placehold.co/400",
        };
    }

    public static UserLoginData CreateUserLoginData(int userId, string email, string password)
    {
        var (hash, salt) = PasswordHelper.HashPassword(password);

        return new UserLoginData
        {
            UserId = userId,
            Username = email.Split('@')[0],
            Email = email,
            Role = Role.user,
            PasswordSalt = salt,
            PasswordHash = hash,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
