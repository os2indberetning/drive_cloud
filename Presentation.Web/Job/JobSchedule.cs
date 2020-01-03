using System;

namespace Presentation.Web.Job
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression, bool enabled)
        {
            JobType = jobType;
            CronExpression = cronExpression;
            Enabled = enabled;
        }

        public Type JobType { get; }
        public string CronExpression { get; }
        public bool Enabled { get; set; }
    }
}
