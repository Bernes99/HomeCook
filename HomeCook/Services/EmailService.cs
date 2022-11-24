using HomeCook.Data.Models;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;

namespace HomeCook.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration Configuration;
        private readonly SmtpClient EmailClient;

        public EmailService(IConfiguration config, UserManager<AppUser> userManager)
        {
            Configuration = config;

            EmailClient = new SmtpClient() 
            {
                Port = int.Parse(Configuration["Email:Port"]),
                EnableSsl = Boolean.Parse( Configuration["Email:SSL"]),
                
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = Configuration["Email:Host"],
                Credentials  = new NetworkCredential(Configuration["Email:Username"], Configuration["Email:Password"])
        };
        }

        public async Task SendEmailConfirmationEmail(AppUser user, string code)
        {
            try
            {
                var emailBody = "Please confirm your email address <a target=\"_blank\" href=\"{0}\">Click here!</a>";
                var link = Configuration["FrontEndUrl"] + "auth/verifyEmail?userid=" + user.Id + "&code=" + code;

                var subject = "HomeCook: Email confirmation";

                var message = new MailMessage()
                {
                    From = new MailAddress(Configuration["Email:From:NoReply"]),
                    Subject = subject,
                    Body = String.Format(emailBody, link),
                    IsBodyHtml = true
                };
                message.To.Add(user.Email);
                #region Attachments
                //Attachment attachment;
                //attachment = new Attachment(attachmentFileName);
                //message.Attachments.Add(attachment);
                #endregion

                EmailClient.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task SendForgotPasswordEmail(AppUser user, string resetToken)
        {
            try
            {
                var emailBody = "Click if you sent request for password reset: <a target=\"_blank\" href=\"{0}\">Click here!</a>";
                var link = Configuration["FrontEndUrl"] + "auth/resetpassword?userid=" + user.Id + "&resetToken=" + resetToken;

                var subject = "HomeCook: Reset Password";

                var message = new MailMessage()
                {
                    From = new MailAddress(Configuration["Email:From:NoReply"]),
                    Subject = subject,
                    Body = String.Format(emailBody, link),
                    IsBodyHtml = true
                };
                message.To.Add(user.Email);
                #region Attachments
                //Attachment attachment;
                //attachment = new Attachment(attachmentFileName);
                //message.Attachments.Add(attachment);
                #endregion

                EmailClient.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
