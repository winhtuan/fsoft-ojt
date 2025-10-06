using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Service;
using Plantpedia.ViewModel;

namespace Plantpedia.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        // TempData keys
        private const string REGISTER_ERRORS = "RegisterErrors";
        private const string REGISTER_MODEL = "RegisterModel";
        private const string LOGIN_ERRORS = "LoginErrors";

        public AuthController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // POST /auth/register  (form đăng ký submit tới đây)
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            [FromForm] RegisterViewModel model,
            [FromQuery] string? returnUrl = null
        )
        {
            LoggerHelper.Info($"Bắt đầu đăng ký qua Controller với email: {model?.Email}");

            // Validate phía server theo DataAnnotations
            if (!ModelState.IsValid)
            {
                PutRegisterErrorsToTempData(ModelStateToMessages(ModelState));
                PutRegisterModelToTempData(model!);
                return RedirectToPage(
                    "/Auth/Login",
                    new { showRegister = true, ReturnUrl = returnUrl }
                );
            }

            try
            {
                // email đã tồn tại?
                var existing = await _userService.GetUserByUsernameAsync(model!.Email);
                if (existing != null)
                {
                    PutRegisterErrorsToTempData(new[] { "Email đã được sử dụng." });
                    PutRegisterModelToTempData(model);
                    return RedirectToPage(
                        "/Auth/Login",
                        new { showRegister = true, ReturnUrl = returnUrl }
                    );
                }

                // tạo user
                var newUser = await _userService.RegisterAsync(model);
                LoggerHelper.Info(
                    $"Đăng ký thành công: UserId={newUser.UserId}, Email={model.Email}"
                );

                // đăng nhập ngay
                await SignInAsync(newUser, model.Email);

                // điều hướng sau đăng ký
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                var role = newUser.LoginData.Role.ToString().ToLowerInvariant();
                return role == "user"
                    ? RedirectToPage("/Home/Home")
                    : RedirectToPage("/Admin/Admin");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Đăng ký thất bại (Controller) với email: {model?.Email}");
                PutRegisterErrorsToTempData(new[] { $"Đăng ký thất bại: {ex.Message}" });
                PutRegisterModelToTempData(model!);
                return RedirectToPage(
                    "/Auth/Login",
                    new { showRegister = true, ReturnUrl = returnUrl }
                );
            }
        }

        // POST /auth/login  nếu muốn xử lý đăng nhập ở Controller
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [FromForm] string username,
            [FromForm] string password,
            [FromQuery] string? returnUrl = null
        )
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData[LOGIN_ERRORS] = JsonSerializer.Serialize(
                    new[] { "Vui lòng nhập đầy đủ thông tin đăng nhập." }
                );
                return RedirectToPage("/Auth/Login", new { ReturnUrl = returnUrl });
            }

            try
            {
                var user = await _userService.LoginAsync(username, password);
                if (user == null)
                {
                    TempData[LOGIN_ERRORS] = JsonSerializer.Serialize(
                        new[] { "Tên đăng nhập hoặc mật khẩu không đúng!" }
                    );
                    return RedirectToPage("/Auth/Login", new { ReturnUrl = returnUrl });
                }

                await SignInAsync(user, user.LoginData.Username);

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                var role = user.LoginData.Role.ToString().ToLowerInvariant();
                return role == "user"
                    ? RedirectToPage("/Home/Home")
                    : RedirectToPage("/Admin/Admin");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi hệ thống khi đăng nhập (Controller).");
                TempData[LOGIN_ERRORS] = JsonSerializer.Serialize(
                    new[] { "Có lỗi hệ thống. Vui lòng thử lại sau." }
                );
                return RedirectToPage("/Auth/Login", new { ReturnUrl = returnUrl });
            }
        }

        // POST /auth/logout
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Auth/Login");
        }

        // ----------------- helpers -----------------
        private async Task SignInAsync(UserAccount user, string displayNameOrEmail)
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
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
        }

        private static IEnumerable<string> ModelStateToMessages(
            Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState
        )
        {
            return modelState
                .Values.SelectMany(v => v.Errors)
                .Select(e =>
                    string.IsNullOrEmpty(e.ErrorMessage) ? "Giá trị không hợp lệ." : e.ErrorMessage
                );
        }

        private void PutRegisterErrorsToTempData(IEnumerable<string> errors)
        {
            TempData[REGISTER_ERRORS] = JsonSerializer.Serialize(errors);
        }

        private void PutRegisterModelToTempData(RegisterViewModel model)
        {
            TempData[REGISTER_MODEL] = JsonSerializer.Serialize(model);
        }
    }
}
