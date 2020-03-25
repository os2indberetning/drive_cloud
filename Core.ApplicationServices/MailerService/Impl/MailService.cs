using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailService : IMailService
    {
        private readonly IGenericRepository<DriveReport> _driveRepo;
        private readonly IGenericRepository<Substitute> _subRepo;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly IGenericRepository<MailNotificationSchedule> _mailScheduleRepo;
        private readonly IGenericRepository<CachedAddress> _cachedAddressRepo;
        private readonly IMailSender _mailSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;


        public MailService(IServiceProvider provider)
        {
            _driveRepo = provider.GetService<IGenericRepository<DriveReport>>();
            _subRepo = provider.GetService<IGenericRepository<Substitute>>();
            _personRepo = provider.GetService<IGenericRepository<Person>>();
            _mailScheduleRepo = provider.GetService<IGenericRepository<MailNotificationSchedule>>();
            _cachedAddressRepo = provider.GetService<IGenericRepository<CachedAddress>>();
            _mailSender = provider.GetService<IMailSender>();
            _logger = provider.GetService<ILogger<MailService>>();
            _configuration = provider.GetService<IConfiguration>();
        }


        public void SendMails()
        {
            SendDirtyAddressesMails();
            SendMailsToLeadersWithPendingReports();
        }

        private void SendDirtyAddressesMails()
        {
            var dirtyAddressCount = _cachedAddressRepo.AsQueryable().Count(x => x.IsDirty);
            if (dirtyAddressCount > 0)
            {
                SendMailToAdmins("Der er adresser der mangler at blive vasket", "Der mangler at blive vasket " + dirtyAddressCount + " adresser");
            }
        }

        /// <summary>
        /// Sends an email to all leaders with pending reports to be approved.
        /// </summary>
        private void SendMailsToLeadersWithPendingReports()
        {
            var startOfDay = Utilities.ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
            var endOfDay = Utilities.ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));

            var notifications = _mailScheduleRepo.AsQueryable().Where(r => r.DateTimestamp >= startOfDay && r.DateTimestamp <= endOfDay);

            if (!notifications.Any())
            {
                _logger.LogDebug("No notifications for leaders found for today");
                return;
            }


            foreach (var notification in notifications.ToList())
            {

                var mailAddresses = GetLeadersWithPendingReportsMails();
                var mailBody = _configuration["Mail:DriveMail:Body"];
                mailBody = mailBody.Replace("####", Utilities.FromUnixTime(notification.FileGenerationSchedule.DateTimestamp).ToString("dd-MM-yyyy"));
                if (!String.IsNullOrEmpty(notification.CustomText))
                {
                    mailBody += $"\n\n{notification.CustomText}";
                }                
                var mailSubject = _configuration["Mail:DriveMail:Subject"];

                foreach (var mailAddress in mailAddresses)
                {
                    _mailSender.SendMail(mailAddress, mailSubject, mailBody);
                }
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

            var reportsWithNoLeader = reports.Where(driveReport => driveReport.PersonReports.Count == 0);

            foreach (var report in reportsWithNoLeader)
            {
                _logger.LogError($"{report.Person.FullName}s indberetning har ingen leder. Indberetningen kan derfor ikke godkendes.");
                _logger.LogError($"{this.GetType().Name}, GetLeadersWithPendingReportsMails(): {report.Person.FullName}s indberetning har ingen leder. Indberetningen kan derfor ikke godkendes.");
            }

            foreach (var driveReport in reports)
            {
                approverEmails.AddRange(driveReport.PersonReports.Where(p => !string.IsNullOrEmpty(p.Person.Mail) && p.Person.RecieveMail).Select(p => p.Person.Mail));
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
