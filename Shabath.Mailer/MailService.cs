using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace Shabath.Mailer
{
    public static class MailService
    {
        public static void SendMail(string[] participateMembersEmails, DateTime eventDate)
        {
            string testParticipateEmails = string.Empty;
            MailMessage mail = new MailMessage();
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("064e170540acc7", "108404d87ffa64"),
                EnableSsl = true
            };
            mail.From = new MailAddress("amirf@test.com");
            foreach (var email in participateMembersEmails)
            {
                testParticipateEmails += email;
            }

            mail.To.Add("f2fe380b1e-486fa6@inbox.mailtrap.io");
            mail.Subject = "this is a test email. to " + testParticipateEmails;
            mail.Body = "this is my test email body eventdate " + eventDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            client.Send(mail);
        }
    }
}
