using System.Threading.Tasks;

namespace Plantpedia.Service
{
    public interface IForgotPasswordService
    {
        Task<bool> SendCodeAsync(string email); // gửi OTP
        Task<bool> VerifyCodeAsync(string email, string code); // xác nhận OTP
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
    }
}
