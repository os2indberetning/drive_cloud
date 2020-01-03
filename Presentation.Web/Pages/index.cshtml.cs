using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Presentation.Web.Pages
{
    public class indexModel : PageModel
    {
        [FromQuery(Name = "loginFailed")]
        public bool loginFailed { get; set; }
        public string Version { get; set; }
        public indexModel(IConfiguration Configuration)
        {
            Version = Configuration.GetValue<string>("Version");
        }

        public IActionResult OnGet()
        {
            if (!loginFailed && !User.Identity.IsAuthenticated)
            {
                return Redirect("/SAML/Login");
            }
            return Page();
        }
    }
}