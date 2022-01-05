using System.Net;
using System.Net.Mail;

namespace WebApplication
{
    public class EmailSender : IEmailSender
    {
        public void SendGreetingsEmail(string email)
        {
            if (!email.Contains("gmail.com"))
                return;
            
            var fromAddress = new MailAddress("prlab2acc@gmail.com");
            var toAddress = new MailAddress(email);
            const string fromPassword = "unbreakable";
            const string subject = "Greetings!";
            const string body = "Dear Friend!\nThank you for registration!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                // DeliveryMethod = SmtpDeliveryMethod.Network,
                // UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
                   {
                       Subject = subject,
                       Body = body
                   })
            {
                smtp.Send(message);
            }            
        }
    }
}