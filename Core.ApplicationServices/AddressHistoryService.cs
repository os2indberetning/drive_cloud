using Core.DomainModel;
using Core.DomainServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ApplicationServices
{
    public class AddressHistoryService
    {
        private readonly IGenericRepository<Employment> _emplRepo;
        private readonly IGenericRepository<AddressHistory> _addressHistoryRepo;
        private readonly IGenericRepository<PersonalAddress> _personalAddressRepo;
        private List<WorkAddress> _workAddresses = new List<WorkAddress>();
        private List<PersonalAddress> _homeAddresses = new List<PersonalAddress>();
        private HashSet<int> _changedHistories = new HashSet<int>();
        private ILogger _logger;

        public AddressHistoryService(IServiceProvider provider) {
            _emplRepo = provider.GetService<IGenericRepository<Employment>>();
            _addressHistoryRepo = provider.GetService<IGenericRepository<AddressHistory>>();
            _personalAddressRepo = provider.GetService<IGenericRepository<PersonalAddress>>();
            _logger = provider.GetService<ILogger<AddressHistoryService>>();
        }


        public void CreateNonExistingHistories()
        {
            Console.WriteLine("Creating non-existing address histories.");
            try
            {
                var i = 0;
                var empls = _emplRepo.AsQueryable().ToList();
                var activeHistories = _addressHistoryRepo.AsQueryable().Where(x => x.EndTimestamp == 0).ToList();
                foreach (var employment in empls)
                {
                    i++;
                    if (i % 10 == 0)
                    {
                        Console.WriteLine("Checking employment " + i + " of " + empls.Count);
                    }
                    if (!activeHistories.AsQueryable().Any(x => x.EmploymentId == employment.Id))
                    {
                        var homeAddress =
                            _personalAddressRepo.AsQueryable()
                                .FirstOrDefault(x => x.PersonId == employment.PersonId && x.Type == PersonalAddressType.Home);

                        if (homeAddress == null || employment.OrgUnit.Address == null)
                        {
                            continue;
                        }

                        _addressHistoryRepo.Insert(new AddressHistory
                        {
                            EmploymentId = employment.Id,
                            EndTimestamp = 0,
                            WorkAddressId = employment.OrgUnit.AddressId,
                            HomeAddressId = homeAddress.Id,
                            StartTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                        });
                    }
                }
                _addressHistoryRepo.Save();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CreateNonExistingHistories()");
                throw e;
            }
        }

        public void UpdateAddressHistories()
        {
            try
            {
                var i = 0;
                var activeHistories = _addressHistoryRepo.AsQueryable().Where(x => x.EndTimestamp == 0).ToList();
                foreach (var addressHistory in activeHistories)
                {
                    if (i % 10 == 0)
                    {
                        Console.WriteLine("Checking active history " + i + " of " + activeHistories.Count);
                    }
                    var homeAddress =
                        _personalAddressRepo.AsQueryable()
                            .FirstOrDefault(
                                x => x.PersonId == addressHistory.Employment.PersonId && x.Type == PersonalAddressType.Home);
                    if (homeAddress != addressHistory.HomeAddress ||
                        addressHistory.WorkAddress != addressHistory.Employment.OrgUnit.Address)
                    {
                        // One or two addresses have changed. End the history;
                        addressHistory.EndTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    }
                    if (i % 1000 == 0)
                    {
                        _addressHistoryRepo.Save();
                    }
                    i++;
                }
                _addressHistoryRepo.Save();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "UpdateAddressHistories()");
                throw e;
            }
        }
    }
}
