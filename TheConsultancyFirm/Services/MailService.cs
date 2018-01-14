using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Models.Mail;

namespace TheConsultancyFirm.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public Task SendAccountCreatedMailAsync(string email, string password)
        {
            return SendMailAsync(email, "Er is een account aangemaakt voor u op de website.", $@"
Beste gebruiker,<br/>
<br/>
Er is een account voor u aangemaakt op de website.<br/>
Het wachtwoord voor de eerste keer inloggen in de applicatie is: <b>{password}</b><br/>
Als u voor de eerste keer inlogt, wordt u verwezen naar de wachtwoord veranderen pagina, waar u dan de optie heeft om uw wachtwoord aan te passen.<br/>
<br/>
Met vriendelijke groet,<br/>
Het TCF-team");
        }

        public Task SendForgotPasswordMailAsync(string email, string callbackUrl)
        {
            return SendMailAsync(email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
        }

        public Task SendEmailConfirmationAsync(string email, string link)
        {
            return SendMailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public Task SendContactMailAsync(Contact contact)
        {
            return SendMailAsync("16056205@student.hhs.nl", "Contact - " + contact.Subject, $@"
Een nieuw contact formulier is verstuurd:<br/>
<br/>
<b>Naam:</b> {contact.Name}<br/>
<b>Email:</b> {contact.Email}<br/>
<b>Telefoon nr.:</b> {contact.Mobile}<br/>
<b>Subject:</b> {contact.Subject}<br/>
<br/>
<b>Message:</b><br/><br/>
{contact.Message}
");
        }

        public Task SendMailAsync(string email, string subject, string message)
        {
            Execute(email, subject, message).Wait();
            return Task.FromResult(0);
        }

        private async Task Execute(string email, string subject, string message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName)
            };

            mail.To.Add(new MailAddress(email));

            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;

            using (var smtp = new SmtpClient(_mailSettings.SmtpHost, _mailSettings.SmtpPort))
            {
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.EmailPassword);
                
                await smtp.SendMailAsync(mail);
            }
        }
    }
}
