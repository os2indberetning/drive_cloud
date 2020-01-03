using Core.ApplicationServices;
using Core.DomainModel;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OS2Indberetning.Controllers
{
    public class AppLoginController : BaseController<AppLogin>
    {

        private IAppLoginService _loginService;

        public AppLoginController(IServiceProvider provider) : base(provider)
        {
            _loginService = provider.GetService<IAppLoginService>();
        }

        [EnableQuery]
        public IActionResult Get(ODataQueryOptions<AppLogin> queryOptions)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);            
        }

        public IActionResult Get([FromODataUri] int key, ODataQueryOptions<AppLogin> queryOptions)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        public new IActionResult Put([FromODataUri] int key, Delta<AppLogin> delta)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        [EnableQuery]
        public new IActionResult Post(AppLogin AppLogin)
        {
            var prepared = _loginService.PrepareAppLogin(AppLogin);
            Repo.Insert(prepared);
            Repo.Save();
            return Ok();
        }

        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<AppLogin> delta)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        
        public new IActionResult Delete([FromODataUri] int key)
        {
            var toDelete = Repo.AsQueryable().Where(x => x.PersonId == key).ToList();
            Repo.DeleteRange(toDelete);
            Repo.Save();
            return Ok();
        }
    }
}