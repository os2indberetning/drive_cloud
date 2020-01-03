using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Presentation.Web.Auth
{
    internal class APIAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        internal const string AuthenticationScheme = "api";
        private readonly string apiKey;
        public APIAuthenticationHandler(IConfiguration configuration, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            apiKey = configuration.GetValue<string>("ApiKey");
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (Request.Headers["ApiKey"] == apiKey)
            {
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(Scheme.Name)), Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            else
            {
                return AuthenticateResult.Fail("Invalid APIKey");
            }
        }
    }
}
