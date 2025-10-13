using System.Threading.Tasks;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public interface IPasswordResetRepository
    {
        Task<PasswordReset> CreateAsync(PasswordReset pr);
        Task<PasswordReset?> GetActiveByEmailAsync(string email);
        Task MarkUsedAsync(long id);
        Task IncrementAttemptsAsync(long id);
        Task UpdateCodeAsync(long id, string newHash, System.DateTime newExpiryUtc);
    }
}
