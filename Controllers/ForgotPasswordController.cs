using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Controllers
{
    [Route("auth/forgot")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _svc;

        public ForgotPasswordController(IForgotPasswordService svc) => _svc = svc;

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromForm] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { success = false, message = "Vui lòng nhập email." });

            var ok = await _svc.SendCodeAsync(email.Trim());
            LoggerHelper.Info($"Yêu cầu gửi OTP cho {email}: {(ok ? "OK" : "FAIL")}");
            return Ok(new { success = true, message = "Nếu email tồn tại, mã đã được gửi." });
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromForm] string email, [FromForm] string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
                return BadRequest(new { success = false, message = "Thiếu email hoặc mã." });

            var ok = await _svc.VerifyCodeAsync(email.Trim(), code.Trim());
            return Ok(new { success = ok });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset(
            [FromForm] string email,
            [FromForm] string code,
            [FromForm] string newPassword
        )
        {
            if (
                string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(code)
                || string.IsNullOrWhiteSpace(newPassword)
            )
                return BadRequest(new { success = false, message = "Thiếu dữ liệu." });

            if (newPassword.Length < 6)
                return BadRequest(new { success = false, message = "Mật khẩu tối thiểu 6 ký tự." });

            var ok = await _svc.ResetPasswordAsync(email.Trim(), code.Trim(), newPassword);
            return Ok(
                new
                {
                    success = ok,
                    message = ok
                        ? "Đổi mật khẩu thành công. Vui lòng đăng nhập lại."
                        : "Mã không hợp lệ hoặc đã hết hạn.",
                }
            );
        }
    }
}
