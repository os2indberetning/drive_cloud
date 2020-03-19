using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Web.Job;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace Presentation.Web.Config
{
    public static class JobConfig
    {
        public static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add Send Mail Job
            services.AddSingleton<SendMailJob>();
            services.AddSingleton(
                new JobSchedule(
                    typeof(SendMailJob),
                    configuration["SendMailJob:Schedule"],
                    Boolean.Parse(configuration["SendMailJob:Enabled"])));

            // Add Clean Audit Log job
            services.AddSingleton<CleanAuditLogJob>();
            services.AddSingleton(new JobSchedule(typeof(CleanAuditLogJob), "0 0 5 * * ? *", true));

            // Add Transfer payroll job
            services.AddSingleton<TransferPayrollJob>();
            services.AddSingleton(
                new JobSchedule(
                    typeof(TransferPayrollJob),
                    configuration["TransferPayrollJob:Schedule"],
                    Boolean.Parse(configuration["TransferPayrollJob:Enabled"])));

            services.AddHostedService<QuartzHostedService>();
            return services;
        }
    }
}