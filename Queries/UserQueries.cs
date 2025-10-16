using System.Linq;
using Microsoft.EntityFrameworkCore;
using Plantpedia.DTO;
using Plantpedia.Models;

public static class UserQueries
{
    public static IQueryable<UserAccount> ApplyAdminFilter(
        this IQueryable<UserAccount> q,
        AdminUserQuery filter
    )
    {
        if (filter.Status != null)
            q = q.Where(u => u.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.Q))
        {
            var k = filter.Q.Trim().ToLower();
            q = q.Where(u =>
                EF.Functions.ILike(u.LastName, $"%{k}%")
                || EF.Functions.ILike(u.LoginData.Username, $"%{k}%")
                || EF.Functions.ILike(u.LoginData.Email, $"%{k}%")
            );
        }

        return q;
    }

    public static async Task<List<T>> PaginateAsync<T>(
        this IQueryable<T> q,
        int page,
        int pageSize,
        CancellationToken ct = default
    ) => await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
}
