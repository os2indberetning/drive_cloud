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
    public class CleanAuditLogJob : IJob
    {
        private readonly IServiceProvider provider;

        public CleanAuditLogJob(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = provider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<CleanAuditLogJob>>();
                var auditlogRepo = scope.ServiceProvider.GetService<IGenericRepository<Auditlog>>();
                logger.LogInformation("Cleaning auditlog");
                var oldLogs = auditlogRepo.AsQueryableLazy().Where(a => a.Timestamp <= DateTime.Now.AddMonths(-6));
                auditlogRepo.DeleteRange(oldLogs);
                auditlogRepo.Save();
                return Task.CompletedTask;
            }
        }
    }
}