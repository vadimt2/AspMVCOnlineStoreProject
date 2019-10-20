using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Email
    {
        public static Email _email;
        private static readonly object rootAsync = new object (); 
        public static Email EmailClass
        {
            get
            {
                lock (rootAsync)
                {
                    if (_email == null)
                    {
                        _email = new Email();
                    } 
                }
                return _email;
            }
        }

        public async Task<bool> SendEmailAsync(User user, string subject, string bodyMessage)
        {

            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(user.Email));  // replace with valid value 
            message.From = new MailAddress("vadimt2@gmail.com");  // replace with valid value
            message.Subject = subject;
            message.Body = string.Format(body, "ClassFiedProj", "System Email", bodyMessage);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "vadimt2@gmail.com", 
                    Password = "" 
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                return true;
            }
        }
    }
}
