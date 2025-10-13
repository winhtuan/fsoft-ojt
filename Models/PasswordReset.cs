using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    public class PasswordReset
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = default!;
        public string CodeHash { get; set; } = default!; // hash mã 6 số
        public DateTime ExpiresAtUtc { get; set; }
        public int Attempts { get; set; } = 0;
        public bool Used { get; set; } = false;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
