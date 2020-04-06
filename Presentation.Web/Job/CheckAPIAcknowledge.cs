using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Web.Job
{
    [DisallowConcurrentExecution]
    public class CheckAPIAcknowledgeJob : IJob
    {
        private readonly IServiceProvider provider;

        public CheckAPIAcknowledgeJob(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = provider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<CheckAPIAcknowledgeJob>>();
                var reportRepo = scope.ServiceProvider.GetService<IGenericRepository<Report>>();
                var min30ago = Utilities.ToUnixTime(DateTime.Now.AddMinutes(-30));
                var unAckedReports = reportRepo.AsQueryable().Where(r => (r.Status == ReportStatus.APIReady || r.Status == ReportStatus.APIFetched ) && r.ProcessedDateTimestamp <= min30ago);
                if (unAckedReports.Any())
                {
                    logger.LogCritical("Reports not acknowledged from on-premise service");
                }
                return Task.CompletedTask;
            }
        }
    }
}