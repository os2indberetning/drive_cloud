using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailService : IMailService
    {
        private readonly IGenericRepository<DriveReport> _driveRepo;
        private readonly IGenericRepository<Substitute> _subRepo;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly IMailSender _mailSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public MailService(IGenericRepository<DriveReport> driveRepo, IGenericRepository<Substitute> subRepo, IGenericRepository<Person> personRepo, IMailSender mailSender, ILogger<MailService> logger, IConfiguration configuration)
        {
            _driveRepo = driveRepo;
            _subRepo = subRepo;
            _personRepo = personRepo;
            _mailSender = mailSender;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Sends an email to all leaders with pending reports to be approved.
        /// </summary>
        public void SendMails(DateTime payRoleDateTime, string customText)
        {
            var mailAddresses = GetLeadersWithPendingReportsMails();
            var mailBody = "";
            if (string.IsNullOrEmpty(customText))
            {
                mailBody = _configuration["Mail:DriveMail:Body"];
                if (string.IsNullOrEmpty(mailBody))
                {
                    _logger.LogDebug($"{this.GetType().Name}, SendMails(): Mail body is null or empty, check value in CustomSettings.config");
                }
            }
            else
            {
                mailBody = customText;
            }
            mailBody = mailBody.Replace("####", payRoleDateTime.ToString("dd-MM-yyyy"));


            var mailSubject = _configuration["Mail:DriveMail:Subject"];
            if (string.IsNullOrEmpty(mailSubject))
            {
                _logger.LogDebug($"{this.GetType().Name}, SendMails(): Mail subject is null or empty, check value in CustomSettings.config");
            }

            foreach (var mailAddress in mailAddresses)
            {
                _mailSender.SendMail(mailAddress, mailSubject, mailBody);
            }
        }

        /// <summary>
        /// Gets the email address of all leaders that have pending reports to be approved.
        /// </summary>
        /// <returns>List of email addresses.</returns>
        public IEnumerable<string> GetLeadersWithPendingReportsMails()
        {
            var approverEmails = new List<String>();

            var reports = _driveRepo.AsQueryable().Where(r => r.Status == ReportStatus.Pending).ToList();

            var reportsWithNoLeader = reports.Where(driveReport => driveReport.ResponsibleLeaders.Count == 0);

            foreach (var report in reportsWithNoLeader)
            {
                _logger.LogError($"{report.Person.FullName}s indberetning har ingen leder. Indberetningen kan derfor ikke godkendes.");
                _logger.LogError($"{this.GetType().Name}, GetLeadersWithPendingReportsMails(): {report.Person.FullName}s indberetning har ingen leder. Indberetningen kan derfor ikke godkendes.");
            }

            foreach (var driveReport in reports)
            {
                approverEmails.AddRange(driveReport.ResponsibleLeaders.Where(p => !string.IsNullOrEmpty(p.Mail) && p.RecieveMail).Select(p => p.Mail));
            }

            return approverEmails.Distinct();
        }

        public void SendMailToAdmins(string subject, string text)
        {
            var adminEmailAdresses = _personRepo.AsQueryable().Where(p => p.IsActive && p.IsAdmin && p.AdminRecieveMail && !string.IsNullOrEmpty(p.Mail)).Select(p => p.Mail);

            foreach (var emailAddress in adminEmailAdresses)
            {
                _mailSender.SendMail(emailAddress, subject, text);
            } 
        }

        public void SendMail(string toAddress, string subject, string text)
        {
            if (!string.IsNullOrEmpty(toAddress))
            {
                _mailSender.SendMail(toAddress, subject, text); 
            }
            else
            {
                throw new ArgumentException("Receiving emailaddress can not be null or empty", "toAddress");
            }
        }
    }
}
