using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Repository;

namespace Plantpedia.Service
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordResetRepository _resets;
        private readonly IEmailService _email;

        private const int CODE_LEN = 6;
        private const int EXPIRE_MIN = 10;
        private const int MAX_ATTEMPTS = 5;

        public ForgotPasswordService(
            IUserRepository users,
            IPasswordResetRepository resets,
            IEmailService email
        )
        {
            _users = users;
            _resets = resets;
            _email = email;
        }

        public async Task<bool> SendCodeAsync(string email)
        {
            // Chuẩn hoá email
            var e = (email ?? string.Empty).Trim().ToLowerInvariant();

            // kiểm tra tồn tại user/email
            if (!await _users.IsGmailExistsAsync(e))
            {
                LoggerHelper.Warn($"Yêu cầu quên mật khẩu cho email không tồn tại: {e}");
                // Tránh lộ thông tin, vẫn trả true
                return true;
            }

            // Lấy user theo EMAIL (không phải username)
            var userLogin = await _users.GetUserLoginDataByEmailAsync(e);
            var userId = userLogin?.User?.UserId ?? 0;

            // tạo/ghi đè mã
            var code = GenerateCode();
            var codeHash = Hash(code);
            var expiry = DateTime.UtcNow.AddMinutes(EXPIRE_MIN);

            var existing = await _resets.GetActiveByEmailAsync(e);
            if (existing == null)
            {
                await _resets.CreateAsync(
                    new PasswordReset
                    {
                        UserId = userId,
                        Email = e, // lưu email đã chuẩn hoá
                        CodeHash = codeHash,
                        ExpiresAtUtc = expiry,
                    }
                );
            }
            else
            {
                await _resets.UpdateCodeAsync(existing.Id, codeHash, expiry);
            }

            // gửi email
            var subject = "Mã xác nhận đặt lại mật khẩu - Plantpedia";
            var html =
                $@"
<div style=""font-family:Segoe UI,Arial,sans-serif;font-size:15px"">
  <p>Xin chào,</p>
  <p>Mã xác nhận đặt lại mật khẩu của bạn là:</p>
  <p style=""font-size:22px;font-weight:700;letter-spacing:2px"">{code}</p>
  <p>Mã có hiệu lực trong <b>{EXPIRE_MIN} phút</b>. Vui lòng không chia sẻ mã này.</p>
  <p>Trân trọng,<br/>Plantpedia</p>
</div>";
            await _email.SendAsync(e, subject, html);

            LoggerHelper.Info($"Đã gửi mã OTP quên mật khẩu tới {e}");
            return true;
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var e = (email ?? string.Empty).Trim().ToLowerInvariant();

            var pr = await _resets.GetActiveByEmailAsync(e);
            if (pr == null)
            {
                LoggerHelper.Warn($"OTP không tồn tại/đã hết hạn cho {e}");
                return false;
            }
            if (pr.Attempts >= MAX_ATTEMPTS)
            {
                LoggerHelper.Warn($"Vượt quá số lần thử OTP cho {e}");
                return false;
            }
            if (DateTime.UtcNow > pr.ExpiresAtUtc)
            {
                LoggerHelper.Warn($"OTP hết hạn cho {e}");
                return false;
            }

            var ok = SlowEquals(pr.CodeHash, Hash(code ?? string.Empty));
            await _resets.IncrementAttemptsAsync(pr.Id);
            return ok;
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var e = (email ?? string.Empty).Trim().ToLowerInvariant();

            var pr = await _resets.GetActiveByEmailAsync(e);
            if (pr == null)
                return false;
            if (pr.Attempts >= MAX_ATTEMPTS)
                return false;
            if (DateTime.UtcNow > pr.ExpiresAtUtc)
                return false;
            if (!SlowEquals(pr.CodeHash, Hash(code ?? string.Empty)))
                return false;

            // set password mới: LẤY USER THEO EMAIL
            var userLogin = await _users.GetUserLoginDataByEmailAsync(e);
            if (userLogin?.User == null)
                return false;

            var newLogin = AuthHelper.CreateUserLoginData(userLogin.UserId, e, newPassword);
            userLogin.PasswordHash = newLogin.PasswordHash;
            userLogin.PasswordSalt = newLogin.PasswordSalt;
            await _users.SaveChangesAsync();

            await _resets.MarkUsedAsync(pr.Id);
            LoggerHelper.Info($"Đã đặt lại mật khẩu cho {e}");
            return true;
        }

        // ===== Helpers =====
        private static string GenerateCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var num = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
            return num.ToString("D6");
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        private static bool SlowEquals(string a, string b)
        {
            if (a.Length != b.Length)
                return false;
            var diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
