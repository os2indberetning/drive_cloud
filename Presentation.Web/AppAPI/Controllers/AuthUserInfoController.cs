using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presentation.Web.AppAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Presentation.Web.AppAPI.Controllers
{
    [ApiController]
    public class AuthUserInfoController : ControllerBase
    {
        private readonly IGenericRepository<Rate> rateRepo;
        private readonly IGenericRepository<UserAuth> authRepo;
        private readonly IGenericRepository<Person> personRepo;
        private readonly IGenericRepository<DriveReport> driveReportRepo;
        private readonly IGenericRepository<AppLogin> loginRepo;
        private readonly ILogger logger;


        public AuthUserInfoController(IServiceProvider provider)
        {
            personRepo = provider.GetService<IGenericRepository<Person>>();
            driveReportRepo = provider.GetService<IGenericRepository<DriveReport>>();
            rateRepo = provider.GetService<IGenericRepository<Rate>>();
            authRepo = provider.GetService<IGenericRepository<UserAuth>>();
            logger = provider.GetService<ILogger<AuthUserInfoController>>();
            loginRepo = provider.GetService<IGenericRepository<AppLogin>>();
        }

        [Route("appapi/auth")]
        public IActionResult Auth([FromBody] AuthRequestViewModel authRequest)
        {
            try
            {
                var appLogin = loginRepo.AsQueryable().Where(l => l.UserName == authRequest.UserName).SingleOrDefault();
                if (appLogin == null)
                {
                    var message = $"No app login found for username {authRequest.UserName}";
                    logger.LogWarning(message);
                    return NotFound(message);
                }

                if (appLogin.Password != GetHash(appLogin.Salt, authRequest.Password))
                {
                    var message = $"Invalid password for username {authRequest.UserName}";
                    logger.LogWarning(message);
                    return Unauthorized(message);
                }
                var userInfo = GenerateUserInfo(appLogin);

                return Ok(userInfo);

            }
            catch (Exception e)
            {
                logger.LogError(e, $"{GetType().Name}, Post(), Post method failed");
                throw;
            }
        }

        [Route("appapi/userinfo")]
        public IActionResult UserInfo([FromBody] AuthorizationViewModel authorization)
        {
            try
            {
                var appLogin = loginRepo.AsQueryable().Where(l => l.GuId == authorization.GuId).SingleOrDefault();
                if (appLogin == null)
                {
                    var message = $"No app login found for guid";
                    logger.LogWarning(message);
                    return Unauthorized(message);
                }
                var userInfo = GenerateUserInfo(appLogin);
                return Ok(userInfo);

            }
            catch (Exception e)
            {
                logger.LogError(e, $"{GetType().Name}, Post(), Post method failed");
                throw;
            }
        }

        private UserInfoViewModel GenerateUserInfo(AppLogin appLogin)
        {
            var person = appLogin.Person;
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var profile = new ProfileViewModel();
            profile.Id = person.Id;
            profile.Firstname = person.FirstName;
            profile.Lastname = person.LastName;
            profile.Authorization = new AuthorizationViewModel() { GuId = appLogin.GuId };
            var homeAddress = person.PersonalAddresses.AsQueryable().Where(a => a.GetType() == typeof(PersonalAddress)).SingleOrDefault();
            if (homeAddress != null)
            {
                profile.HomeLatitude = homeAddress.Latitude;
                profile.HomeLongitude = homeAddress.Longitude;
            }
            profile.Employments = new List<EmploymentViewModel>();
            foreach (var employment in person.Employments.Where(x => x.StartDateTimestamp < currentTimestamp && (x.EndDateTimestamp > currentTimestamp || x.EndDateTimestamp == 0)))
            {
                profile.Employments.Add(new EmploymentViewModel()
                {
                    EmploymentPosition = employment.Position,
                    EndDateTimestamp = employment.EndDateTimestamp,
                    Id = employment.Id,
                    ManNr = employment.EmploymentId,
                    StartDateTimestamp = employment.StartDateTimestamp,
                    OrgUnit = new OrgUnitViewModel()
                    {
                        OrgId = employment.OrgUnitId,
                        FourKmRuleAllowed = employment.OrgUnit.HasAccessToFourKmRule
                    }

                });
            }

            var ui = new UserInfoViewModel();
            ui.profile = profile;
            ui.rates = new List<RateViewModel>();
            foreach (var rate in rateRepo.AsQueryable().Where(x => x.Year == DateTime.Now.Year && x.Active).ToList())
            {
                ui.rates.Add(new RateViewModel()
                {
                    Id = rate.Id,
                    Description = rate.Type.Description,
                    Year = rate.Year.ToString()
                });
            }
            return ui;
        }

        private static string GetHash(string salt, string password)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(salt + password));

                foreach (var b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
