﻿using Core.ApplicationServices;
using Core.DomainModel;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace OS2Indberetning.Controllers
{
    public class RatesController : BaseController<Rate>
    {
        readonly RatePostService _ratePostService = new RatePostService();

        public RatesController(IServiceProvider provider) : base(provider) { }

        /// <summary>
        /// GET API endpoint for Rates.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Rates</returns>
        [EnableQuery]
        public IQueryable<Rate> Get(ODataQueryOptions<Rate> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/Rates(5)
        /// <summary>
        /// GET API endpoint for a single Rate.
        /// </summary>
        /// <param name="key">Returns the rate identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public IQueryable<Rate> Get([FromODataUri] int key, ODataQueryOptions<Rate> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }


        //PUT: odata/Rates(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<Rate> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Rates
        /// <summary>
        /// POST API endpoint. 
        /// Returns forbidden if the current user is not an admin.
        /// Deactivates any existing rates in the same year with the same TF-code.
        /// </summary>
        /// <param name="Rate">The Rate to be posted.</param>
        /// <returns>The posted rate.</returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] Rate Rate)
        {
            if (!CurrentUser.IsAdmin) return StatusCode(StatusCodes.Status403Forbidden);
            _ratePostService.DeactivateExistingRate(Repo.AsQueryable(), Rate);
            return base.Post(Rate);
        }

        //PATCH: odata/Rates(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<Rate> delta)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        //DELETE: odata/Rates(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }
        
        // GET: odata/Rates/Service.ThisYearsRates
        /// <summary>
        /// Returns Rates for the current year.
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IQueryable<Rate> ThisYearsRates()
        {
            IQueryable<Rate> result = null;
            try
            {
                result = Repo.AsQueryable().Where(x => x.Active);
            }catch(Exception e)
            {
                _logger.LogError(e, $"{GetType().Name}, ThisYearsRates(), Error");
            }
            return result;
        }
    }
}
