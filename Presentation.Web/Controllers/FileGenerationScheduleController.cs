using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace OS2Indberetning.Controllers
{
    public class FileGenerationScheduleController : BaseController<FileGenerationSchedule>
    {
        public FileGenerationScheduleController(IServiceProvider provider) : base(provider) { }

        //GET: odata/FileGenerationSchedules
        /// <summary>
        /// GET API for FileGenerations
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>FileGenerationSchedules</returns>
        [EnableQuery]
        public IQueryable<FileGenerationSchedule> Get(ODataQueryOptions<FileGenerationSchedule> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/FileGenerationSchedule(5)
        /// <summary>
        /// GET API endpoint for a single FileGenerationSchedule
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns>A single FileGenereationSchedule</returns>
        public IQueryable<FileGenerationSchedule> Get([FromODataUri] int key, ODataQueryOptions<FileGenerationSchedule> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/FileGenerationSchedules(5)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<FileGenerationSchedule> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/FileGenerationSchedules
        /// <summary>
        /// POST API endpoint for FileGenerationSchedules
        /// Returns forbidden if the current user is not an admin
        /// </summary>
        /// <param name="FileGenerationSchedule">FileGenerationSchedule to be posted</param>
        /// <returns></returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] FileGenerationSchedule FileGenerationSchedule)
        {
            return CurrentUser.IsAdmin ? base.Post(FileGenerationSchedule) : StatusCode(StatusCodes.Status403Forbidden);
        }

        //PATCH: odata/FileGenerationSchedules(5)
        /// <summary>
        /// PATCH API endpoint for FileGenerationSchedule
        /// </summary>
        /// <param name="key">Patches the FileGenerationSchedule identified by key</param>
        /// <param name="delta"></param>
        /// <returns>Returns forbidden if the current user is not an admin</returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<FileGenerationSchedule> delta)
        {
            return CurrentUser.IsAdmin ? base.Patch(key, delta) : StatusCode(StatusCodes.Status403Forbidden);
        }

        //DELETE: odata/FileGenerationShcedule(5)
        /// <summary>
        /// DELETE API endpoint for FileGenerationSchedule.
        /// Returns forbidden if the current user is not an admin.
        /// </summary>
        /// <param name="key">Deletes FileGenerationSchedule identified by the key</param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.IsAdmin ? base.Delete(key) : StatusCode(StatusCodes.Status403Forbidden);
        }

    }
}