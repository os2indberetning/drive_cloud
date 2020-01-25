using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNet.OData.Builder;
using Core.DomainModel;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;

namespace Presentation.Web.Config
{
    public static class RouteConfig
    {
        public static IRouteBuilder Use(IRouteBuilder routeBuilder)
        {
            routeBuilder.Select().Filter().Expand().Count().MaxTop(100).OrderBy();            
            routeBuilder.MapODataServiceRoute("odata", "odata", GetODataModel());
            return routeBuilder;
        }

        private static IEdmModel GetODataModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
            .Action("SetCoordinatesOnAddress")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
            .Action("SetCoordinatesOnAddressList")
            .ReturnsFromEntitySet<Address>("Addresses");



            builder.EntityType<Address>().Collection
            .Function("GetPersonalAndStandard")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
            .Function("GetStandard")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
            .Function("GetCachedAddresses")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
           .Action("AttemptCleanCachedAddress")
           .ReturnsFromEntitySet<Address>("Addresses");





            builder.EntityType<Address>().Collection
                .Function("GetMapStart")
                .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntitySet<DriveReport>("DriveReports");

            builder.EntityType<DriveReport>().Collection
            .Function("GetLatestReportForUser")
            .ReturnsFromEntitySet<DriveReport>("DriveReports");

            builder.EntityType<DriveReport>().Collection
            .Function("GetCalculationMethod")
            .ReturnsFromEntitySet<DriveReport>("DriveReports");

            builder.EntityType<DriveReport>().Collection
            .Function("TransferReportsToPayroll")
            .ReturnsFromEntitySet<DriveReport>("DriveReports");

            builder.EntitySet<DriveReportPoint>("DriveReportPoints");
            builder.EntityType<DriveReport>().Collection
             .Function("Eksport")
             .ReturnsFromEntitySet<DriveReport>("DriveReports");


            builder.EntitySet<Employment>("Employments");
            builder.EntityType<Employment>().HasKey(e => e.Id);

            builder.EntitySet<FileGenerationSchedule>("FileGenerationSchedule");

            builder.EntitySet<LicensePlate>("LicensePlates");

            //var lType = builder.EntityType<LicensePlate>();
            //lType.Ignore(l => l.Person);


            builder.EntitySet<MailNotificationSchedule>("MailNotifications");

            builder.EntitySet<RateType>("RateTypes");

            builder.EntitySet<MobileToken>("MobileToken");

            builder.EntitySet<OrgUnit>("OrgUnits");


            builder.EntitySet<AppLogin>("AppLogin");

            builder.EntitySet<Person>("Person");
            var pType = builder.EntityType<Person>();
            pType.HasKey(p => p.Id);
            //pType.Ignore(p => p.LicensePlates);


            builder.EntityType<Person>().Collection
           .Function("GetCurrentUser")
           .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<Person>().Collection
           .Function("GetDistanceFromHome")
           .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<Person>().Collection
            .Function("GetUserAsCurrentUser")
            .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<Person>().Collection
            .Function("GetEmployeesOfLeader")
            .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<OrgUnit>().Collection
            .Function("GetOrgUnitsForLeader")
            .ReturnsFromEntitySet<OrgUnit>("OrgUnits");



            builder.EntitySet<PersonalAddress>("PersonalAddresses");

            builder.EntityType<PersonalAddress>().Collection
            .Function("GetHome")
            .ReturnsFromEntitySet<PersonalAddress>("PersonalAddresses");

            builder.EntityType<PersonalAddress>().Collection
            .Function("GetRealHome")
            .ReturnsFromEntitySet<PersonalAddress>("PersonalAddresses");




            builder.EntityType<PersonalAddress>().Collection
            .Function("GetAlternativeHome")
            .ReturnsFromEntitySet<PersonalAddress>("PersonalAddresses");


            builder.EntityType<OrgUnit>().Collection
            .Function("GetLeaderOfOrg")
            .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<OrgUnit>().Collection
            .Function("GetWhereUserIsResponsible")
            .ReturnsFromEntitySet<OrgUnit>("OrgUnits");


            builder.EntitySet<PersonalRoute>("PersonalRoutes");

            builder.EntitySet<Point>("Points");

            builder.EntitySet<Report>("Reports");



            builder.EntitySet<BankAccount>("BankAccounts");

            builder.EntitySet<Person>("Person");
            builder.EntityType<Person>()
                .Action("HasLicensePlate");


            builder.EntitySet<Substitute>("Substitutes");
            builder.EntityType<Substitute>().Collection
                .Function("Personal")
                .ReturnsFromEntitySet<Substitute>("Substitutes");
            builder.EntityType<Substitute>().Collection
                .Function("Substitute")
                .ReturnsFromEntitySet<Substitute>("Substitutes");

            builder.EntitySet<Rate>("Rates");
            builder.EntityType<Rate>().Collection
                .Function("ThisYearsRates")
                .ReturnsFromEntitySet<Rate>("Rates");

            builder.Namespace = "Service";

            return builder.GetEdmModel();
        }
    }
}
