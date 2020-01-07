using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OS2Indberetning.Controllers
{
    public class OrgUnitsController : BaseController<OrgUnit>
    {
        private readonly IOrgUnitService _orgService;
        private IPersonService _person;


        public OrgUnitsController(IServiceProvider provider) : base(provider)
        {
            _orgService = provider.GetService<IOrgUnitService>();
            _person = provider.GetService<IPersonService>();
        }

        //GET: odata/OrgUnits
        /// <summary>
        /// GET API endpoint for OrgUnits
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>OrgUnits</returns>
        [EnableQuery]
        public IQueryable<OrgUnit> Get(ODataQueryOptions<OrgUnit> queryOptions)
        {
            var res =  GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/OrgUnits(5)
        /// <summary>
        /// GET API endpoint for a single OrgUnit
        /// </summary>
        /// <param name="key">Returns the OrgUnit identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>A single OrgUnit</returns>
        public IQueryable<OrgUnit> Get([FromODataUri] int key, ODataQueryOptions<OrgUnit> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //GET: odata/OrgUnits
        /// <summary>
        /// Returns OrgUnits for which the user identified by personId is responsible for approving.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>OrgUnits</returns>
        [EnableQuery]
        public IActionResult GetWhereUserIsResponsible(int personId)
        {
           return Ok(_orgService.GetWhereUserIsResponsible(personId));
        }

        /// <summary>
        /// Returns the leader of the orgunit specified by orgId
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>OrgUnits</returns>
        [EnableQuery]
        public IActionResult GetLeaderOfOrg(int orgId)
        {
            return Ok(_orgService.GetLeaderOfOrg(orgId));
        }

        //PUT: odata/OrgUnits(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<OrgUnit> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/OrgUnits
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="orgUnit"></param>
        /// <returns></returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] OrgUnit orgUnit)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        //PATCH: odata/OrgUnits(5)
        /// <summary>
        /// PATCH API endpoint for OrgUnits. Returns NotAllowed if the current user is not an admin.
        /// </summary>
        /// <param name="key">Patches the OrgUnit identified by key</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<OrgUnit> delta)
        {
            return CurrentUser.IsAdmin ? base.Patch(key, delta) : StatusCode(StatusCodes.Status405MethodNotAllowed);
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

        /// <summary>
        /// Gets all orgUnits that for the leader
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetOrgUnitsForLeader()
        {
            List<OrgUnit> orgUnits = new List<OrgUnit>();
            if (CurrentUser.Employments.Where(e => e.IsLeader).Any() || CurrentUser.SubstituteLeaders.Count > 0)
            {
                orgUnits = _person.GetOrgUnitsForLeader(CurrentUser);
            }
            else
            {
                return Unauthorized();
            }
            return Ok(orgUnits);
        }
    }
}