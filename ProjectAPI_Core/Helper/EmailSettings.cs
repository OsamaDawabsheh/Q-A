using System.Net.Mail;
using System.Net;
using ProjectAPI_Core.DTOs.Auth;

namespace ProjectAPI.Helper
{
    public class EmailSettings
    {
        public static void SendEmail(EmailDto email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("omad10200099@gmail.com", "gxsu lasv asjg sppv");
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("omad10200099@gmail.com");
            msg.Subject = email.Subject;
            msg.Body = email.Body;
            msg.To.Add(email.Receiver);
            msg.IsBodyHtml = true;
            client.Send(msg);
        }
    }
}
