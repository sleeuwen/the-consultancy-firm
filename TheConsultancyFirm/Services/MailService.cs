using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
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
