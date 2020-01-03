using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace OS2Indberetning.Controllers
{
    [Route("api/[controller]")]
    public class HelpTextController : Controller
    {
        private IEnumerable<KeyValuePair<string, string>> _helpTextConfig;
        public HelpTextController(IConfiguration config)
        {
            _helpTextConfig = config.GetSection("HelpText").AsEnumerable().Select(c => new KeyValuePair<string, string>(c.Key.Replace("HelpText:", ""), c.Value));            
        }

        // GET api/<controller>/5
        /// <summary>
        /// API Endpoint for getting help texts to be displayed in the frontend.
        /// HelpTexts are read from CustomSettings.config
        /// </summary>
        /// <param name="id">Returns the helptext identified by id</param>
        /// <returns>Help text</returns>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Ok(_helpTextConfig.Where(c => c.Key == id).Select(c => c.Value).First());
        }

        /// <summary>
        /// Returns all help texts to cut down number of requests.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetAll()
        {            
            return Ok(_helpTextConfig);
        }
    }
}