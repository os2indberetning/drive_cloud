using System;
using System.Net;
using System.Net.Mail;
using Core.ApplicationServices.MailerService.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailSender : IMailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly ILogger<MailSender> logger;
        private readonly IConfiguration config;

        public MailSender(ILogger<MailSender> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
            _smtpClient = new SmtpClient()
            {
                Host = this.config["Mail:Host"],
                Port = int.Parse(this.config["Mail:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential()
                {
                    UserName = this.config["Mail:User"],
                    Password = this.config["Mail:Password"]
                }
            };

        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="to">Email address of recipient.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body of the email.</param>
        public void SendMail(string to, string subject, string body)
        {
            if (!Boolean.Parse(config["Mail:ServiceEnabled"]))
            {
                logger.LogWarning("MailService is disabled. Tried to send mail.\nTo: {0}\nSubject: {1}\nBody: {2}", to, subject, body);
                return;
            }

            if (String.IsNullOrWhiteSpace(to))
            {
                logger.LogWarning("Attempted to send mail, but 'to' field is empty.\nSubject: {0}\nBody: {1}", subject, body);
                return;
            }
            var msg = new MailMessage();
            msg.To.Add(to);
            msg.From = new MailAddress(config["Mail:FromAddress"]);
            msg.Body = body;
            msg.Subject = subject;
            try
            {
                _smtpClient.Send(msg);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Fejl under afsendelse af mail. Mail er ikke afsendt.");
            }
        }
    }
}
