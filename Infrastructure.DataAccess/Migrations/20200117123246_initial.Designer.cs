﻿// <auto-generated />
using System;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.DataAccess.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200117123246_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Core.DomainModel.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Latitude")
                        .IsRequired();

                    b.Property<string>("Longitude")
                        .IsRequired();

                    b.Property<string>("StreetName")
                        .IsRequired();

                    b.Property<string>("StreetNumber")
                        .IsRequired();

                    b.Property<string>("Town")
                        .IsRequired();

                    b.Property<int>("ZipCode");

                    b.HasKey("Id");

                    b.ToTable("Addresses");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Address");
                });

            modelBuilder.Entity("Core.DomainModel.AddressHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EmploymentId");

                    b.Property<long>("EndTimestamp");

                    b.Property<int?>("HomeAddressId");

                    b.Property<bool>("IsMigrated");

                    b.Property<long>("StartTimestamp");

                    b.Property<int?>("WorkAddressId");

                    b.HasKey("Id");

                    b.HasIndex("EmploymentId");

                    b.HasIndex("HomeAddressId");

                    b.HasIndex("WorkAddressId");

                    b.ToTable("AddressHistory");
                });

            modelBuilder.Entity("Core.DomainModel.AppLogin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GuId");

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<int>("PersonId");

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("AppLogin");
                });

            modelBuilder.Entity("Core.DomainModel.Auditlog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action");

                    b.Property<string>("Controller");

                    b.Property<string>("Location");

                    b.Property<string>("Parameters");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("User");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Auditlog");
                });

            modelBuilder.Entity("Core.DomainModel.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Number")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("BankAccounts");
                });

            modelBuilder.Entity("Core.DomainModel.Employment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AlternativeWorkAddressId");

                    b.Property<long?>("CostCenter");

                    b.Property<string>("EmploymentId")
                        .IsRequired();

                    b.Property<int>("EmploymentType");

                    b.Property<long>("EndDateTimestamp");

                    b.Property<int>("ExtraNumber");

                    b.Property<double>("HomeWorkDistance");

                    b.Property<string>("InstituteCode");

                    b.Property<bool>("IsLeader");

                    b.Property<int>("OrgUnitId");

                    b.Property<int>("PersonId");

                    b.Property<string>("Position")
                        .IsRequired();

                    b.Property<long>("StartDateTimestamp");

                    b.Property<double>("WorkDistanceOverride");

                    b.HasKey("Id");

                    b.HasIndex("AlternativeWorkAddressId");

                    b.HasIndex("OrgUnitId");

                    b.HasIndex("PersonId");

                    b.ToTable("Employments");
                });

            modelBuilder.Entity("Core.DomainModel.FileGenerationSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Completed");

                    b.Property<long>("DateTimestamp");

                    b.Property<bool>("Repeat");

                    b.HasKey("Id");

                    b.ToTable("FileGenerationSchedules");
                });

            modelBuilder.Entity("Core.DomainModel.LicensePlate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<bool>("IsPrimary");

                    b.Property<int>("PersonId");

                    b.Property<string>("Plate")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("License");
                });

            modelBuilder.Entity("Core.DomainModel.MailNotificationSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CustomText");

                    b.Property<long>("DateTimestamp");

                    b.Property<int>("FileGenerationScheduleId");

                    b.HasKey("Id");

                    b.HasIndex("FileGenerationScheduleId");

                    b.ToTable("MailNotificationSchedules");
                });

            modelBuilder.Entity("Core.DomainModel.MobileToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<Guid>("Guid");

                    b.Property<int>("PersonId");

                    b.Property<int>("Status");

                    b.Property<string>("Token")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("MobileTokens");
                });

            modelBuilder.Entity("Core.DomainModel.OrgUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<int>("DefaultKilometerAllowance");

                    b.Property<bool>("HasAccessToFourKmRule");

                    b.Property<string>("LongDescription");

                    b.Property<int>("OrgId");

                    b.Property<string>("OrgOUID");

                    b.Property<int?>("ParentId");

                    b.Property<string>("ShortDescription")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("ParentId");

                    b.ToTable("OrgUnits");
                });

            modelBuilder.Entity("Core.DomainModel.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AdminRecieveMail");

                    b.Property<string>("CprNumber")
                        .IsRequired()
                        .IsFixedLength(true)
                        .HasMaxLength(10);

                    b.Property<double>("DistanceFromHomeToBorder");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<string>("Initials")
                        .IsRequired();

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Mail")
                        .IsRequired();

                    b.Property<bool>("RecieveMail");

                    b.Property<int?>("ReportId");

                    b.HasKey("Id");

                    b.HasIndex("ReportId");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Core.DomainModel.PersonalRoute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("PersonId");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("PersonalRoutes");
                });

            modelBuilder.Entity("Core.DomainModel.Rate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<float>("KmRate");

                    b.Property<int>("TypeId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("Rates");
                });

            modelBuilder.Entity("Core.DomainModel.RateType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<bool>("IsBike");

                    b.Property<bool>("RequiresLicensePlate");

                    b.Property<string>("TFCode")
                        .IsRequired();

                    b.Property<string>("TFCodeOptional");

                    b.HasKey("Id");

                    b.ToTable("RateTypes");
                });

            modelBuilder.Entity("Core.DomainModel.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActualLeaderId");

                    b.Property<int?>("ApprovedById");

                    b.Property<long>("ClosedDateTimestamp");

                    b.Property<string>("Comment")
                        .IsRequired();

                    b.Property<long>("CreatedDateTimestamp");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<long>("EditedDateTimestamp");

                    b.Property<int>("EmploymentId");

                    b.Property<int>("PersonId");

                    b.Property<long>("ProcessedDateTimestamp");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("ActualLeaderId");

                    b.HasIndex("ApprovedById");

                    b.HasIndex("EmploymentId");

                    b.HasIndex("PersonId");

                    b.ToTable("Reports");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Report");
                });

            modelBuilder.Entity("Core.DomainModel.Substitute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CreatedById");

                    b.Property<long>("EndDateTimestamp");

                    b.Property<int>("LeaderId");

                    b.Property<int>("OrgUnitId");

                    b.Property<int>("PersonId");

                    b.Property<long>("StartDateTimestamp");

                    b.Property<int>("SubId");

                    b.Property<bool>("TakesOverOriginalLeaderReports");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LeaderId");

                    b.HasIndex("OrgUnitId");

                    b.HasIndex("PersonId");

                    b.HasIndex("SubId");

                    b.ToTable("Substitutes");
                });

            modelBuilder.Entity("Core.DomainModel.CachedAddress", b =>
                {
                    b.HasBaseType("Core.DomainModel.Address");

                    b.Property<string>("DirtyString");

                    b.Property<bool>("IsDirty");

                    b.HasDiscriminator().HasValue("CachedAddress");
                });

            modelBuilder.Entity("Core.DomainModel.DriveReportPoint", b =>
                {
                    b.HasBaseType("Core.DomainModel.Address");

                    b.Property<int>("DriveReportId");

                    b.Property<int?>("NextPointId");

                    b.Property<int?>("PreviousPointId");

                    b.HasIndex("DriveReportId");

                    b.HasDiscriminator().HasValue("DriveReportPoint");
                });

            modelBuilder.Entity("Core.DomainModel.PersonalAddress", b =>
                {
                    b.HasBaseType("Core.DomainModel.Address");

                    b.Property<int>("PersonId");

                    b.Property<int>("Type");

                    b.HasIndex("PersonId");

                    b.HasDiscriminator().HasValue("PersonalAddress");
                });

            modelBuilder.Entity("Core.DomainModel.Point", b =>
                {
                    b.HasBaseType("Core.DomainModel.Address");

                    b.Property<int?>("NextPointId")
                        .HasColumnName("Point_NextPointId");

                    b.Property<int>("PersonalRouteId");

                    b.Property<int?>("PreviousPointId")
                        .HasColumnName("Point_PreviousPointId");

                    b.HasIndex("NextPointId")
                        .IsUnique();

                    b.HasIndex("PersonalRouteId");

                    b.HasDiscriminator().HasValue("Point");
                });

            modelBuilder.Entity("Core.DomainModel.WorkAddress", b =>
                {
                    b.HasBaseType("Core.DomainModel.Address");

                    b.HasDiscriminator().HasValue("WorkAddress");
                });

            modelBuilder.Entity("Core.DomainModel.DriveReport", b =>
                {
                    b.HasBaseType("Core.DomainModel.Report");

                    b.Property<string>("AccountNumber");

                    b.Property<double>("AmountToReimburse");

                    b.Property<double>("Distance");

                    b.Property<long>("DriveDateTimestamp");

                    b.Property<bool>("EndsAtHome");

                    b.Property<bool>("FourKmRule");

                    b.Property<double>("FourKmRuleDeducted");

                    b.Property<string>("FullName");

                    b.Property<double>("HomeToBorderDistance");

                    b.Property<bool?>("IsExtraDistance");

                    b.Property<bool>("IsFromApp");

                    b.Property<bool?>("IsOldMigratedReport");

                    b.Property<bool?>("IsRoundTrip");

                    b.Property<bool>("IsUsingDivergentAddress");

                    b.Property<int>("KilometerAllowance");

                    b.Property<double>("KmRate");

                    b.Property<string>("LicensePlate");

                    b.Property<string>("Purpose")
                        .IsRequired();

                    b.Property<string>("RouteGeometry");

                    b.Property<bool>("SixtyDaysRule");

                    b.Property<bool>("StartsAtHome");

                    b.Property<string>("TFCode")
                        .IsRequired();

                    b.Property<string>("TFCodeOptional");

                    b.Property<string>("UserComment");

                    b.HasDiscriminator().HasValue("DriveReport");
                });

            modelBuilder.Entity("Core.DomainModel.AddressHistory", b =>
                {
                    b.HasOne("Core.DomainModel.Employment", "Employment")
                        .WithMany()
                        .HasForeignKey("EmploymentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.DomainModel.PersonalAddress", "HomeAddress")
                        .WithMany()
                        .HasForeignKey("HomeAddressId");

                    b.HasOne("Core.DomainModel.WorkAddress", "WorkAddress")
                        .WithMany()
                        .HasForeignKey("WorkAddressId");
                });

            modelBuilder.Entity("Core.DomainModel.AppLogin", b =>
                {
                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.Employment", b =>
                {
                    b.HasOne("Core.DomainModel.PersonalAddress", "AlternativeWorkAddress")
                        .WithMany()
                        .HasForeignKey("AlternativeWorkAddressId");

                    b.HasOne("Core.DomainModel.OrgUnit", "OrgUnit")
                        .WithMany("Employments")
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany("Employments")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.LicensePlate", b =>
                {
                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany("LicensePlates")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.MailNotificationSchedule", b =>
                {
                    b.HasOne("Core.DomainModel.FileGenerationSchedule", "FileGenerationSchedule")
                        .WithMany("MailNotificationSchedules")
                        .HasForeignKey("FileGenerationScheduleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.MobileToken", b =>
                {
                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany("MobileTokens")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.OrgUnit", b =>
                {
                    b.HasOne("Core.DomainModel.WorkAddress", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.DomainModel.OrgUnit", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Core.DomainModel.Person", b =>
                {
                    b.HasOne("Core.DomainModel.Report")
                        .WithMany("ResponsibleLeaders")
                        .HasForeignKey("ReportId");
                });

            modelBuilder.Entity("Core.DomainModel.PersonalRoute", b =>
                {
                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany("PersonalRoutes")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.Rate", b =>
                {
                    b.HasOne("Core.DomainModel.RateType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.Report", b =>
                {
                    b.HasOne("Core.DomainModel.Person", "ActualLeader")
                        .WithMany()
                        .HasForeignKey("ActualLeaderId");

                    b.HasOne("Core.DomainModel.Person", "ApprovedBy")
                        .WithMany()
                        .HasForeignKey("ApprovedById");

                    b.HasOne("Core.DomainModel.Employment", "Employment")
                        .WithMany("Reports")
                        .HasForeignKey("EmploymentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany("Reports")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.Substitute", b =>
                {
                    b.HasOne("Core.DomainModel.Person", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Core.DomainModel.Person", "Leader")
                        .WithMany("Substitutes")
                        .HasForeignKey("LeaderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.DomainModel.OrgUnit", "OrgUnit")
                        .WithMany("Substitutes")
                        .HasForeignKey("OrgUnitId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany("SubstituteFor")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.DomainModel.Person", "Sub")
                        .WithMany("SubstituteLeaders")
                        .HasForeignKey("SubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.DriveReportPoint", b =>
                {
                    b.HasOne("Core.DomainModel.DriveReport", "DriveReport")
                        .WithMany("DriveReportPoints")
                        .HasForeignKey("DriveReportId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.PersonalAddress", b =>
                {
                    b.HasOne("Core.DomainModel.Person", "Person")
                        .WithMany("PersonalAddresses")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.DomainModel.Point", b =>
                {
                    b.HasOne("Core.DomainModel.Point", "NextPoint")
                        .WithOne("PreviousPoint")
                        .HasForeignKey("Core.DomainModel.Point", "NextPointId");

                    b.HasOne("Core.DomainModel.PersonalRoute", "PersonalRoute")
                        .WithMany("Points")
                        .HasForeignKey("PersonalRouteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}