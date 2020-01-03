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

            builder.EntitySet<DriveReportPoint>("DriveReportPoints");

            builder.EntitySet<Employment>("Employments");
            var eType = builder.EntityType<Employment>();
            eType.HasKey(e => e.Id);

            builder.EntitySet<FileGenerationSchedule>("FileGenerationSchedules");

            builder.EntitySet<LicensePlate>("LicensePlates");

            builder.EntitySet<MailNotificationSchedule>("MailNotifications");

            builder.EntitySet<RateType>("RateTypes");

            builder.EntitySet<MobileToken>("MobileToken");

            builder.EntitySet<OrgUnit>("OrgUnits");

            builder.EntitySet<AppLogin>("AppLogin");

            builder.EntitySet<Person>("Person");
            var pType = builder.EntityType<Person>();
            pType.HasKey(p => p.Id);

            builder.EntityType<Person>().Collection
           .Function("GetCurrentUser")
           .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<Person>().Collection
            .Function("GetUserAsCurrentUser")
            .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<Person>().Collection
                .Function("LeadersPeople")
                .ReturnsFromEntitySet<Person>("Person")
                .Parameter<int>("Type");

            builder.EntityType<Person>().Collection
                .Function("PeopleInMyOrganisation")
                .ReturnsFromEntitySet<Person>("Person")
                .Parameter<int>("Id");

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
                .ReturnsFromEntitySet<Substitute>("Substitutes")
                .Parameter<int>("Type");
            builder.EntityType<Substitute>().Collection
                .Function("Substitute")
                .ReturnsFromEntitySet<Substitute>("Substitutes")
                .Parameter<int>("Type");

            builder.EntitySet<Rate>("Rates");
            builder.EntityType<Rate>().Collection
                .Function("ThisYearsRates")
                .ReturnsFromEntitySet<Rate>("Rates");

            builder.EntityType<OrgUnit>().Collection
                .Function("SetVacationAccess")
                .ReturnsFromEntitySet<OrgUnit>("OrgUnits")
                .Parameter<int>("Key");

            builder.Namespace = "Service";

            return builder.GetEdmModel();
        }
    }
}
