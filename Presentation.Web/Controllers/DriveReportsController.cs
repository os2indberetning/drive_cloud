using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace OS2Indberetning.Controllers
{
    public class DriveReportsController : BaseController<DriveReport>
    {
        private readonly IDriveReportService _driveService;
        //private readonly IGenericRepository<Employment> _employmentRepo;
        //private readonly IGenericRepository<Person> _personRepo;
        //private readonly IGenericRepository<OrgUnit> _orgrepo;
        //private readonly IGenericRepository<BankAccount> _Bankrepo;
        //private readonly IGenericRepository<LicensePlate> _LicensePlateRepo;
        private readonly ITransferToPayrollService _transferToPayrollService;


        public DriveReportsController(IServiceProvider provider) : base(provider)
        {
            _driveService = provider.GetService<IDriveReportService>();
            //_employmentRepo = provider.GetService<IGenericRepository<Employment>>();
            //_personRepo = provider.GetService<IGenericRepository<Person>>();
            //_orgrepo = provider.GetService<IGenericRepository<OrgUnit>>();
            //_Bankrepo = provider.GetService<IGenericRepository<BankAccount>>();
            //_LicensePlateRepo = provider.GetService<IGenericRepository<LicensePlate>>();
            _transferToPayrollService = provider.GetService<ITransferToPayrollService>();
        }

        // GET: odata/DriveReports
        /// <summary>
        /// ODATA GET API endpoint for drivereports.
        /// Converts string status to a ReportStatus enum and filters by it.
        /// Filters reports by leaderId and returns reports which that leader is responsible for approving.
        /// Does not return reports for which there is a substitute, unless getReportsWhereSubExists is true.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <param name="status"></param>
        /// <param name="leaderId"></param>
        /// <param name="getReportsWhereSubExists"></param>
        /// <returns>DriveReports</returns>
        [EnableQuery]
        public IActionResult Get(ODataQueryOptions<DriveReport> queryOptions, string from = "", string status = "", int leaderId = 0, bool getReportsWhereSubExists = false)
        {
            IQueryable<DriveReport> queryable = null;
            try
            {
                queryable = GetQueryable(queryOptions)
                    .Include(r => r.DriveReportPoints)
                    .Include(r => r.PersonReports)
                    .Include(r => r.Employment).ThenInclude(e => e.OrgUnit)
                    .Include(r => r.Person).ThenInclude(p => p.PersonalAddresses)
                    .Include(r => r.ApprovedBy);

                ReportStatus reportStatus;
                if (ReportStatus.TryParse(status, true, out reportStatus))
                {
                    if (reportStatus == ReportStatus.Accepted)
                    {
                        // If accepted reports are requested, then return accepted and invoiced. 
                        // Invoiced reports are accepted reports that have been processed for payment.
                        // So they are still accepted reports.
                        queryable =
                            queryable.Where(dr => dr.Status == ReportStatus.Accepted || dr.Status == ReportStatus.APIReady || dr.Status == ReportStatus.APIFetched ||  dr.Status == ReportStatus.Invoiced);
                    }
                    else
                    {
                        queryable = queryable.Where(dr => dr.Status == reportStatus);
                    }

                    switch (from)
                    {
                        case "approve":                                
                                queryable = queryable.Where(dr => dr.PersonReports.Any(pr => pr.Person.Id == CurrentUser.Id));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType().Name}, Get(), queryOption={queryOptions}, status={status}, leaderId={leaderId}, getReportsWhereSubExists={getReportsWhereSubExists}", ex);
            }
            return Ok(queryable);
        }

        [EnableQuery]
        public IActionResult Get(ODataQueryOptions<DriveReport> queryOptions, string queryType)
        {
            IQueryable<DriveReport> queryable = null;
            switch (queryType)
            {
                case "admin":
                    {
                        if (CurrentUser.IsAdmin)
                        {
                            queryable = GetQueryable(queryOptions);
                        }
                        else
                        {
                            return Unauthorized();
                        }
                    }
                    break;
                case "godkender":
                    {
                        if(CurrentUser.Employments.Any(em => em.IsLeader) || CurrentUser.SubstituteLeaders.Count > 0)
                        {
                            queryable = GetQueryable(queryOptions);
                        }
                        else
                        {
                            return Unauthorized();
                        }
                    }
                    break;
                case "mine":
                    {
                        queryable = GetQueryable(queryOptions).Where(dr => dr.PersonId == CurrentUser.Id);                        
                    }
                    break;
            }
            return Ok(queryable);
        }

        /// <summary>
        /// Returns the latest drivereport for a given user.
        /// Used for setting the option fields in DrivingView to the same as the latest report by the user.
        /// </summary>
        /// <param name="personId">Id of person to get report for.</param>
        /// <returns></returns>
        [EnableQuery]
        public IActionResult GetLatestReportForUser(int personId)
        {
            try
            {
                var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var report = Repo.AsQueryable()
                    .Where(
                        x => x.PersonId.Equals(personId)
                        && x.Employment.StartDateTimestamp < currentTimestamp
                        && (x.Employment.EndDateTimestamp > currentTimestamp || x.Employment.EndDateTimestamp == 0)
                        && !x.IsFromApp)
                    .OrderByDescending(x => x.CreatedDateTimestamp)
                    .FirstOrDefault();

                if (report != null)
                {
                    return Ok(report);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType().Name}, GetLatestReportForUser(), personId={personId}", ex);
            }
            _logger.LogDebug($"{GetType().Name}, GetLatestReportForUser(), personId={personId}, statusCode=204 No Content");
            return StatusCode(StatusCodes.Status204NoContent);
            
        }

        /// <summary>
        /// Returns a bool indicating if the special Norddjurs calculation method is configured to be used.
        /// Used for setting the available options in the kilometerallowance menu.
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCalculationMethod()
        {
            return Ok(Boolean.Parse(_configuration["AlternativeCalculationMethod"]));
        }

        public IActionResult TransferReportsToPayroll()
        {
            _logger.LogDebug($"{GetType().Name}, Get(), TransferReportsToPayroll initialized");
            if (!CurrentUser.IsAdmin)
            {
                _logger.LogError($"{GetType().Name}, Get(), {CurrentUser} is not admin, file generation aborted, Status code:403 Forbidden");
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            try
            {
                _transferToPayrollService.TransferReportsToPayroll();
                _logger.LogDebug($"{GetType().Name}, Get(), Transfer to payroll finished");
                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogError($"{GetType().Name}, Get(), Error when transfering reports to payroll, Status Code: 500 Internal Server Error", e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        //GET: odata/DriveReports(5)
        /// <summary>
        /// ODATA API endpoint for a single drivereport.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns>A single DriveReport</returns>
        public IActionResult GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            return Ok(GetQueryable(key, queryOptions));
        }

        // PUT: odata/DriveReports(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<DriveReport> delta)
        {
            return base.Put(key, delta);
        }

        // POST: odata/DriveReports
        /// <summary>
        /// ODATA POST api endpoint for drivereports.
        /// Returns forbidden if the user associated with the posted report is not the current user.
        /// </summary>
        /// <param name="driveReport"></param>
        /// <returns>The posted report.</returns>
        [EnableQuery]
        public IActionResult Post([FromBody] DriveReport driveReport, string emailText)
        {
            if (CurrentUser.IsAdmin && emailText != null && driveReport.Status == ReportStatus.Accepted)
            {
                // An admin is trying to edit an already approved report.
                var adminEditResult = _driveService.Create(driveReport);
                // CurrentUser is restored after the calculation.
                _driveService.SendMailToUserAndApproverOfEditedReport(adminEditResult, emailText, CurrentUser, "redigeret");
                return Ok(adminEditResult);
            }

            if (!CurrentUser.Id.Equals(driveReport.PersonId))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            try
            {
                var result = _driveService.Create(driveReport);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Could not create Report");
                return BadRequest(e);
            }
        }

        // PATCH: odata/DriveReports(5)
        /// <summary>
        /// PATCH API endpoint for drivereports.
        /// Returns forbidden if a user is trying to patch his/her own report or if the user is not the responsible leader for the report.
        /// Also returns forbidden if the report to be patched has a status other than pending.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <param name="emailText">The message to be sent to the owner of a report an admin has rejected or edited.</param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<DriveReport> delta, string emailText)
        {

            var report = Repo.AsQueryable().SingleOrDefault(x => x.Id == key);

            if (report == null)
            {
                return NotFound();
            }

            if (report.PersonReports.Count == 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (CurrentUser.IsAdmin && emailText != null && report.Status == ReportStatus.Accepted)
            {
                // An admin is trying to reject an approved report.
                report.Status = ReportStatus.Rejected;
                report.Comment = emailText;
                report.ClosedDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                report.ApprovedBy = CurrentUser;
                try
                {
                    Repo.Save();
                    if (report.FourKmRule)
                    {
                        _driveService.CalculateFourKmRuleForOtherReports(report); 
                    }
                    _driveService.SendMailToUserAndApproverOfEditedReport(report, emailText, CurrentUser, "afvist");
                    return Ok();
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"Fejl under forsøg på at afvise en allerede godkendt indberetning fra {report.Person.FullName}. Rapportens status er ikke ændret.");
                }
            }


            // Cannot approve own reports.
            if (report.PersonId == CurrentUser.Id)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            // Cannot approve reports where you are not responsible leader
            if (!report.IsPersonResponsible(CurrentUser.Id))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            // Return Unauthorized if the status is not pending when trying to patch.
            // User should not be allowed to change a Report which has been accepted or rejected.
            if (report.Status != ReportStatus.Pending)
            {
                _logger.LogWarning("Forsøg på at redigere indberetning med anden status end afventende. Rapportens status er ikke ændret.");
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var status = new object();
            if (delta.TryGetPropertyValue("Status", out status))
            {
                if (status.ToString().Equals("Rejected"))
                {
                    bool sendEmailResult = true;
                    try
                    {
                        base.Patch(key, delta);
                        _driveService.CalculateFourKmRuleForOtherReports(report);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{GetType().Name}, Patch(), Error when trying to update report status for user {report.Person.FullName}", ex);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                        
                    }

                    try
                    {
                        _driveService.SendMailForRejectedReport(key, delta);
                    }
                    catch
                    {
                        _logger.LogWarning($"{report.Person.FullName} har fået en indberetning afvist af sin leder, men er ikke blevet notificeret via email");
                        sendEmailResult = false;
                    }
                    return Ok(sendEmailResult);
                }
            }

            return base.Patch(key, delta);
        }

        // DELETE: odata/DriveReports(5)
        /// <summary>
        /// DELETE API endpoint for drivereports.
        /// Deletes the report identified by key if the current user is the owner of the report or is an admin.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            var report = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (report == null)
            {
                return NotFound();
            }
            if (report.PersonId.Equals(CurrentUser.Id) || CurrentUser.IsAdmin)
            {
                var deleteResult = base.Delete(key);
                if (report.FourKmRule)
                {
                    _driveService.CalculateFourKmRuleForOtherReports(report); 
                }
                return deleteResult;
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
