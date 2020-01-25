using Core.ApplicationServices;
using Core.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Web.Auth;
using System.Collections.Generic;

namespace Presentation.Web.API.Controllers
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

        [HttpGet]
        public ActionResult<IEnumerable<APIReportDTO>> GetReportsToPayroll()
        {
            var reportsToPayroll = _apiService.GetReportsToPayroll();
            return Ok(reportsToPayroll);
        }

        [HttpPost]
        public void AcknowledgeReportsProcessed(int[] reportIds)
        {
            _apiService.AcknowledgeReportsProcessed(reportIds);
        }


    }
}
