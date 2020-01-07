using Core.DomainModel;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace OS2Indberetning.Controllers
{
    public class RateTypesController : BaseController<RateType>
    {
        public RateTypesController(IServiceProvider provider) : base(provider) { }

        //GET: odata/RateTypes
        /// <summary>
        /// GET API endpoint for RateTypes.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>RateTypes</returns>
        [EnableQuery]
        public IQueryable<RateType> Get(ODataQueryOptions<RateType> queryOptions)
        {
            var res =  GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/RateTypes(5)
        /// <summary>
        /// GET API endpoint for a single RateType
        /// </summary>
        /// <param name="key">Returns the RateType identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>A single RateType</returns>
        public IQueryable<RateType> Get([FromODataUri] int key, ODataQueryOptions<RateType> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/RateTypes(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<RateType> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/RateTypes
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="RateType"></param>
        /// <returns></returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] RateType RateType)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        //PATCH: odata/RateTypes(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<RateType> delta)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        //DELETE: odata/OrgUnits(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }
    }
}