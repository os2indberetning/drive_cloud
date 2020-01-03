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

            // Add our job
            services.AddSingleton<SendMailJob>();
            services.AddSingleton(
                new JobSchedule(
                    typeof(SendMailJob),
                    configuration["SendMailJob:Schedule"],
                    Boolean.Parse(configuration["SendMailJob:Enabled"])));

            services.AddHostedService<QuartzHostedService>();
            return services;
        }
    }
}