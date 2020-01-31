using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.DataAccess.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditlog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Controller = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditlog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileGenerationSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateTimestamp = table.Column<long>(nullable: false),
                    Repeat = table.Column<bool>(nullable: false),
                    Completed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileGenerationSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CprNumber = table.Column<string>(fixedLength: true, maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Mail = table.Column<string>(nullable: false),
                    RecieveMail = table.Column<bool>(nullable: false),
                    DistanceFromHomeToBorder = table.Column<double>(nullable: false),
                    Initials = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    AdminRecieveMail = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RateTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    TFCode = table.Column<string>(nullable: false),
                    TFCodeOptional = table.Column<string>(nullable: true),
                    RequiresLicensePlate = table.Column<bool>(nullable: false),
                    IsBike = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailNotificationSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateTimestamp = table.Column<long>(nullable: false),
                    FileGenerationScheduleId = table.Column<int>(nullable: false),
                    CustomText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailNotificationSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailNotificationSchedules_FileGenerationSchedules_FileGenera~",
                        column: x => x.FileGenerationScheduleId,
                        principalTable: "FileGenerationSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppLogin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: false),
                    GuId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLogin_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Plate = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.Id);
                    table.ForeignKey(
                        name: "FK_License_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalRoutes_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Year = table.Column<int>(nullable: false),
                    KmRate = table.Column<float>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rates_RateTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "RateTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentId = table.Column<int>(nullable: false),
                    WorkAddressId = table.Column<int>(nullable: true),
                    HomeAddressId = table.Column<int>(nullable: true),
                    StartTimestamp = table.Column<long>(nullable: false),
                    EndTimestamp = table.Column<long>(nullable: false),
                    IsMigrated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentId = table.Column<string>(nullable: false),
                    Position = table.Column<string>(nullable: false),
                    IsLeader = table.Column<bool>(nullable: false),
                    StartDateTimestamp = table.Column<long>(nullable: false),
                    EndDateTimestamp = table.Column<long>(nullable: false),
                    EmploymentType = table.Column<int>(nullable: false),
                    ExtraNumber = table.Column<int>(nullable: false),
                    WorkDistanceOverride = table.Column<double>(nullable: false),
                    HomeWorkDistance = table.Column<double>(nullable: false),
                    AlternativeWorkAddressId = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    OrgUnitId = table.Column<int>(nullable: false),
                    CostCenter = table.Column<long>(nullable: true),
                    InstituteCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employments_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(nullable: false),
                    CreatedDateTimestamp = table.Column<long>(nullable: false),
                    EditedDateTimestamp = table.Column<long>(nullable: false),
                    Comment = table.Column<string>(nullable: false),
                    ClosedDateTimestamp = table.Column<long>(nullable: false),
                    ProcessedDateTimestamp = table.Column<long>(nullable: false),
                    ApprovedById = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    EmploymentId = table.Column<int>(nullable: false),
                    ActualLeaderId = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Distance = table.Column<double>(nullable: true),
                    AmountToReimburse = table.Column<double>(nullable: true),
                    Purpose = table.Column<string>(nullable: true),
                    KmRate = table.Column<double>(nullable: true),
                    DriveDateTimestamp = table.Column<long>(nullable: true),
                    FourKmRule = table.Column<bool>(nullable: true),
                    FourKmRuleDeducted = table.Column<double>(nullable: true),
                    HomeToBorderDistance = table.Column<double>(nullable: true),
                    StartsAtHome = table.Column<bool>(nullable: true),
                    EndsAtHome = table.Column<bool>(nullable: true),
                    LicensePlate = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: true),
                    TFCode = table.Column<string>(nullable: true),
                    TFCodeOptional = table.Column<string>(nullable: true),
                    KilometerAllowance = table.Column<int>(nullable: true),
                    IsFromApp = table.Column<bool>(nullable: true),
                    AppUuid = table.Column<string>(nullable: true),
                    UserComment = table.Column<string>(nullable: true),
                    RouteGeometry = table.Column<string>(nullable: true),
                    IsExtraDistance = table.Column<bool>(nullable: true),
                    IsOldMigratedReport = table.Column<bool>(nullable: true),
                    IsRoundTrip = table.Column<bool>(nullable: true),
                    SixtyDaysRule = table.Column<bool>(nullable: true),
                    IsUsingDivergentAddress = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Persons_ActualLeaderId",
                        column: x => x.ActualLeaderId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Persons_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Employments_EmploymentId",
                        column: x => x.EmploymentId,
                        principalTable: "Employments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StreetName = table.Column<string>(nullable: false),
                    StreetNumber = table.Column<string>(nullable: false),
                    ZipCode = table.Column<int>(nullable: false),
                    Town = table.Column<string>(nullable: false),
                    Longitude = table.Column<string>(nullable: false),
                    Latitude = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    IsDirty = table.Column<bool>(nullable: true),
                    DirtyString = table.Column<string>(nullable: true),
                    NextPointId = table.Column<int>(nullable: true),
                    PreviousPointId = table.Column<int>(nullable: true),
                    DriveReportId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: true),
                    Point_NextPointId = table.Column<int>(nullable: true),
                    Point_PreviousPointId = table.Column<int>(nullable: true),
                    PersonalRouteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Reports_DriveReportId",
                        column: x => x.DriveReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Addresses_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Addresses_Addresses_Point_NextPointId",
                        column: x => x.Point_NextPointId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_PersonalRoutes_PersonalRouteId",
                        column: x => x.PersonalRouteId,
                        principalTable: "PersonalRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonReport",
                columns: table => new
                {
                    PersonId = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonReport", x => new { x.PersonId, x.ReportId });
                    table.ForeignKey(
                        name: "FK_PersonReport_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonReport_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrgUnits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrgId = table.Column<int>(nullable: false),
                    OrgOUID = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: false),
                    LongDescription = table.Column<string>(nullable: true),
                    HasAccessToFourKmRule = table.Column<bool>(nullable: false),
                    DefaultKilometerAllowance = table.Column<int>(nullable: false),
                    AddressId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgUnits_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrgUnits_OrgUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrgUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Substitutes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartDateTimestamp = table.Column<long>(nullable: false),
                    EndDateTimestamp = table.Column<long>(nullable: false),
                    LeaderId = table.Column<int>(nullable: false),
                    SubId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    OrgUnitId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: true),
                    TakesOverOriginalLeaderReports = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substitutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Substitutes_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Substitutes_Persons_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Substitutes_OrgUnits_OrgUnitId",
                        column: x => x.OrgUnitId,
                        principalTable: "OrgUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Substitutes_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Substitutes_Persons_SubId",
                        column: x => x.SubId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_DriveReportId",
                table: "Addresses",
                column: "DriveReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PersonId",
                table: "Addresses",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Point_NextPointId",
                table: "Addresses",
                column: "Point_NextPointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PersonalRouteId",
                table: "Addresses",
                column: "PersonalRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressHistory_EmploymentId",
                table: "AddressHistory",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressHistory_HomeAddressId",
                table: "AddressHistory",
                column: "HomeAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressHistory_WorkAddressId",
                table: "AddressHistory",
                column: "WorkAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogin_PersonId",
                table: "AppLogin",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_AlternativeWorkAddressId",
                table: "Employments",
                column: "AlternativeWorkAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_OrgUnitId",
                table: "Employments",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_PersonId",
                table: "Employments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_License_PersonId",
                table: "License",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotificationSchedules_FileGenerationScheduleId",
                table: "MailNotificationSchedules",
                column: "FileGenerationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnits_AddressId",
                table: "OrgUnits",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnits_ParentId",
                table: "OrgUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalRoutes_PersonId",
                table: "PersonalRoutes",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonReport_ReportId",
                table: "PersonReport",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_TypeId",
                table: "Rates",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AppUuid",
                table: "Reports",
                column: "AppUuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ActualLeaderId",
                table: "Reports",
                column: "ActualLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ApprovedById",
                table: "Reports",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EmploymentId",
                table: "Reports",
                column: "EmploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PersonId",
                table: "Reports",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutes_CreatedById",
                table: "Substitutes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutes_LeaderId",
                table: "Substitutes",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutes_OrgUnitId",
                table: "Substitutes",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutes_PersonId",
                table: "Substitutes",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutes_SubId",
                table: "Substitutes",
                column: "SubId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressHistory_Employments_EmploymentId",
                table: "AddressHistory",
                column: "EmploymentId",
                principalTable: "Employments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressHistory_Addresses_HomeAddressId",
                table: "AddressHistory",
                column: "HomeAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressHistory_Addresses_WorkAddressId",
                table: "AddressHistory",
                column: "WorkAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_Addresses_AlternativeWorkAddressId",
                table: "Employments",
                column: "AlternativeWorkAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_OrgUnits_OrgUnitId",
                table: "Employments",
                column: "OrgUnitId",
                principalTable: "OrgUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Reports_DriveReportId",
                table: "Addresses");

            migrationBuilder.DropTable(
                name: "AddressHistory");

            migrationBuilder.DropTable(
                name: "AppLogin");

            migrationBuilder.DropTable(
                name: "Auditlog");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "License");

            migrationBuilder.DropTable(
                name: "MailNotificationSchedules");

            migrationBuilder.DropTable(
                name: "PersonReport");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "Substitutes");

            migrationBuilder.DropTable(
                name: "FileGenerationSchedules");

            migrationBuilder.DropTable(
                name: "RateTypes");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Employments");

            migrationBuilder.DropTable(
                name: "OrgUnits");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "PersonalRoutes");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
