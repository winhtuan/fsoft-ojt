using System.Threading.Tasks;

namespace Plantpedia.Service
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}
