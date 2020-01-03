using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace OS2Indberetning.Controllers
{
    public class FileController : BaseController<DriveReport>
    {
        private readonly ITransferToPayrollService _transferToPayrollService;
        public FileController(IServiceProvider provider) : base(provider)
        {
            _transferToPayrollService = provider.GetService<ITransferToPayrollService>();
        }

        //GET: Generate KMD File
        /// <summary>
        /// Generates KMD file.
        /// </summary>
        /// <returns></returns>
        public IActionResult Get()
        {
            _logger.LogDebug($"{GetType().Name}, Get(), Generate KMD file initialized");
            if (!CurrentUser.IsAdmin)
            {
                _logger.LogError($"{GetType().Name}, Get(), {CurrentUser} is not admin, file generation aborted, Status code:403 Forbidden");
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            try
            {
                _transferToPayrollService.TransferReportsToPayroll();
                _logger.LogDebug($"{GetType().Name}, Get(), Transfer to payroll finished");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{GetType().Name}, Get(), Error when transfering reports to payroll, Status Code: 500 Internal Server Error", e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}