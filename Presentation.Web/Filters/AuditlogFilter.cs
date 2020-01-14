using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Presentation.Web.Auth;
using System;
using System.Security.Claims;

namespace OS2Indberetning.Filters
{
    public class AuditlogFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IGenericRepository<Auditlog> _auditlogRepo;
        private readonly UserManager<IdentityPerson> _userManager;
        private Person _loggedInPerson;

        public AuditlogFilter(IServiceProvider provider)
        {
            _logger = provider.GetService<ILogger<AuditlogFilter>>();
            _auditlogRepo = provider.GetService<IGenericRepository<Auditlog>>();
             _userManager = provider.GetService<UserManager<IdentityPerson>>();
        }

        private Person GetLoggedInPerson(ClaimsPrincipal claimsPrincipal)
        {
            if( _loggedInPerson == null)
            {
                var user = _userManager.GetUserAsync(claimsPrincipal);
                user.Wait();
                _loggedInPerson = user.Result.Person;
            }
            return _loggedInPerson;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var logEntry = new Auditlog();
                var person = GetLoggedInPerson(context.HttpContext.User);
                logEntry.UserId = person.Id;
                logEntry.User = person.FullName;
                logEntry.Location = context.HttpContext.Connection.RemoteIpAddress.ToString();
                logEntry.Action = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;
                logEntry.Controller = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
                logEntry.Parameters = GetODataParameters(context);
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

        private string GetODataParameters(ActionExecutingContext context)
        {
            try
            {
                return JsonConvert.SerializeObject(((ODataQueryOptions)context.ActionArguments["queryOptions"]).RawValues);
            }
            catch (Exception)
            {
                // No paramater of type ODataQueryOptions was found in actionContext.ActionArguments, which is fine.
                return null;
            }
        }
    }
}