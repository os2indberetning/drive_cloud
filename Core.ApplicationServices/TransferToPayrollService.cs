using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Core.ApplicationServices
{
    public class TransferToPayrollService : ITransferToPayrollService
    {
        private readonly IGenericRepository<DriveReport> _driveReportRepo;
        private readonly ILogger _logger;

        public TransferToPayrollService(IGenericRepository<DriveReport> driveReportRepo, ILogger<TransferToPayrollService> logger)
        {
            _driveReportRepo = driveReportRepo;
            _logger = logger;
        }

        public void TransferReportsToPayroll()
        {
            _logger.LogInformation("Transerfering reports to payroll / api");
            foreach (var report in _driveReportRepo.AsQueryable().Where(r => r.Status == ReportStatus.Accepted))
            {
                report.Status = report.Distance == 0 ? ReportStatus.Invoiced : ReportStatus.APIReady;
                report.ProcessedDateTimestamp = GetTimeAsLong();
                _driveReportRepo.Update(report);
            }
            _driveReportRepo.Save();
        }

        private long GetTimeAsLong()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var deltaTime = DateTime.Now.ToUniversalTime() - epoch;
            return (long)deltaTime.TotalSeconds;
        }

    }
}
