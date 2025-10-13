using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.Helper;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly AppDbContext _db;

        public PasswordResetRepository(AppDbContext db) => _db = db;

        public async Task<PasswordReset> CreateAsync(PasswordReset pr)
        {
            LoggerHelper.Info($"Tạo yêu cầu đặt lại mật khẩu cho {pr.Email}");
            _db.PasswordResets.Add(pr);
            await _db.SaveChangesAsync();
            return pr;
        }

        public async Task<PasswordReset?> GetActiveByEmailAsync(string email)
        {
            var e = email.Trim().ToLowerInvariant();
            return await _db
                .PasswordResets.Where(x =>
                    x.Email.ToLower() == e && !x.Used && x.ExpiresAtUtc > DateTime.UtcNow
                )
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync();
        }

        public async Task MarkUsedAsync(long id)
        {
            var pr = await _db.PasswordResets.FindAsync(id);
            if (pr == null)
                return;
            pr.Used = true;
            await _db.SaveChangesAsync();
        }

        public async Task IncrementAttemptsAsync(long id)
        {
            var pr = await _db.PasswordResets.FindAsync(id);
            if (pr == null)
                return;
            pr.Attempts++;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCodeAsync(long id, string newHash, DateTime newExpiryUtc)
        {
            var pr = await _db.PasswordResets.FindAsync(id);
            if (pr == null)
                return;
            pr.CodeHash = newHash;
            pr.ExpiresAtUtc = newExpiryUtc;
            pr.CreatedAtUtc = DateTime.UtcNow;
            pr.Attempts = 0;
            await _db.SaveChangesAsync();
        }
    }
}
