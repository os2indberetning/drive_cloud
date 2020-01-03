using Core.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PersonalAddress> PersonalAddresses { get; set; }
        public DbSet<PersonalRoute> PersonalRoutes { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<LicensePlate> License { get; set; }
        public DbSet<MobileToken> MobileTokens { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<MailNotificationSchedule> MailNotificationSchedules { get; set; }
        public DbSet<FileGenerationSchedule> FileGenerationSchedules { get; set; }
        public DbSet<DriveReportPoint> DriveReportPoints { get; set; }
        public DbSet<DriveReport> DriveReports { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<Substitute> Substitutes { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; } 
        public DbSet<RateType> RateTypes { get; set; }
        public DbSet<CachedAddress> CachedAddresses { get; set; }
        public DbSet<AddressHistory> AddressHistory { get; set; }
        public DbSet<AppLogin> AppLogin { get; set; }
        public DbSet<Auditlog> Auditlog { get; set; }


        /**
         * Sets up 
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigurePropertiesForPerson(modelBuilder);
            ConfigurePropertiesForAddress(modelBuilder);
            ConfigurePropertiesForPersonalAddress(modelBuilder);
            ConfigurePropertiesForPersonalRoute(modelBuilder);
            ConfigurePropertiesForPoint(modelBuilder);
            ConfigurePropertiesForLicensePlate(modelBuilder);
            ConfigurePropertiesForMobileToken(modelBuilder);
            ConfigurePropertiesForRate(modelBuilder);
            ConfigurePropertiesForMailNoficationSchedule(modelBuilder);
            ConfigurePropertiesForFileGenerationSchedule(modelBuilder);
            ConfigurePropertiesForDriveReportPoint(modelBuilder);
            ConfigurePropertiesForDriveReport(modelBuilder);
            ConfigurePropertiesForReport(modelBuilder);
            ConfigurePropertiesForEmployment(modelBuilder);
            ConfigurePropertiesForOrgUnit(modelBuilder);
            ConfigurePropertiesForSubstitute(modelBuilder);
            ConfigurePropertiesForBankAccount(modelBuilder);
            ConfigurePropertiesForRateType(modelBuilder);
            ConfigurePropertiesForCachedAddress(modelBuilder);
            ConfigurePropertiesForWorkAddress(modelBuilder);
            ConfigurePropertiesForAppLogin(modelBuilder);
        }

        private void ConfigurePropertiesForPerson(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.CprNumber).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.Mail).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.Initials).IsRequired();
            modelBuilder.Entity<Person>().Property(t => t.CprNumber).IsFixedLength().HasMaxLength(10);
            modelBuilder.Entity<Person>().Property(t => t.FullName).IsRequired();
            modelBuilder.Entity<Person>().Ignore(t => t.IsSubstitute);
            modelBuilder.Entity<Person>().Ignore(t => t.HasAppPassword);
        }

        private void ConfigurePropertiesForCachedAddress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CachedAddress>().Property(p => p.IsDirty).IsRequired();
        }

        private void ConfigurePropertiesForAddress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().Property(p => p.StreetName).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.StreetNumber).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.ZipCode).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Town).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Longitude).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Latitude).IsRequired();      
        }

        private void ConfigurePropertiesForWorkAddress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkAddress>().Ignore(p => p.OrgUnitId);
        }

        private void ConfigurePropertiesForRateType(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RateType>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<RateType>().Property(p => p.TFCode).IsRequired();
        }

        private void ConfigurePropertiesForPersonalAddress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonalAddress>().Property(p => p.Type).IsRequired();
            modelBuilder.Entity<PersonalAddress>().HasOne(p => p.Person);
        }

        private void ConfigurePropertiesForBankAccount(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<BankAccount>().Property(p => p.Number).IsRequired();
        }

        private void ConfigurePropertiesForPersonalRoute(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonalRoute>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<PersonalRoute>().HasOne(p => p.Person);
        }

        private void ConfigurePropertiesForPoint(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Point>().HasOne(p => p.PersonalRoute);
            modelBuilder.Entity<Point>().HasOne(p => p.NextPoint).WithOne(p => p.PreviousPoint);
        }

        private void ConfigurePropertiesForLicensePlate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicensePlate>().Property(p => p.Plate).IsRequired();
            modelBuilder.Entity<LicensePlate>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<LicensePlate>().HasOne(p => p.Person);
        }

        private void ConfigurePropertiesForMobileToken(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MobileToken>().Property(p => p.Guid).IsRequired();
            modelBuilder.Entity<MobileToken>().Property(p => p.Status).IsRequired();
            modelBuilder.Entity<MobileToken>().Property(p => p.Token).IsRequired();
            modelBuilder.Entity<MobileToken>().HasOne(p => p.Person);
            modelBuilder.Entity<MobileToken>().Ignore(p => p.StatusToPresent);
        }

        private void ConfigurePropertiesForRate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rate>().Property(p => p.Year).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.KmRate).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.TypeId).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.Active).IsRequired();
        }

        private void ConfigurePropertiesForMailNoficationSchedule(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.DateTimestamp).IsRequired();
            //modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.Notified).IsRequired();
            //modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.Repeat).IsRequired();
            
        }

        private void ConfigurePropertiesForFileGenerationSchedule(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileGenerationSchedule>().Property(p => p.DateTimestamp).IsRequired();
            //modelBuilder.Entity<FileGenerationSchedule>().Property(p => p.Generated).IsRequired();
        }

        private void ConfigurePropertiesForDriveReportPoint(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriveReportPoint>().HasOne(p => p.DriveReport);
        }

        private void ConfigurePropertiesForDriveReport(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriveReport>().Property(p => p.Distance).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.AmountToReimburse).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.Purpose).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.KmRate).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.DriveDateTimestamp).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.FourKmRule).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.StartsAtHome).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.EndsAtHome).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.TFCode).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.IsFromApp).IsRequired();
            modelBuilder.Entity<DriveReport>().Ignore(p => p.WorkAddressId);
        }

        private void ConfigurePropertiesForReport(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>().Property(p => p.Status).IsRequired();
            modelBuilder.Entity<Report>().Property(p => p.CreatedDateTimestamp).IsRequired();
            modelBuilder.Entity<Report>().Property(p => p.Comment).IsRequired();
            modelBuilder.Entity<Report>().HasOne(p => p.Person);
            modelBuilder.Entity<Report>().HasOne(p => p.Employment);
            modelBuilder.Entity<Report>().HasMany(c => c.ResponsibleLeaders);
            // todo is many-to-many really needed here?
                //.WithMany()
                //.Map(x =>
                //{
                //    x.MapLeftKey("ReportId");
                //    x.MapRightKey("PersonId");
                //    x.ToTable("ReportPersonMapping");
                //});
        }

        private void ConfigurePropertiesForEmployment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employment>().Property(p => p.EmploymentId).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.Position).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.IsLeader).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.StartDateTimestamp).IsRequired();
        }

        private void ConfigurePropertiesForOrgUnit(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrgUnit>().Property(p => p.OrgId).IsRequired();
            modelBuilder.Entity<OrgUnit>().Property(p => p.ShortDescription).IsRequired();
            modelBuilder.Entity<OrgUnit>().Property(p => p.Level).IsRequired();
        }

        private void ConfigurePropertiesForAppLogin(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppLogin>().Property(p => p.Password).IsRequired();
            modelBuilder.Entity<AppLogin>().Property(p => p.PersonId).IsRequired();
            modelBuilder.Entity<AppLogin>().Property(p => p.Salt).IsRequired();
            modelBuilder.Entity<AppLogin>().Property(p => p.UserName).IsRequired();
        }

        private void ConfigurePropertiesForSubstitute(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Substitute>().Property(p => p.StartDateTimestamp).IsRequired();

            modelBuilder.Entity<Substitute>().HasOne(p => p.OrgUnit);

            modelBuilder.Entity<Substitute>().HasOne(p => p.Leader).WithMany(p => p.Substitutes);
            modelBuilder.Entity<Substitute>().HasOne(p => p.Sub).WithMany(p => p.SubstituteLeaders);
            modelBuilder.Entity<Substitute>().HasOne(p => p.Person).WithMany(p => p.SubstituteFor);
        }

    }
}
