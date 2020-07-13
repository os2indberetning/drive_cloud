using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Interfaces;
using Infrastructure.AddressServices.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Core.ApplicationServices
{
    public class APIService
    {
        private static Boolean isUpdating = false;
        private readonly IGenericRepository<OrgUnit> _orgUnitRepo;
        private readonly IGenericRepository<CachedAddress> _cachedRepo;
        private readonly IAddressLaunderer _actualLaunderer;
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly ISubstituteService _subService;
        private readonly IGenericRepository<Substitute> _subRepo;
        private readonly IGenericRepository<DriveReport> _reportRepo;

        private readonly IDriveReportService _driveService;
        private readonly ILogger<APIService> _logger;
        private readonly CachedAddressLaunderer _launderer;
        private readonly AddressHistoryService _addressHistoryService;
        private readonly IConfiguration _configuration;


        public APIService(IServiceProvider provider)
        {
            _orgUnitRepo = provider.GetService<IGenericRepository<OrgUnit>>();
            _cachedRepo = provider.GetService<IGenericRepository<CachedAddress>>();
            _actualLaunderer = provider.GetService<IAddressLaunderer>();
            _coordinates = provider.GetService<IAddressCoordinates>(); ;
            _personRepo = provider.GetService<IGenericRepository<Person>>(); ;
            _subService = provider.GetService<ISubstituteService>(); ;
            _subRepo = provider.GetService<IGenericRepository<Substitute>>(); ;
            _reportRepo = provider.GetService<IGenericRepository<DriveReport>>(); ;
            _driveService = provider.GetService<IDriveReportService>(); ;
            _logger = provider.GetService<ILogger<APIService>>(); ;
            _launderer = new CachedAddressLaunderer(_cachedRepo, _actualLaunderer, _coordinates);
            _addressHistoryService = provider.GetService<AddressHistoryService>();
            _configuration = provider.GetService<IConfiguration>();

            // manually handle changes on these large datasets to improve performance
            _orgUnitRepo.SetChangeTrackingEnabled(false);
            _personRepo.SetChangeTrackingEnabled(false);
            _subRepo.SetChangeTrackingEnabled(false);
            _reportRepo.SetChangeTrackingEnabled(false);
        }

        public IEnumerable<APIReportDTO> GetReportsToPayroll()
        {
            var result = new List<APIReportDTO>();
            foreach (var reportToPayroll in _reportRepo.AsQueryable().Where(r => r.Status == ReportStatus.APIReady))
            {
                var isAdministrativeCostCenter = !String.IsNullOrEmpty(_configuration["AdministrativeCostCenterPrefix"]) && reportToPayroll.Employment.CostCenter.ToString().StartsWith(_configuration["AdministrativeCostCenterPrefix"]);
                result.Add(new APIReportDTO()
                {
                    Id = reportToPayroll.Id,
                    AccountNumber = isAdministrativeCostCenter ? _configuration["AdministrativeAccount"] : reportToPayroll.AccountNumber,
                    CprNumber = reportToPayroll.Person.CprNumber,
                    DriveDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(reportToPayroll.DriveDateTimestamp).ToLocalTime(),
                    EmploymentId = reportToPayroll.Employment.EmploymentId,
                    EmploymentType = reportToPayroll.Employment.EmploymentType,
                    ExtraNumber = reportToPayroll.Employment.ExtraNumber,
                    TFCode = reportToPayroll.TFCode,
                    TFCodeOptional = reportToPayroll.TFCodeOptional,
                    DistanceMeters = Convert.ToInt32(reportToPayroll.Distance * 1000),
                    InstituteCode = reportToPayroll.Employment.InstituteCode,
                    ApprovedByCpr = reportToPayroll.ApprovedBy.CprNumber,
                    LicensePlate = reportToPayroll.LicensePlate
                });
                reportToPayroll.Status = ReportStatus.APIFetched;
                _reportRepo.Update(reportToPayroll);
            }
            _reportRepo.Save();
            return result;
        }


        public void AcknowledgeReportsProcessed(int[] reportIds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            foreach (var report in _reportRepo.AsQueryable().Where(r => reportIds.Contains(r.Id)))
            {
                report.Status = ReportStatus.Invoiced;
                var deltaTime = DateTime.Now.ToUniversalTime() - epoch;
                report.ProcessedDateTimestamp = (long)deltaTime.TotalSeconds;
                _reportRepo.Update(report);
            }
            _reportRepo.Save();
        }

        public void UpdateOrganization(APIOrganizationDTO apiOrganizationDTO)
        {
            try
            {
                if (isUpdating)
                {
                    _logger.LogError("UpdateOrganization cancelled because it is already updating");
                }
                else
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    isUpdating = true;
                    _logger.LogInformation("Updating OrgUnits");
                    UpdateOrgUnits(apiOrganizationDTO.OrgUnits);
                    _logger.LogInformation("Updating Persons");
                    UpdatePersons(apiOrganizationDTO.Persons);
                    _logger.LogInformation("Updating address histories");
                    _addressHistoryService.UpdateAddressHistories();
                    _logger.LogInformation("Creating non existing address histories");
                    _addressHistoryService.CreateNonExistingHistories();
                    _logger.LogInformation("Updating leaders on expired or activated substitutes");
                    UpdateLeadersOnExpiredOrActivatedSubstitutes();
                    _logger.LogInformation("Adding leaders to reports that have none");
                    AddLeadersToReportsThatHaveNone();
                    _logger.LogInformation("Update completed in {0} seconds", stopwatch.Elapsed.TotalSeconds);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update Organization");
            }
            finally
            { 
                isUpdating = false;
            }
        }

        private void UpdateOrgUnits(IEnumerable<APIOrgUnit> apiOrgUnits)
        {
            // Handle inserts
            var toBeInserted = apiOrgUnits.Where(s => !_orgUnitRepo.AsNoTracking().Select(d => d.OrgId.ToString()).Contains(s.Id)).ToList();
            var toBeUpdated = _orgUnitRepo.AsQueryableLazy().Where(d => apiOrgUnits.Select(s => s.Id).Contains(d.OrgId.ToString())).ToList();

            var insertTotal = toBeInserted.Count();
            var insertCounter = 0;
            _logger.LogDebug("Orgunits to be inserted: {0}", insertTotal);
            foreach (var apiOrgUnit in toBeInserted)
            {
                if (++insertCounter % 10 == 0)
                {
                    _logger.LogDebug("Inserting orgunit {0} of {1}", insertCounter, insertTotal);
                }
                var orgToInsert = new OrgUnit();
                orgToInsert.HasAccessToFourKmRule = false;
                orgToInsert.DefaultKilometerAllowance = KilometerAllowance.Calculated;
                mapAPIOrgUnit(apiOrgUnit,ref orgToInsert);
                if (orgToInsert.Address == null)
                {
                    _logger.LogWarning("Skipping orgunit insert because it has no address. OrgId: {0} Name: {1}", orgToInsert.OrgId, orgToInsert.LongDescription);
                    continue;
                }
                _orgUnitRepo.Insert(orgToInsert);
                // need to save each time to generate db id used in the next parent references
                _orgUnitRepo.Save();
            }

            // Handle updates
            var updateTotal = toBeUpdated.Count();
            var updateCounter = 0;
            _logger.LogDebug("Orgunits to be updated: {0}", updateTotal);
            foreach (var orgUnit in toBeUpdated)
            {
                try
                {
                    if (++updateCounter % 10 == 0)
                    {
                        _logger.LogDebug("Updating orgunit {0} of {1}", updateCounter, updateTotal);
                    }
                    var apiOrgUnit = apiOrgUnits.Where(s => s.Id == orgUnit.OrgId.ToString()).First();
                    var orgToUpdate = orgUnit;
                    mapAPIOrgUnit(apiOrgUnit, ref orgToUpdate);
                    if (orgToUpdate.Address == null)
                    {
                        _logger.LogWarning("Skipping orgunit update because it has no address. OrgId: {0} Name: {1}", orgToUpdate.OrgId, orgToUpdate.LongDescription);
                        continue;
                    }
                    _orgUnitRepo.Update(orgToUpdate);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"Failed to update Orgunit. OrgId: {0} Name: {1}", orgUnit.OrgId, orgUnit.LongDescription);
                }

            }
            _orgUnitRepo.DetectChanges();
            _orgUnitRepo.Save();
        }

        private void UpdatePersons(IEnumerable<APIPerson> apiPersons)
        {
            // Handle inserts
            var toBeInserted = apiPersons.Where(s => !_personRepo.AsNoTracking().Select(d => d.CprNumber).Contains(s.CPR));
            var toBeUpdated = _personRepo.AsQueryableLazy().Where(d => apiPersons.Select(s => s.CPR).Contains(d.CprNumber)).ToList();
            var toBeDeleted = _personRepo.AsQueryableLazy().Where(p => p.IsActive && !apiPersons.Select(ap => ap.CPR).Contains(p.CprNumber)).ToList();

            var insertTotal = toBeInserted.Count();
            var insertCounter = 0;
            _logger.LogDebug("Persons to be inserted: {0}", insertTotal);
            foreach (var apiPerson in toBeInserted)
            {
                if (++insertCounter % 10 == 0)
                {
                    _logger.LogDebug("Inserting person {0} of {1}", insertCounter, insertTotal);
                }
                var personToInsert = new Person();
                personToInsert.IsAdmin = false;
                personToInsert.RecieveMail = true;
                personToInsert.Employments = new List<Employment>();
                personToInsert.PersonalAddresses = new List<PersonalAddress>();
                mapAPIPerson(apiPerson, ref personToInsert);
                _personRepo.Insert(personToInsert);
                _personRepo.Save();
                UpdateHomeAddress(apiPerson, ref personToInsert);
            }
            _personRepo.Save();

            // Handle updates
            var updateTotal = toBeUpdated.Count();
            var updateCounter = 0;
            _logger.LogDebug("Persons to be updated: {0}", updateTotal);
            foreach (var person in toBeUpdated)
            {
                try
                {
                    if (++updateCounter % 10 == 0)
                    {
                        _logger.LogDebug("Updating person {0} of {1}", updateCounter, updateTotal);
                    }
                    var apiPerson = apiPersons.Where(s => s.CPR == person.CprNumber).First();
                    var personToUpdate = person;
                    mapAPIPerson(apiPerson, ref personToUpdate);
                    UpdateHomeAddress(apiPerson, ref personToUpdate);
                    _personRepo.Update(personToUpdate);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to update Person. Name: {0}", person.FullName);
                }
            }

            // Handle deletes
            var deleteTotal = toBeDeleted.Count();
            var deleteCounter = 0;
            _logger.LogDebug("Persons to be inactivated: {0}", deleteTotal);
            foreach (var personToBeDeleted in toBeDeleted)
            {
                if (++deleteCounter % 10 == 0)
                {
                    _logger.LogDebug("Inactivating person {0} of {1}", deleteCounter, deleteTotal);
                }
                personToBeDeleted.IsActive = false;
                foreach (var employment in personToBeDeleted.Employments)
                {
                    if (employment.EndDateTimestamp == 0 || employment.EndDateTimestamp > GetUnixTime(DateTime.Now.Date))
                    {
                        employment.EndDateTimestamp = GetUnixTime(DateTime.Now.Date);
                    }
                }
                _personRepo.Update(personToBeDeleted);
            }
            _personRepo.DetectChanges();
            _personRepo.Save();
        }

        private void mapAPIPerson(APIPerson apiPerson, ref Person personToInsert)
        {
            personToInsert.CprNumber = apiPerson.CPR;
            personToInsert.FirstName = apiPerson.FirstName ?? "ikke opgivet";
            personToInsert.LastName = apiPerson.LastName ?? "ikke opgivet";
            personToInsert.Initials = apiPerson.Initials ?? "";
            // only update email if supplied
            if (apiPerson.Email != null || personToInsert.Mail == null)
            {
                personToInsert.Mail = apiPerson.Email ?? "";
            }            
            foreach(var existingEmployment in personToInsert.Employments.Where(e => e.EndDateTimestamp == 0 || e.EndDateTimestamp >= GetUnixTime(DateTime.Now.Date)))
            {
                // if database active employment does not exist in source then set end date
                if (!apiPerson.Employments.Select(s => s.EmployeeNumber).Contains(existingEmployment.EmploymentId.ToString())) 
                {
                    existingEmployment.EndDateTimestamp = GetUnixTime(DateTime.Now.Date);
                }
            }
            personToInsert.IsActive = false;
            foreach (var sourceEmployment in apiPerson.Employments)
            {
                var employment = personToInsert.Employments.Where(d => d.EmploymentId.ToString() == sourceEmployment.EmployeeNumber && (d.EndDateTimestamp == 0 || d.EndDateTimestamp >= GetUnixTime(DateTime.Now.Date))).SingleOrDefault();

                if (employment == null)
                {
                    employment = new Employment();
                    personToInsert.Employments.Add(employment);
                }
                var orgUnitId = _orgUnitRepo.AsNoTracking().Where(o => o.OrgId.ToString() == sourceEmployment.OrgUnitId).Select(o => o.Id).First();
                employment.OrgUnitId = orgUnitId;
                employment.Position = sourceEmployment.Position ?? "Ingen stillingsbetegnelse";
                employment.IsLeader = sourceEmployment.Manager;
                employment.StartDateTimestamp = GetUnixTime( sourceEmployment.FromDate ?? DateTime.Now.Date );
                employment.ExtraNumber = sourceEmployment.ExtraNumber ?? 0;
                employment.EmploymentType = sourceEmployment.EmploymentType ?? 0;
                employment.CostCenter = sourceEmployment.CostCenter == null ? 0 : long.Parse(sourceEmployment.CostCenter);
                employment.EmploymentId = sourceEmployment.EmployeeNumber;
                employment.EndDateTimestamp = sourceEmployment.ToDate == null ? 0 : GetUnixTime( sourceEmployment.ToDate.Value.AddDays(1) );
                employment.InstituteCode = sourceEmployment.InstituteCode;
                // only set person active to true if person has a current or future employment
                if (sourceEmployment.ToDate == null || sourceEmployment.ToDate >= DateTime.Now.Date)
                {
                    personToInsert.IsActive = true;
                }
            }
        }

        private OrgUnit mapAPIOrgUnit(APIOrgUnit apiOrgUnit, ref OrgUnit orgUnit)
        {            
            orgUnit.OrgId = apiOrgUnit.Id;
            if (!String.IsNullOrEmpty(apiOrgUnit.ParentId))
            {
                orgUnit.ParentId = _orgUnitRepo.AsNoTracking().Where(o => o.OrgId.ToString() == apiOrgUnit.ParentId).Select(o => o.Id).First();
            }
            orgUnit.ShortDescription = apiOrgUnit.Name;
            orgUnit.LongDescription = apiOrgUnit.Name;
            var workAddress = GetWorkAddress(apiOrgUnit);
            if (workAddress != orgUnit.Address)
            {
                orgUnit.Address = workAddress;
            }
            return orgUnit;
        }

        private long GetUnixTime(DateTime dateTime)
        {
            return (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1).Date)).TotalSeconds;
        }


        /// <summary>
        /// Gets work address from API Address.
        /// </summary>
        /// <param name="org"></param>
        /// <returns>WorkAddress</returns>
        private WorkAddress GetWorkAddress(APIOrgUnit apiOrgunit)
        {
            var launderer = new CachedAddressLaunderer(_cachedRepo, _actualLaunderer, _coordinates);

            if (apiOrgunit.Address?.Street == null)
            {
                return null;
            }

            var splitStreetAddress = SplitAddressOnNumber(apiOrgunit.Address.Street);

            var addressToLaunder = new Address
            {
                StreetName = splitStreetAddress.ElementAt(0),
                StreetNumber = splitStreetAddress.Count > 1 ? splitStreetAddress.ElementAt(1) : "1",
                ZipCode = apiOrgunit.Address.PostalCode,
                Town = apiOrgunit.Address.City,
                Description = apiOrgunit.Name
            };

            addressToLaunder = launderer.Launder(addressToLaunder);

            var launderedAddress = new WorkAddress()
            {
                StreetName = addressToLaunder.StreetName,
                StreetNumber = addressToLaunder.StreetNumber,
                ZipCode = addressToLaunder.ZipCode,
                Town = addressToLaunder.Town,
                Latitude = addressToLaunder.Latitude ?? "",
                Longitude = addressToLaunder.Longitude ?? "",
                Description = apiOrgunit.Name
            };

            var existingOrg = _orgUnitRepo.AsQueryable().FirstOrDefault(x => x.OrgId.Equals(apiOrgunit.Id));

            // If the address hasn't changed then set the Id to be the same as the existing one.
            // That way a new address won't be created in the database.
            // If the address is not the same as the existing one,
            // Then the Id will be 0, and a new address will be created in the database.
            if (existingOrg != null
                && existingOrg.Address != null
                && existingOrg.Address.StreetName == launderedAddress.StreetName
                && existingOrg.Address.StreetNumber == launderedAddress.StreetNumber
                && existingOrg.Address.ZipCode == launderedAddress.ZipCode
                && existingOrg.Address.Town == launderedAddress.Town
                && existingOrg.Address.Latitude == launderedAddress.Latitude
                && existingOrg.Address.Longitude == launderedAddress.Longitude
                && existingOrg.Address.Description == launderedAddress.Description)
            {
                launderedAddress.Id = existingOrg.AddressId;
            }
            return launderedAddress;
        }

        /// <summary>
        /// Splits an address represented as "StreetName StreetNumber" into a list of "StreetName" , "StreetNumber"
        /// </summary>
        /// <param name="address">String to split</param>
        /// <returns>List of StreetName and StreetNumber</returns>
        private List<String> SplitAddressOnNumber(string address)
        {
            var result = new List<string>();
            var index = address.IndexOfAny("0123456789".ToCharArray());
            if (index <= 0)
            {
                result.Add(address);
            }
            else
            {
                result.Add(address.Substring(0, index - 1));
                result.Add(address.Substring(index, address.Length - index));
            }
            return result;
        }

        /// <summary>
        /// Updates ResponsibleLeader on all reports that had a substitute which expired yesterday or became active today.
        /// </summary>
        public void UpdateLeadersOnExpiredOrActivatedSubstitutes()
        {
            // TODO Find something more generic for updating drive and vacation reports.
            var yesterdayTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds;
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var endOfDayStamp = _subService.GetEndOfDayTimestamp(yesterdayTimestamp);
            var startOfDayStamp = _subService.GetStartOfDayTimestamp(currentTimestamp);

            var affectedSubstitutes = _subRepo.AsNoTracking().Where(s => (s.EndDateTimestamp == endOfDayStamp) || (s.StartDateTimestamp == startOfDayStamp)).ToList();
            Console.WriteLine(affectedSubstitutes.Count() + " substitutes have expired or become active. Updating affected reports.");
            foreach (var sub in affectedSubstitutes)
            {
                _subService.UpdateReportsAffectedBySubstitute(sub);
            }
        }

        public void AddLeadersToReportsThatHaveNone()
        {
            // Fail-safe as some reports for unknown reasons have not had a leader attached
            Console.WriteLine("Adding leaders to drive reports that have none");
            var reports = _reportRepo.AsQueryableLazy().Where(r => (r.PersonReports.Count == 0 || r.ActualLeader == null) && r.Status == ReportStatus.Pending ).ToList();
            foreach (var report in reports)
            {
                report.UpdateResponsibleLeaders(_driveService.GetResponsibleLeadersForReport(report));
                var actualLeader = _driveService.GetActualLeaderForReport(report);
                if (actualLeader != null)
                {
                    report.ActualLeaderId = actualLeader.Id;
                }                    
            }
            _reportRepo.DetectChanges();
            _reportRepo.Save();
        }

        public void UpdateHomeAddress(APIPerson apiPerson, ref Person person)
        {
            if (apiPerson.Address == null)
            {
                _logger.LogWarning($"No home address supplied for {apiPerson.FirstName} {apiPerson.LastName}");
                return;
            }
            var splitStreetAddress = SplitAddressOnNumber(apiPerson.Address.Street);
            var apiAddress = new Address
            {
                Description = person.FullName,
                StreetName = splitStreetAddress.ElementAt(0),
                StreetNumber = splitStreetAddress.Count > 1 ? splitStreetAddress.ElementAt(1) : "1",
                ZipCode = apiPerson.Address.PostalCode,
                Town = apiPerson.Address.City ?? "",
            };
            apiAddress = _launderer.Launder(apiAddress);
            var apiPersonalAddress = new PersonalAddress()
            {
                PersonId = person.Id,
                Type = PersonalAddressType.Home,
                StreetName = apiAddress.StreetName,
                StreetNumber = apiAddress.StreetNumber,
                ZipCode = apiAddress.ZipCode,
                Town = apiAddress.Town,
                Latitude = apiAddress.Latitude ?? "",
                Longitude = apiAddress.Longitude ?? "",
                Description = apiAddress.Description
            };
            var homeAddress = person.PersonalAddresses.Where(a => a.Type == PersonalAddressType.Home).FirstOrDefault();
            if (homeAddress == null)
            {
                person.PersonalAddresses.Add(apiPersonalAddress);
            }
            else if (homeAddress != apiPersonalAddress)
            {
                homeAddress.Type = PersonalAddressType.OldHome;
                person.PersonalAddresses.Add(apiPersonalAddress);
            }
        }

    }


}
