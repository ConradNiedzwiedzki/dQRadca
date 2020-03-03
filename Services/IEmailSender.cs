using System.Threading.Tasks;

namespace dQRadca.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
