using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BackEndServer.Services
{
    public class EmailService
    {
        public static string SOURCE_ADDRESS = "projectjetson1@gmail.com";
        //FOR NOW, FILL IN PASSWORD MANUALY BEFORE COMPILING
        //IMPORTANT: REMOVE PASSWORD BEFORE PUSHING TO REMOTE
        public static string SOURCE_ADDRESS_PASSWORD = "";
        public static SmtpClient SMTP_CLIENT = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            //UseDefaultCredentials must be set to false before the Credentials are set.
            //Weird but very critical for the success of the email sending operation
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(SOURCE_ADDRESS, SOURCE_ADDRESS_PASSWORD),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
        
        public static bool SendEmail(string destinationAddress, 
                              string messageSubject, string messageBody)
        {
            try
            {
                MailMessage mail = CreateMail(destinationAddress, messageSubject, messageBody);
                SMTP_CLIENT.Send(mail);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static MailMessage CreateMail(string destinationAddress, string messageSubject, string messageBody, bool isBodyHtml = true)
        {
            MailMessage mail = new MailMessage(SOURCE_ADDRESS, destinationAddress)
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