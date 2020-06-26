using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OS2Indberetning.Controllers
{
    public class PersonalRoutesController : BaseController<PersonalRoute>
    {
        private readonly IPersonalRouteService _routeService;
        //GET: odata/PersonalRoutes
        public PersonalRoutesController(IServiceProvider provider) : base(provider)
        {
            _routeService = provider.GetService<IPersonalRouteService>();
        }

        /// <summary>
        /// GET API endpoint for Personal Routes.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Personal Routes</returns>
        [EnableQuery]
        public IQueryable<PersonalRoute> Get(ODataQueryOptions<PersonalRoute> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/PersonalRoutes(5)
        /// <summary>
        /// GET API endpoint for a single Personal Route.
        /// </summary>
        /// <param name="key">Returns the Route identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>A single Personal Route</returns>
        public IQueryable<PersonalRoute> Get([FromODataUri] int key, ODataQueryOptions<PersonalRoute> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/PersonalRoutes(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<PersonalRoute> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/PersonalRoutes
        /// <summary>
        /// POST API endpoint for Personal Routes.
        /// Returns forbidden if the user associated with the Personal Route is not the current user.
        /// Returns
        /// </summary>
        /// <param name="personalRoute">The route to be posted</param>
        /// <returns>The posted Route</returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] PersonalRoute personalRoute)
        {
            try
            {
                return personalRoute.PersonId.Equals(CurrentUser.Id) ? (IActionResult)Ok(_routeService.Create(personalRoute)) : StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Fail to save personal address");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //PATCH: odata/PersonalRoutes(5)
        /// <summary>
        /// PATCH API endpoint for Personal Routes.
        /// Returns forbidden if the user associated with the Personal Route is not the current user.
        /// </summary>
        /// <param name="key">Patches the Personal Route identified by key.</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<PersonalRoute> delta)
        {
            return Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId.Equals(CurrentUser.Id) ? base.Patch(key, delta) : StatusCode(StatusCodes.Status403Forbidden);
        }

        //DELETE: odata/PersonalRoutes(5)
        /// <summary>
        /// DELETE API endpoint for Personal Routes.
        /// Returns forbidden if the user associated with the Personal Route is not the current user.
        /// </summary>
        /// <param name="key">Deletes the Personal Route identified by key</param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) ? base.Delete(key) : StatusCode(StatusCodes.Status403Forbidden);
        }
    }
}
