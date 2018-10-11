using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BackEndServer.Services
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _sourceEmailAddress;

        public EmailService(string sourceEmailAddress, string sourceEmailPassword)
        {
            _sourceEmailAddress = sourceEmailAddress;
            _smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                //UseDefaultCredentials must be set to false before the Credentials are set.
                //Weird but very critical for the success of the email sending operation
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_sourceEmailAddress, sourceEmailPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        public bool SendEmail(string destinationAddress, 
                              string messageSubject, string messageBody)
        {
            try
            {
                MailMessage mail = CreateMail(destinationAddress, messageSubject, messageBody);
                _smtpClient.Send(mail);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private MailMessage CreateMail(string destinationAddress, string messageSubject, string messageBody, bool isBodyHtml = true)
        {
            MailMessage mail = new MailMessage(_sourceEmailAddress, destinationAddress)
            {
                Subject = messageSubject,
                Body = messageBody,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = isBodyHtml
            };
            return mail;
        }
    }
}