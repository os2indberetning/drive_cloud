using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Web.AppAPI.ViewModels;
using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Web.AppAPI.Controllers
{
    [ApiController]
    [AppAPIFilter]
    public class ReportController : ControllerBase
    {

        //private IUnitOfWork Uow { get; set; }
        //private IGenericRepository<DriveReport> DriveReportRepo { get; set; }
        //private IGenericRepository<UserAuth> AuthRepo { get; set; }
        private readonly ILogger logger;


        //public ReportController(IUnitOfWork uow, IGenericRepository<DriveReport> driveReportRepo, IGenericRepository<UserAuth> authRepo, ILogger logger) : base(logger)
        //{
        //    Uow = uow;
        //    DriveReportRepo = driveReportRepo;
        //    AuthRepo = authRepo;
        //}

        public ReportController(IServiceProvider provider)
        {
            logger = provider.GetService<ILogger<ReportController>>();
        }

        [Route("appapi/report")]
        public IActionResult Report([FromBody] DriveViewModel driveViewModel)
        {
            //var encryptedGuId = Encryptor.EncryptAuthorization(driveViewModel.Authorization).GuId;
            //var auth = AuthRepo.Get(t => t.GuId == encryptedGuId).FirstOrDefault();
            //var duplicateReportCheck = DriveReportRepo.Get(t => t.Uuid == driveViewModel.DriveReport.Uuid).Any();

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

            try
            {
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
                //logger.LogError(ex,$"{GetType().Name}, Post(), Could not save drivereport, uuid: {driveViewModel.DriveReport.Uuid}, profileId: {auth.ProfileId}");
                return new CustomErrorActionResult(Request, "Could not save drivereport", ErrorCodes.SaveError, HttpStatusCode.BadRequest);
            }
        }
    }
}
