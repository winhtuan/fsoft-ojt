using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.Models;

public static class UserCompiled
{
    public static readonly Func<AppDbContext, int, Task<UserAccount?>> ById = EF.CompileAsyncQuery(
        (AppDbContext db, int id) =>
            db.UserAccounts.Include(u => u.LoginData).FirstOrDefault(u => u.UserId == id)
    );

    public static readonly Func<AppDbContext, string, Task<UserLoginData?>> LoginByUsername =
        EF.CompileAsyncQuery(
            (AppDbContext db, string username) =>
                db.UserLoginDatas.Include(x => x.User).FirstOrDefault(x => x.Username == username)
        );

    public static readonly Func<AppDbContext, string, Task<UserLoginData?>> LoginByEmail =
        EF.CompileAsyncQuery(
            (AppDbContext db, string emailLower) =>
                db
                    .UserLoginDatas.Include(x => x.User)
                    .FirstOrDefault(x => x.Email.ToLower() == emailLower)
        );
}
