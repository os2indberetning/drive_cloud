
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.DomainModel
{
    public enum ReportStatus
    {
        Pending,
        Accepted,
        Rejected,
        Invoiced,
        APIReady,
        APIFetched
    }

    public class Report
    {
        public int Id { get; set; }
        public ReportStatus Status { get; set; }
        public long CreatedDateTimestamp { get; set; }
        public long EditedDateTimestamp { get; set; }
        public String Comment { get; set; }
        public long ClosedDateTimestamp { get; set; }
        public long ProcessedDateTimestamp { get; set; }
        public virtual Person ApprovedBy { get; set; }
        public int? ApprovedById { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int EmploymentId { get; set; }
        public virtual Employment Employment { get; set; }

        public virtual ICollection<PersonReport> PersonReports { get; set; }

        public int? ActualLeaderId { get; set; }
        public virtual Person ActualLeader { get; set; }

        public bool IsPersonResponsible(Person person)
        {
            return IsPersonResponsible(person.Id);
        }

        public bool IsPersonResponsible(int personId)
        {
            return PersonReports.Select(p => p.PersonId).Contains(personId);
        }

        public void UpdateResponsibleLeaders(ICollection<Person> newlist)
        {
            if (newlist == null)
            {
                return;
            } 
            if (PersonReports == null)
            {
                PersonReports = new List<PersonReport>();
            }
                

            foreach (var personReport in PersonReports.ToList())
            {
                if (!newlist.Any(p => p.Id == personReport.PersonId))
                {
                    PersonReports.Remove(personReport);
                }
            }

            foreach (var person in newlist)
            {
                if (!PersonReports.Any(p => p.PersonId == person.Id))
                {
                    PersonReports.Add(new PersonReport() { PersonId = person.Id, Report = this });
                }
            }
        }
    }
}
