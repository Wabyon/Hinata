using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace Hinata.SendGrid
{
    public class EmailService : IIdentityMessageService
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string SendGridUserName { get; set; }
        public string SendGridPassword { get; set; }

        public Task SendAsync(IdentityMessage message)
        {
            var sgMsg = new SendGridMessage();
            sgMsg.AddTo(message.Destination);
            sgMsg.From = new MailAddress(FromEmail, FromName);
            sgMsg.Subject = message.Subject;
            sgMsg.Text = message.Body;
            sgMsg.Html = message.Body;
            var credentials = new NetworkCredential(SendGridUserName, SendGridPassword);
            var sgWeb = new Web(credentials);
            return sgWeb.DeliverAsync(sgMsg);
        }
    }
}
