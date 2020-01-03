using Core.ApplicationServices;
using Core.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Web.Auth;

namespace Presentation.Web.Controllers.API
{
    [Route("api/[action]")]
    [Authorize(AuthenticationSchemes = APIAuthenticationHandler.AuthenticationScheme)]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly APIService _apiService;

        public APIController(APIService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        public void UpdateOrganization([FromBody] APIOrganizationDTO apiOrganizationDTO)
        {
            _apiService.UpdateOrganization(apiOrganizationDTO);
        }
    }
}
