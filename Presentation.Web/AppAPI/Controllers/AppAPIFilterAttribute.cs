using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Buffers;

namespace Presentation.Web.AppAPI.Controllers
{
    public class AppAPIFilterAttribute : ActionFilterAttribute
    {
        // takes care of the correct attribute casing (Camel Case) expected by the apps
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                };
                var jsonFormatter = new JsonOutputFormatter(serializerSettings,ArrayPool<char>.Shared);
                objectResult.Formatters.Add(jsonFormatter);
            }
            base.OnResultExecuting(context);
        }
    }
}
