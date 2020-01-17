using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Web.AppAPI.ViewModels;
using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Core.DomainServices;
using Core.DomainModel;
using System.Linq;
using static Presentation.Web.AppAPI.Controllers.ErrorHandler;

namespace Presentation.Web.AppAPI.Controllers
{
    [ApiController]
    [AppAPIFilter]
    public class ReportController : ControllerBase
    {

        private readonly IGenericRepository<Rate> rateRepo;
        private readonly IGenericRepository<UserAuth> authRepo;
        private readonly IGenericRepository<Person> personRepo;
        private readonly IGenericRepository<DriveReport> driveReportRepo;
        private readonly IGenericRepository<AppLogin> loginRepo;
        private readonly ILogger logger;


        public ReportController(IServiceProvider provider)
        {
            personRepo = provider.GetService<IGenericRepository<Person>>();
            driveReportRepo = provider.GetService<IGenericRepository<DriveReport>>();
            rateRepo = provider.GetService<IGenericRepository<Rate>>();
            authRepo = provider.GetService<IGenericRepository<UserAuth>>();
            logger = provider.GetService<ILogger<AuthUserInfoController>>();
            loginRepo = provider.GetService<IGenericRepository<AppLogin>>();
        }

        [Route("appapi/report")]
        public IActionResult Report([FromBody] DriveViewModel driveViewModel)
        {
            try
            {
                var appLogin = loginRepo.AsQueryable().Where(l => l.GuId == driveViewModel.Authorization.GuId).SingleOrDefault();
                if (appLogin == null)
                {
                    var message = $"No app login found for guid";
                    logger.LogWarning(message);
                    return ErrorResult(message, ErrorCodes.InvalidAuthorization, HttpStatusCode.Unauthorized);
                }
                
                var duplicateReportCheck = driveReportRepo.AsQueryable().Where(t => t.AppUuid == driveViewModel.DriveReport.Uuid).Any();

                //if (auth == null)
                //{
                //    logger.LogDebug($"{GetType().Name}, Post(), Invalid authorization for guid: {encryptedGuId}");
                //    return new CustomErrorActionResult(Request, "Invalid authorization", ErrorCodes.InvalidAuthorization,HttpStatusCode.Unauthorized);
                //}
                //if (auth.ProfileId != driveViewModel.DriveReport.ProfileId)
                //{
                //    logger.LogDebug($"{GetType().Name}, Post(), User and drive report user do not match for profileId: {auth.ProfileId}");
                //    return new CustomErrorActionResult(Request, "User and drive report user do not match", ErrorCodes.ReportAndUserDoNotMatch,
                //         HttpStatusCode.Unauthorized);
                //}
                //if (duplicateReportCheck)
                //{
                //    logger.LogDebug($"{GetType().Name}, Post(), Report rejected, duplicate found. Drivereport uuid: {driveViewModel.DriveReport.Uuid}, profileId: {auth.ProfileId}");
                //    return new CustomErrorActionResult(Request, "Report rejected, duplicate found", ErrorCodes.DuplicateReportFound, HttpStatusCode.OK);
                //}

                //driveViewModel.DriveReport = Encryptor.EncryptDriveReport(driveViewModel.DriveReport);

                //var model = AutoMapper.Mapper.Map<DriveReport>(driveViewModel.DriveReport);

                //try
                //{
                //    Auditlog(auth.UserName, System.Reflection.MethodBase.GetCurrentMethod().Name, driveViewModel.DriveReport);
                //}
                //catch (Exception e)
                //{
                //    _logger.Error($"{GetType().Name}, Post(), Auditlog failed", e);
                //    return InternalServerError();
                //}

                //DriveReportRepo.Insert(model);
                //try
                //{
                //    Uow.Save();
                //}
                //catch (DbUpdateException dbue)
                //{
                //    var innertype = dbue.InnerException?.InnerException.GetType();
                //    if (dbue.InnerException?.InnerException is MySqlException)
                //    {
                //        MySqlException sqle = (MySqlException)dbue.InnerException?.InnerException;
                //        if (sqle.Number == 1062)
                //        {
                //            // Unique constraint on uuid has been violated, so the drivereport should not be saved. This handles an error where the app would send two duplicate reports in a row.
                //            _logger.Error($"{GetType().Name}, Post(), Duplicate report", dbue);
                //            return new CustomErrorActionResult(Request, "Report rejected, duplicate found", ErrorCodes.DuplicateReportFound, HttpStatusCode.OK);
                //        }

                //        _logger.Error($"{GetType().Name}, Post(), Save new drivereport failed", dbue);
                //        return InternalServerError();
                //    }

                //    _logger.Error($"{GetType().Name}, Post(), Save new drivereport failed", dbue);
                //    return InternalServerError();
                //}
                //catch (Exception e)
                //{
                //    _logger.Error($"{GetType().Name}, Post(), Save new drivereport failed", e);
                //    return InternalServerError();
                //}

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,$"Could not save drivereport, uuid: {driveViewModel.DriveReport.Uuid}");
                return ErrorResult("Could not save drivereport", ErrorCodes.SaveError, HttpStatusCode.BadRequest);
            }
        }
    }
}
