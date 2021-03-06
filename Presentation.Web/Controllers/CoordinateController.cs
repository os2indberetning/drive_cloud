﻿using Core.DomainModel;
using Core.DomainServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OS2Indberetning.Filters;
using System.Collections.Generic;
using System.Linq;

namespace OS2Indberetning.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    [ServiceFilter(typeof(AuditlogFilter))]
    public class CoordinateController : Controller
    {
        private readonly IAddressCoordinates _coordinates;
        private readonly ILogger _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinates"></param>
        public CoordinateController(IAddressCoordinates coordinates, ILogger<CoordinateController> logger)
        {
            _coordinates = coordinates;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public IActionResult SetCoordinatesOnAddressList([FromBody] IEnumerable<Address> addresses)
        {
            try
            {
                var result = addresses.Select(address => _coordinates.GetAddressCoordinates(address, true)).ToList();
                return Ok(result);
            }
            catch (System.Exception e)
            {
                _logger.LogWarning(e, $"Could not SetCoordinatesOnAddressList {JsonConvert.SerializeObject(addresses)}");
                return BadRequest();
            }
        }


    }
}