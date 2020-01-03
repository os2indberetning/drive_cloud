using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainServices;
using Core.DomainServices.Interfaces;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Presentation.Web.Config
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options => options.UseMySql(
                "Server=" + configuration["Database:Server"] + 
                ";Database=" + configuration["Database:Database"] + 
                ";Uid=" + configuration["Database:Uid"] + 
                ";Pwd=" + configuration["Database:Pwd"] + ";", 
                    mysqlOptions => mysqlOptions.ServerVersion(new System.Version(5, 6, 30), ServerType.MySql)).UseLazyLoadingProxies());
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IMobileTokenService,MobileTokenService>();
            services.AddScoped<IMailSender,MailSender>();
            services.AddScoped<IMailService,MailService>();
            services.AddScoped<ISubstituteService,SubstituteService>();
            services.AddScoped<IAddressCoordinates, AddressCoordinates>();
            services.AddScoped<IRoute<RouteInformation>, BestRoute>();
            services.AddScoped<IReimbursementCalculator,ReimbursementCalculator>();
            services.AddScoped<ILicensePlateService,LicensePlateService>();
            services.AddScoped<IPersonalRouteService,PersonalRouteService>();
            services.AddScoped<IAddressLaunderer,AddressLaundering>();
            services.AddScoped<IOrgUnitService,OrgUnitService>();
            services.AddScoped<IAppLoginService,AppLoginService>();
            services.AddScoped<IDriveReportService,DriveReportService>();
            services.AddScoped<APIService>();
            services.AddScoped<IUrlDefinitions,UrlDefinitions>();
            services.AddScoped<IRouter, SeptimaRouter>();
            return services;
        }
    }
}
