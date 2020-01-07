using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;

namespace OS2Indberetning.Controllers
{
    public class LicensePlatesController : BaseController<LicensePlate>
    {
        private readonly ILicensePlateService _plateService;

        public LicensePlatesController(IServiceProvider provider) : base(provider)
        {
            _plateService = provider.GetService<ILicensePlateService>();
        }

        //GET: odata/LicensePlates
        /// <summary>
        /// ODATA GET API endpoint for license plates.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        public IQueryable<LicensePlate> Get(ODataQueryOptions<LicensePlate> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/LicensePlates(5)
        /// <summary>
        /// GET API endpoint for a single license plate
        /// </summary>
        /// <param name="key">Returns the licenseplate identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public IQueryable<LicensePlate> Get([FromODataUri] int key, ODataQueryOptions<LicensePlate> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/LicensePlates(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<LicensePlate> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/LicensePlates
        /// <summary>
        /// POST API endpoint for license plates.
        /// Returns forbidden if the user associated with the license plate is not the current user.
        /// </summary>
        /// <param name="LicensePlate">License plate to be posted.</param>
        /// <returns></returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] LicensePlate LicensePlate)
        {
            if (!CurrentUser.Id.Equals(LicensePlate.PersonId))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            LicensePlate = _plateService.HandlePost(LicensePlate);

            return base.Post(LicensePlate);
        }

        //PATCH: odata/LicensePlates(5)
        /// <summary>
        /// PATCH API endpoint for license plates.
        /// Returns forbidden if the user associated with the license plate is not the current user.
        /// </summary>
        /// <param name="key">Patches the license plate identified by key</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<LicensePlate> delta)
        {
            if (!CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var primary = new object();
            if (delta.TryGetPropertyValue("IsPrimary", out primary) && (bool)primary)
            {
                _plateService.MakeLicensePlatePrimary(key);
            }
            return base.Patch(key, delta);
        }

        //DELETE: odata/LicensePlates(5)
        /// <summary>
        /// DELETE API endpoint for license plates.
        /// Returns forbidden if the user associated with the license plate is not the current user.
        /// If the plate to be deleted is currently the primary license plate, a new randomly picked license plate will be made primary.
        /// </summary>
        /// <param name="key">Deletes the license plate identified by key</param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            // Get the plate to be deleted
            var plate = Repo.AsQueryable().SingleOrDefault(lp => lp.Id == key);

            if (!CurrentUser.Id.Equals(plate.PersonId))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            _plateService.HandleDelete(plate);           

            return base.Delete(key);
        }

        /// <summary>
        /// Makes the license plate identified by plateId the primary license plate.
        /// </summary>
        /// <param name="plateId"></param>
        /// <returns></returns>
        public IActionResult MakePrimary(int plateId)
        {
            if (!CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(plateId)).PersonId))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (_plateService.MakeLicensePlatePrimary(plateId))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}