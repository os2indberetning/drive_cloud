using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DomainModel
{
    public class APIPerson
    {
        public string CPR { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public APIAddress Address { get; set; }
        public string Email { get; set; }
        public IEnumerable<APIEmployment> Employments { get; set; }
        public string Initials { get; set; }
    }
}
