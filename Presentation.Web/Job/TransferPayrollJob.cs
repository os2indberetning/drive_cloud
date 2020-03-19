using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Presentation.Web.Job
{
    [DisallowConcurrentExecution]
    public class TransferPayrollJob : IJob
    {
        private readonly IServiceProvider provider;

        public TransferPayrollJob(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = provider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<TransferPayrollJob>>();
                logger.LogInformation("Executing TransferPayrollJob");

                var fileRepo = scope.ServiceProvider.GetService<IGenericRepository<FileGenerationSchedule>>();
                var mailRepo = scope.ServiceProvider.GetService<IGenericRepository<MailNotificationSchedule>>();
                var transferToPayrollService = scope.ServiceProvider.GetService<ITransferToPayrollService>();

                var startOfDay = Utilities.ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
                var endOfDay = Utilities.ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));

                // Filter the repository with the files that need to be generated today
                var filesToGenerate = fileRepo.AsQueryable().Where(r => r.DateTimestamp >= startOfDay && r.DateTimestamp <= endOfDay && !r.Completed).ToList();

                logger.LogInformation($"Found {filesToGenerate.Count} scheduled payroll transfers" );

                if (filesToGenerate.Any())
                {

                    foreach (var file in filesToGenerate)
                    {
                        transferToPayrollService.TransferReportsToPayroll();

                        // Set file generation complete
                        file.Completed = true;
                        fileRepo.Save();

                        // Check if Repeat is true and schedule all mail notification and file gen jobs for the next month
                        if (file.Repeat)
                        {
                            var newDate = Utilities.ToUnixTime(Utilities.FromUnixTime(file.DateTimestamp).AddMonths(1));
                            try
                            {
                                var newFile = fileRepo.Insert(new FileGenerationSchedule
                                {
                                    DateTimestamp = newDate,
                                    Repeat = true,
                                    Completed = false,
                                });
                                fileRepo.Save();

                                // Check all mail notifications
                                if (file.MailNotificationSchedules != null && file.MailNotificationSchedules.Any())
                                {
                                    foreach (var mail in file.MailNotificationSchedules)
                                    {
                                        var newDateTime = Utilities.ToUnixTime(Utilities.FromUnixTime(mail.DateTimestamp).AddMonths(1));
                                        var newNotification = mailRepo.Insert(new MailNotificationSchedule()
                                        {
                                            FileGenerationScheduleId = newFile.Id, // ?
                                            DateTimestamp = newDateTime,
                                            CustomText = mail.CustomText
                                        });
                                    }
                                    mailRepo.Save();
                                }
                            }
                            catch (Exception e)
                            {                                                                
                                logger.LogCritical($"File generation was not rescheduled");
                            }
                        }
                    }
                }
                
                return Task.CompletedTask;
            }
        }
    }
}