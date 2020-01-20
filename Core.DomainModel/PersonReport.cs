using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DomainModel
{
    public class PersonReport
    {
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int ReportId { get; set; }
        public virtual Report Report { get; set; }
    }
}
