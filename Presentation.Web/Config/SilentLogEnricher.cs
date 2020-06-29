using Microsoft.Extensions.Configuration;
using Serilog.Core;
using Serilog.Events;
using System.Linq;

namespace Presentation.Web.Config
{
    public class SilentLogEnricher : ILogEventEnricher
    {
        private string[] silentLogs;
        public SilentLogEnricher(IConfiguration configuration)
        {
            silentLogs = configuration.GetSection("SilentLogs").Get<string[]>() ?? new string[0];
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (silentLogs.Contains(logEvent.Exception?.Source))
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Silent", "SILENT-"));
            }
            
        }
    }
}
