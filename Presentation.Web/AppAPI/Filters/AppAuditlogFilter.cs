using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Presentation.Web.AppAPI.ViewModels;
using System;

namespace Presentation.Web.AppAPI.Filters
{
    public class AppAuditlogFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IGenericRepository<Auditlog> _auditlogRepo;

        public AppAuditlogFilter(IServiceProvider provider)
        {
            _logger = provider.GetService<ILogger<AppAuditlogFilter>>();
            _auditlogRepo = provider.GetService<IGenericRepository<Auditlog>>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var logEntry = new Auditlog();
                logEntry.Location = context.HttpContext.Connection.RemoteIpAddress.ToString();
                logEntry.Action = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;
                logEntry.Controller = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
                logEntry.Parameters = JsonConvert.SerializeObject(context.ActionArguments, new JsonSerializerSettings()
                {
                    ContractResolver = new IgnorePropertiesResolver(new[] { "GuId", "Password" })
                });
                logEntry.Timestamp = DateTime.Now;
                _auditlogRepo.Insert(logEntry);
                _auditlogRepo.Save();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OnActionExecuting(), Auditlogging failed.");
                context.Result = new BadRequestResult();
            }
            base.OnActionExecuting(context);
        }
    }
}