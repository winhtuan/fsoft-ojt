using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Plantpedia.Enum;
using Plantpedia.Models;

namespace Plantpedia.Helper;

public static class AuthHelper
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

    public static async Task SignInUserAsync(
        this HttpContext httpContext,
        UserAccount user,
        string displayNameOrEmail
    )
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.LoginData.Username ?? displayNameOrEmail),
            new Claim(ClaimTypes.Role, user.LoginData.Role.ToString()),
            new Claim(ClaimTypes.Email, displayNameOrEmail),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );
    }

    public static IEnumerable<string> ToErrorMessages(this ModelStateDictionary modelState)
    {
        return modelState
            .Values.SelectMany(v => v.Errors)
            .Select(e =>
                string.IsNullOrEmpty(e.ErrorMessage) ? "Giá trị không hợp lệ." : e.ErrorMessage
            );
    }
}
