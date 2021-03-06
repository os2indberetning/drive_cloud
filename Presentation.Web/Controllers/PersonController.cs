﻿using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OS2Indberetning.Controllers
{
    public class PersonController : BaseController<Person>
    {
        private IPersonService _person;
        private readonly IGenericRepository<Employment> _employmentRepo;
        private readonly IGenericRepository<LicensePlate> _licensePlateRepo;
        private readonly IGenericRepository<Substitute> _substituteRepo;
        private readonly IGenericRepository<AppLogin> loginRepo;

        public PersonController(IServiceProvider provider) : base(provider)
        {
            _person = provider.GetService<IPersonService>();
            _employmentRepo = provider.GetService<IGenericRepository<Employment>>();
            _licensePlateRepo = provider.GetService<IGenericRepository<LicensePlate>>();
            _substituteRepo = provider.GetService<IGenericRepository<Substitute>>();
            loginRepo = provider.GetService<IGenericRepository<AppLogin>>();
        }

        // GET: odata/Person
        /// <summary>
        /// GET API endpoint for Person
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>People</returns>
        [EnableQuery]
        public IActionResult GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            _person.ScrubCprFromPersons(res);
            var currentTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //We used to loop through all persons and remove employments that had expired
            //This proved to be to slow, and we did not have a use for current employments 
            //for a list of users, each time we needed someones employment we made queries
            //to that person anyway, so the return of this function has all employments
            //including the expired ones.
            return Ok(res);
        }


        /// <summary>
        /// GET API endpoint for CurrentUser.
        /// Sets HomeWorkDistance on each of the users employments.
        /// Strips CPR-number off.
        /// </summary>
        /// <returns>The user currently logged in.</returns>
        [EnableQuery(MaxExpansionDepth = 4)]
        // Disable caching.
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public Person GetCurrentUser()
        {
            return GetCurrentUserFromPerson(CurrentUser);
        }

        private Person GetCurrentUserFromPerson(Person person)
        {
            try
            {
                var currentUser = person;
                var currentDateTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                currentUser.Employments = currentUser.Employments.Where(x => x.EndDateTimestamp == 0 || x.EndDateTimestamp > currentDateTimestamp).ToList();
                currentUser.CprNumber = "";
                currentUser.IsSubstitute = currentUser.SubstituteFor.Count + currentUser.SubstituteLeaders.Count > 0;
                var appLogin = loginRepo.AsQueryableLazy().Where(l => l.PersonId == currentUser.Id).SingleOrDefault();
                currentUser.HasAppPassword = appLogin != null;
                currentUser.AppUserName = appLogin?.UserName;
                return currentUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name}, GetCurrentUser(), Error");
                throw ex;
            }
        }

        /// <summary>
        /// GET API endpoint for distance between CurrentUsers home address and given address.
        /// </summary>
        /// <returns>distance between CurrentUsers home address and given address</returns>
        [ResponseCache(Duration = 300, VaryByHeader = "Cookie")]
        public IActionResult GetDistanceFromHome([FromODataUri] int addressId)
        {
            var distance = _person.GetDistanceFromHome(CurrentUser,addressId);
            return Ok(distance);
        }

        /// <summary>
        /// GET API endpoint for user as CurrentUser.
        /// Sets HomeWorkDistance on each of the users employments.
        /// Strips CPR-number off.
        /// </summary>
        /// <returns>A user with with properties like CurrentUser. Is used when retrieving a user as CurrentUser when an admin tries to edit an approved report.</returns>
        [EnableQuery(MaxExpansionDepth = 4)]
        // Disable caching.
        // [ResponseCache(Duration = 300, VaryByHeader = "Cookie")]
        public Person GetUserAsCurrentUser(int id)
        {
            var person = Repo.AsQueryable().First(x => x.Id == id);
            return GetCurrentUserFromPerson(person);
        }

        //GET: odata/Person(5)
        /// <summary>
        /// GET API endpoint for a single person
        /// Strips CPR-number off.
        /// </summary>
        /// <param name="key">Returns the person identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>A single Person</returns>
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            try
            {
                var cprScrubbed = _person.ScrubCprFromPersons(GetQueryable(key, queryOptions));
                var res = cprScrubbed.ToList();
                var currentTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                foreach (var person in res.ToList())
                {
                    // Remove employments that have expired.
                    person.Employments = person.Employments.Where(x => x.EndDateTimestamp == 0 || x.EndDateTimestamp > currentTimestamp).ToList();
                }

                return res.AsQueryable();
            }
            catch (RouteInformationException e)
            {
                throw new Exception("Kunne ikke beregne rute mellem hjemme- og arbejdsadresse.", e);
            }
        }

        // PUT: odata/Person(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<Person> delta)
        {
            return base.Put(key, delta);
        }

        // POST: odata/Person
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] Person person)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        // PATCH: odata/Person(5)
        /// <summary>
        /// PATCH API endpoint for person.
        /// Returns forbidden if the person identified by key is not the current user or if the current user is not an admin.
        /// </summary>
        /// <param name="key">Patches the Person identified by key</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<Person> delta)
        {
            var person = Repo.AsQueryable().Single(x => x.Id == key);
            return CurrentUser.IsAdmin || CurrentUser.Id == person.Id ? base.Patch(key, delta) : StatusCode(StatusCodes.Status403Forbidden);
        }

        // DELETE: odata/Person(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        // GET odata/Person(5)/Employments
        /// <summary>
        /// Returns employments belonging to the user identified by key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns>Employments</returns>
        [EnableQuery]
        public IActionResult GetEmployments([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            var person = Repo.AsQueryable().FirstOrDefault(x => x.Id.Equals(key));

            if (person == null)
            {
                return BadRequest("Der findes ingen person med id " + key);
            }

            var currentTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // Remove employments that have expired.
            var res = person.Employments.Where(x => x.EndDateTimestamp == 0 || x.EndDateTimestamp > currentTimestamp);

            return Ok(res);
        }

        // GET: odata/Person(5)/Service.HasLicensePlate
        /// <summary>
        /// Returns whether or not the user identified by key has a license plate.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IActionResult HasLicensePlate([FromODataUri] int key, ODataActionParameters parameters)
        {
            return Ok(_licensePlateRepo.AsQueryable().Any(x => x.PersonId == key));
        }

        /// <summary>
        /// Gets all persons that for the logged in leader
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetEmployeesOfLeader()
        {
            List<Person> employees = new List<Person>();

            if (CurrentUser.Employments.Where(e => e.IsLeader).Any() || CurrentUser.SubstituteLeaders.Count > 0)
            {
                employees = _person.GetEmployeesOfLeader(CurrentUser);
            }
            else
            {
                return Unauthorized();
            }

            return Ok(employees);
        }
    }
}
