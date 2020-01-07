using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Interfaces;
using Infrastructure.AddressServices.Interfaces;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;

namespace OS2Indberetning.Controllers
{


    public class AddressesController : BaseController<Address>
    {
        private readonly IGenericRepository<Employment> _employmentRepo;
        private readonly IAddressLaunderer _launderer;
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<CachedAddress> _cachedAddressRepo;
        private readonly IGenericRepository<PersonalAddress> _personalAddressRepo;
        private static Address MapStartAddress { get; set; }

        //GET: odata/Addresses
        public AddressesController(IServiceProvider provider) : base(provider)
        {
            _employmentRepo = provider.GetService<IGenericRepository<Employment>>();
            _launderer = provider.GetService<IAddressLaunderer>();
            _coordinates = provider.GetService<IAddressCoordinates>();
            _cachedAddressRepo = provider.GetService<IGenericRepository<CachedAddress>>();
            _personalAddressRepo = provider.GetService<IGenericRepository<PersonalAddress>>();
        }

        /// <summary>
        /// ODATA GET api endpoint for addresses
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Addresses</returns>
        [EnableQuery]
        public IQueryable<Address> Get(ODataQueryOptions<Address> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        /// <summary>
        /// API endpoint for getting the starting address of the map in the frontend.
        /// </summary>
        /// <returns>Starting address of frontend map</returns>
        public Address GetMapStart()
        {
            if (MapStartAddress == null)
            {
                MapStartAddress = new Address
                {
                    StreetName = _configuration["MapStart:StreetName"],
                    StreetNumber = _configuration["MapStart:StreetNumber"],
                    ZipCode = int.Parse(_configuration["MapStart:ZipCode"]),
                    Town = _configuration["MapStart:Town"],
                };

                MapStartAddress = _coordinates.GetAddressCoordinates(MapStartAddress);
            }
            return MapStartAddress;
        }


        //GET: odata/Addresses(5)
        /// <summary>
        /// ODATA GET api endpoint for a single address
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns>An address</returns>
        public IQueryable<Address> Get([FromODataUri] int key, ODataQueryOptions<Address> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/Addresses(5)
        /// <summary>
        /// Is not implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IActionResult Put([FromODataUri] int key, Delta<Address> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Addresses
        /// <summary>
        /// ODATA POST api endpoint for addresses.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns>The posted object</returns>
        [EnableQuery]
        public new IActionResult Post([FromBody] Address Address)
        {
            return base.Post(Address);
        }

        //PATCH: odata/Addresses(5)
        /// <summary>
        /// ODATA PATCH api endpoint for addresses.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IActionResult Patch([FromODataUri] int key, Delta<Address> delta)
        {
            var addr = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (addr == null)
            {
                return NotFound();
            }
            return base.Patch(key, delta);

        }

        //DELETE: odata/Addresses(5)
        /// <summary>
        /// DELETE API Endpoint. Deletes the entity identified by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IActionResult Delete([FromODataUri] int key)
        {
            var addr = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (addr == null)
            {
                return NotFound();
            }
            return base.Delete(key);
        }

        /// <summary>
        /// Returns personal and standard addresses for the user identified by personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Personal and standard addresses</returns>
        [EnableQuery]
        public IActionResult GetPersonalAndStandard(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var rep = Repo.AsQueryable();
            // Select all standard addresses.
            var addresses = rep.Where(elem => !(elem is DriveReportPoint || elem is Point || elem is CachedAddress || elem is WorkAddress || elem is PersonalAddress)).ToList();
            // Add personal addresses to addresses.
            addresses.AddRange(_personalAddressRepo.AsQueryable().Where(elem => (elem.PersonId.Equals(personId)) && elem.Type != PersonalAddressType.OldHome));


            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var employments = _employmentRepo.AsQueryable().Where(x => x.PersonId.Equals(personId)
                                                                            && x.StartDateTimestamp < currentTimestamp 
                                                                            && (x.EndDateTimestamp > currentTimestamp ||x.EndDateTimestamp == 0))
                                                                  .ToList();
            // Add the workAddress of each of the user's employments.
            foreach (var empl in employments)
            {
                var tempAddr = empl.OrgUnit.Address;
                tempAddr.Description = empl.OrgUnit.LongDescription;
                addresses.Add(tempAddr);
            }

            return Ok(addresses.AsQueryable());
        }

        /// <summary>
        /// Returns CachedAddresses for address cleaning in the admin view.
        /// A clean address is an address on which a coordinate lookup was performed successfully.
        /// By default, it will only return addresses that could not be looked up.
        /// </summary>
        /// <param name="includeCleanAddresses">if includeCleanAddresses is true it will also return the clean ones.</param>
        /// <returns>Addresses for which coordinate lookup failed.</returns>
        [EnableQuery]
        public IActionResult GetCachedAddresses(bool includeCleanAddresses = false)
        {
            if (CurrentUser.IsAdmin)
            {
                if (!includeCleanAddresses)
                {
                    var res = _cachedAddressRepo.AsQueryable().Where(x => x.IsDirty);
                    return Ok(res);
                }
                return Ok(_cachedAddressRepo.AsQueryable());
            }
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// Returns standard addresses
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IQueryable<Address> GetStandard()
        {
            var rep = Repo.AsQueryable();
            var res = rep.Where(elem => !(elem is DriveReportPoint || elem is Point)).Where(elem => !(elem is PersonalAddress || elem is WorkAddress || elem is CachedAddress));
            return res.AsQueryable();
        }

        /// <summary>
        /// Receives an address from the address cleaning view of the admin page.
        /// The address is changed and a new coordinate lookup is performed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>IActionResult</returns>
        [EnableQuery]
        public IActionResult AttemptCleanCachedAddress(Address input)
        {
            try
            {
                var cleanAddress = _launderer.Launder(input);
                cleanAddress = _coordinates.GetAddressCoordinates(cleanAddress, true);
                var cachedAddr = _cachedAddressRepo.AsQueryable().Single(x => x.Id.Equals(input.Id));
                cachedAddr.Latitude = cleanAddress.Latitude;
                cachedAddr.Longitude = cleanAddress.Longitude;
                cachedAddr.StreetName = cleanAddress.StreetName;
                cachedAddr.StreetNumber = cleanAddress.StreetNumber;
                cachedAddr.ZipCode = cleanAddress.ZipCode;
                cachedAddr.Town = cleanAddress.Town;
                cachedAddr.IsDirty = false;
                _cachedAddressRepo.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest);                
            }
        }
    }
}
