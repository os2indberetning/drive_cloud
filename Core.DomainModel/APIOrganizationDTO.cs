using System.Collections.Generic;

namespace Core.DomainModel
{
    public class APIOrganizationDTO
    {
        public IEnumerable<APIOrgUnit> OrgUnits { get; set; }
        public IEnumerable<APIPerson> Persons { get; set; }
    }
}
