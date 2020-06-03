using System;

namespace Core.DomainModel
{
    public class APIEmployment
    {
        public string EmployeeNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string OrgUnitId { get; set; }
        public string Position { get; set; }
        public string CostCenter { get; set; }
        public bool Manager { get; set; }
        public int? ExtraNumber { get; set; }
        public int? EmploymentType { get; set; }
        public string InstituteCode { get; set; }
    }
}
