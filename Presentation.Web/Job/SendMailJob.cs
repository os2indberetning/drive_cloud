using Core.ApplicationServices.MailerService.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Presentation.Web.Job
{
    [DisallowConcurrentExecution]
    public class SendMailJob : IJob
    {
        private readonly IServiceProvider provider;

        public SendMailJob(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = provider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<SendMailJob>>();
                var mailService = scope.ServiceProvider.GetService<IMailService>();
                logger.LogInformation("Executing SendMailJob");
                mailService.SendMails();
                return Task.CompletedTask;
            }
        }
    }
}