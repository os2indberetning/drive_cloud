using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.AppAPI.ViewModels
{
    public class OrgUnitViewModel
    {
        //public int Id { get; set; }
        public int OrgId { get; set; }
        public bool FourKmRuleAllowed { get; set; }
    }
}