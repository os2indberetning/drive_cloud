using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Presentation.Web.Auth;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData.Query;
using Newtonsoft.Json;
using Core.DomainServices;
using Core.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
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
                logEntry.Parameters = GetODataParameters(context.ActionArguments);
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

        private string GetODataParameters(IDictionary<string, object> parameters)
        {
            string result = null;
            var queryOptionsDictionaryEntry = parameters.Where(x => x.Value is ODataQueryOptions).FirstOrDefault();
            try
            {
                var queryOptions = queryOptionsDictionaryEntry.Value as ODataQueryOptions;
                if (queryOptions != null)
                {
                    Dictionary<string, string> rawValues = new Dictionary<string, string>();
                    rawValues.Add("Count", queryOptions.RawValues.Count);
                    rawValues.Add("Expand", queryOptions.RawValues.Expand);
                    rawValues.Add("Filter", queryOptions.RawValues.Filter);
                    rawValues.Add("Format", queryOptions.RawValues.Format);
                    rawValues.Add("OrderBy", queryOptions.RawValues.OrderBy);
                    rawValues.Add("Select", queryOptions.RawValues.Select);
                    rawValues.Add("Skip", queryOptions.RawValues.Skip);
                    rawValues.Add("SkipToken", queryOptions.RawValues.SkipToken);
                    rawValues.Add("Top", queryOptions.RawValues.Top);

                    parameters[queryOptionsDictionaryEntry.Key] = rawValues;
                    result = JsonConvert.SerializeObject(parameters);
                }
            }
            catch (Exception)
            {
                // No paramater of type ODataQueryOptions was found in actionContext.ActionArguments, which is fine.
            }
            return result;
        }
    }
}