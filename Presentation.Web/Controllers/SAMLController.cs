using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Web.Auth;
using Sustainsys.Saml2.AspNetCore2;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Web.Controllers.API
{
    [Route("[Controller]/[action]")]
    [ApiController]
    public class SAMLController : ControllerBase
    {
        private readonly SignInManager<IdentityPerson> _signInManager;
        private readonly UserManager<IdentityPerson> _userManager;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly ILogger<SAMLController> _logger;

        public SAMLController(SignInManager<IdentityPerson> signinManager, UserManager<IdentityPerson> userManager, IGenericRepository<Person> personRepo, ILogger<SAMLController> logger)
        {
            _signInManager = signinManager;
            _userManager = userManager;
            _personRepo = personRepo;
            _logger = logger;
        }

        public ActionResult Login()
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(Saml2Defaults.Scheme, "/SAML/Callback");
            return new ChallengeResult(Saml2Defaults.Scheme, properties);
        }

        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return SignOut(new AuthenticationProperties(){RedirectUri = "/Index"},Saml2Defaults.Scheme);            
        }

        public async Task<ActionResult> Callback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Could not get ExternalLoginInfo - Name Id claim was probably not sent correctly");
                return Redirect("/index?loginFailed=true");
            }
            _logger.LogDebug("Received Claims {claims}", info.Principal.Claims);
            _signInManager.ClaimsFactory = new Saml2ClaimsFactory(_signInManager.ClaimsFactory, info);
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                // update user admin and email field from claims
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                user.Person.IsAdmin = info.Principal.Claims.Any(c => c.Type == "roles" && c.Value == "administrator");
                var email = info.Principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault();
                if (email != null)
                {
                    user.Person.Mail = email.Value;
                }
                _personRepo.Save();
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", user.Person.FullName, info.LoginProvider);
                return Redirect("/index");
            }
            else
            {
                return Redirect("/index?loginFailed=true");
            }
        }
    }
}