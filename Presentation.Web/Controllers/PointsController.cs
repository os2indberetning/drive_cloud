using Core.DomainModel;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Point>("Points");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PointsController : BaseController<Point>
    {
        public PointsController(IServiceProvider provider) : base(provider) { }

        //GET: odata/Points
        /// <summary>
        /// GET API endpoint for Points
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        public IQueryable<Point> Get(ODataQueryOptions<Point> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/Points(5)
        /// <summary>
        /// GET API endpoint for a single point.
        /// </summary>
        /// <param name="key">Returns the point identified by key.</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public IQueryable<Point> Get([FromODataUri] int key, ODataQueryOptions<Point> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/Points(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<Point> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Points
        /// <summary>
        /// POST API endpoint for points.
        /// </summary>
        /// <param name="Point">The point to be posted.</param>
        /// <returns>The posted point.</returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] Point Point)
        {
            return base.Post(Point);
        }

        //PATCH: odata/Points(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<Point> delta)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        //DELETE: odata/Points(5)
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
