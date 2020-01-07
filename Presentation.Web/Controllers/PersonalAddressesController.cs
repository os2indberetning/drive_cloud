using Core.DomainModel;
using Core.DomainServices.Interfaces;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OS2Indberetning.Controllers
{
    public class PersonalAddressesController : BaseController<PersonalAddress>
    {
        private readonly IAddressCoordinates coordinates;
        //GET: odata/PersonalAddresses
        public PersonalAddressesController(IServiceProvider provider) : base(provider) 
        {
            coordinates = provider.GetService<IAddressCoordinates>();
        }

        /// <summary>
        /// GET API endpoint for PersonalAddresses.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Personal Addresses</returns>
        [EnableQuery]
        public IActionResult Get(ODataQueryOptions<PersonalAddress> queryOptions)
        {
            return Ok(GetQueryable(queryOptions));
        }

        //GET: odata/PersonalAddresses(5)
        /// <summary>
        /// GET API endpoint for a single Personal Address
        /// </summary>
        /// <param name="key">Returns the Personal Address identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>A single Personal Address</returns>
        public IActionResult Get([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            return Ok(GetQueryable(key, queryOptions));

        }

        //PUT: odata/PersonalAddresses(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<PersonalAddress> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/PersonalAddresses
        /// <summary>
        /// POST API endpoint for Personal Addresses.
        /// Returns forbidden if the user associated with the Personal Address is not the current user.
        /// Also performs a coordinate lookup.
        /// </summary>
        /// <param name="personalAddress">The Personal Address to be posted.</param>
        /// <returns>The posted Personal Address</returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] PersonalAddress personalAddress)
        {
            if (!CurrentUser.Id.Equals(personalAddress.PersonId))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var result = coordinates.GetAddressCoordinates(personalAddress);
            personalAddress.Latitude = result.Latitude;
            personalAddress.Longitude = result.Longitude;
            personalAddress.StreetName = result.StreetName;
            personalAddress.StreetNumber = result.StreetNumber;
            personalAddress.ZipCode = result.ZipCode;
            personalAddress.Town = result.Town;

            return base.Post(personalAddress);
        }

        //PATCH: odata/PersonalAddresses(5)
        /// <summary>
        /// PATCH API endpoint for Personal Addresses.
        /// Returns forbidden if the user associated with the Personal Address is not the current user or if the current user is not an admin.
        /// </summary>
        /// <param name="key">Patches the Personal Address identified by key</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<PersonalAddress> delta)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) || CurrentUser.IsAdmin ? base.Patch(key, delta) : StatusCode(StatusCodes.Status403Forbidden);
        }

        //DELETE: odata/PersonalAddresses(5)
        /// <summary>
        /// DELETE API endpoint for Personal Addresses.
        /// Returns forbidden if the current user is not the user associated with the Personal Address or if the current user is not an admin.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) || CurrentUser.IsAdmin ? base.Delete(key) : StatusCode(StatusCodes.Status403Forbidden);
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeHome
        /// <summary>
        /// Attempts to resolve an alternative home address for the user identified by personid.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Alternative Home Address for user identified by personId</returns>
        public IActionResult GetAlternativeHome(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            var res = Repo.AsQueryable().FirstOrDefault(x => x.PersonId == personId && x.Type == PersonalAddressType.AlternativeHome);
            return res == null ? (IActionResult) Ok() : Ok(res);
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeHome
        /// <summary>
        /// Returns the home address for the user identified by personId.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public IActionResult GetRealHome(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            var res = Repo.AsQueryable().FirstOrDefault(x => x.PersonId == personId && x.Type == PersonalAddressType.Home);
            return res == null ? (IActionResult)Ok() : Ok(res);
        }

        //GET odata/PersonalAddresses/Service.GetAlternativeHome?personId=1
        /// <summary>
        /// Returns Alternative Home Address for the user identified by personId if one exists. Otherwise the real Home Address is returned.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public IActionResult GetHome(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var addresses =
                Repo.AsQueryable()
                    .Where(
                        x =>
                            (x.Type == PersonalAddressType.Home || x.Type == PersonalAddressType.AlternativeHome)
                            && x.PersonId == personId);


            var res = new List<PersonalAddress>
            {
                addresses.Any(x => x.Type == PersonalAddressType.AlternativeHome)
                    ? addresses.First(x => x.Type == PersonalAddressType.AlternativeHome)
                    : addresses.First(x => x.Type == PersonalAddressType.Home)
            };

            return Ok(res.AsQueryable());
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeWork
        /// <summary>
        /// Returns alternative work addresses for the user identified by key.
        /// Returns forbidden if the user identified by key is not the currentuser or if the currentuser is not an admin.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public IActionResult GetAlternativeWork([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            if (!CurrentUser.Id.Equals(key) && !CurrentUser.IsAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return Ok(GetQueryable(queryOptions).Where(x => x.PersonId == key && x.Type == PersonalAddressType.AlternativeWork));
        }
    }
}
