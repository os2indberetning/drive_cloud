using Core.DomainModel;
using Microsoft.AspNet.OData;
using System.Collections.Generic;

namespace Core.ApplicationServices.Interfaces
{
    public interface IDriveReportService
    {
        DriveReport Create(DriveReport report);
        void SendMailForRejectedReport(int key, Delta<DriveReport> delta);

        void CalculateFourKmRuleForOtherReports(DriveReport report);

        List<Person> GetResponsibleLeadersForReport(DriveReport driveReport);
        Person GetActualLeaderForReport(DriveReport driveReport);
        bool Validate(DriveReport report);

        void SendMailToUserAndApproverOfEditedReport(DriveReport report, string emailText, Person admin, string action);
    }
}
