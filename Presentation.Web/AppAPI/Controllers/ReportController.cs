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
using Core.ApplicationServices.Interfaces;
using System.Collections.Generic;
using Core.DomainServices.Interfaces;
using Infrastructure.AddressServices;
using Presentation.Web.AppAPI.Filters;

namespace Presentation.Web.AppAPI.Controllers
{
    [ApiController]
    [AppAPIFilter]
    [ServiceFilter(typeof(AppAuditlogFilter))]
    public class ReportController : ControllerBase
    {
        private readonly IDriveReportService driveService;
        private readonly IGenericRepository<Rate> rateRepo;
        private readonly IGenericRepository<DriveReport> driveReportRepo;
        private readonly IGenericRepository<AppLogin> loginRepo;
        private readonly ILogger logger;
        private readonly IAddressCoordinates addressCoordinates;

        public ReportController(IServiceProvider provider)
        {
            driveService = provider.GetService<IDriveReportService>();
            rateRepo = provider.GetService<IGenericRepository<Rate>>();
            addressCoordinates = provider.GetService<IAddressCoordinates>();
            driveReportRepo = provider.GetService<IGenericRepository<DriveReport>>();
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
                    var message = $"Ugyldigt login token";
                    logger.LogWarning(message);
                    return ErrorResult(message, ErrorCodes.InvalidAuthorization, HttpStatusCode.Unauthorized);
                }

                if (appLogin.Person.Id != driveViewModel.DriveReport.ProfileId)
                {
                    var message = $"Forsøg på at indberette kørsel på forkert person";
                    logger.LogWarning(message);
                    return ErrorResult(message, ErrorCodes.ReportAndUserDoNotMatch, HttpStatusCode.Unauthorized);
                }

                var duplicateReportCheck = driveReportRepo.AsQueryable().Where(t => t.AppUuid == driveViewModel.DriveReport.Uuid).Any();
                if (duplicateReportCheck)
                {
                    var message = "Indberetning afvist da den allerede er indberettet";
                    logger.LogWarning(message);
                    return ErrorResult(message, ErrorCodes.DuplicateReportFound, HttpStatusCode.OK);
                }
                var appReport = driveViewModel.DriveReport;

                var points = new List<DriveReportPoint>();
                var viaPoints = new List<DriveReportPoint>();
                for (var i = 0; i < appReport.route.GPSCoordinates.Count; i++)
                {
                    var coordinate = appReport.route.GPSCoordinates.ToArray()[i];
                    points.Add(new DriveReportPoint
                    {
                        Latitude = coordinate.Latitude,
                        Longitude = coordinate.Longitude,
                    });

                    if (coordinate.IsViaPoint || i == 0 || i == appReport.route.GPSCoordinates.Count - 1)
                    {
                        var address = addressCoordinates.GetAddressFromCoordinates(new Address
                        {
                            Latitude = coordinate.Latitude,
                            Longitude = coordinate.Longitude
                        });

                        viaPoints.Add(new DriveReportPoint()
                        {
                            Latitude = coordinate.Latitude,
                            Longitude = coordinate.Longitude,
                            StreetName = address.StreetName,
                            StreetNumber = address.StreetNumber,
                            ZipCode = address.ZipCode,
                            Town = address.Town,
                        });
                    }
                }

                var rate = rateRepo.AsQueryable().Where(r => r.Id == appReport.RateId).First();
                var licensePlate = appLogin.Person.LicensePlates.Where(p => p.IsPrimary).FirstOrDefault();

                DriveReport newReport = new DriveReport();
                newReport.AppUuid = appReport.Uuid;
                newReport.FourKmRule = appReport.FourKmRule;
                newReport.IsFromApp = true;
                newReport.HomeToBorderDistance = appReport.HomeToBorderDistance;
                newReport.StartsAtHome = appReport.StartsAtHome;
                newReport.EndsAtHome = appReport.EndsAtHome;
                newReport.Purpose = appReport.Purpose;
                newReport.PersonId = appReport.ProfileId;
                newReport.EmploymentId = appReport.EmploymentId;
                newReport.KmRate = rate.KmRate;
                newReport.UserComment = appReport.ManualEntryRemark;
                newReport.Status = ReportStatus.Pending;
                newReport.LicensePlate = licensePlate != null ? licensePlate.Plate : "UKENDT";
                newReport.Comment = "";
                newReport.DriveReportPoints = viaPoints;
                newReport.Distance = appReport.route.TotalDistance;
                newReport.KilometerAllowance = appReport.route.GPSCoordinates.Count > 0 ? KilometerAllowance.Calculated : KilometerAllowance.Read;
                newReport.DriveDateTimestamp = (Int32)(Convert.ToDateTime(appReport.Date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                newReport.CreatedDateTimestamp = (Int32)(Convert.ToDateTime(appReport.Date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                newReport.TFCode = rate.Type.TFCode;
                newReport.TFCodeOptional = rate.Type.TFCodeOptional;
                newReport.FullName = appLogin.Person.FullName;
                newReport.RouteGeometry = GeoService.Encode(points);
                driveService.Create(newReport);

                return Ok();
            }
            catch (Exception ex)
            {
                var message = "Kunne ikke gemme indberetning fra app";
                logger.LogError(ex,$"{message}, uuid: {driveViewModel.DriveReport.Uuid}");
                return ErrorResult(message, ErrorCodes.SaveError, HttpStatusCode.BadRequest);
            }
        }
    }
}
