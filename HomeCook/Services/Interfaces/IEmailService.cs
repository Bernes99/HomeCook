using HomeCook.Data.Models;

namespace HomeCook.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfirmationEmail(AppUser user, string code);
        Task SendForgotPasswordEmail(AppUser user, string code);
    }
}