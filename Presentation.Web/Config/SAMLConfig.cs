using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Presentation.Web.Config
{

    public static class SAMLConfig
    {
        public static IServiceCollection AddSAMLAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<Saml2Options>>();
            services.AddAuthentication()
                .AddSaml2(options =>
                {
                    options.SPOptions.Logger = new AspNetCoreLoggerAdapter(logger);
                    options.SPOptions.EntityId = new EntityId(configuration["SAML:EntityId"]);
                    options.SPOptions.PublicOrigin = new Uri(configuration["SAML:PublicOrigin"]);
                    options.SPOptions.ReturnUrl = new Uri(options.SPOptions.PublicOrigin, "index");
                    options.IdentityProviders.Add(
                        new IdentityProvider(
                            new EntityId(configuration["SAML:IdpEntityId"]), options.SPOptions)
                        {
                            MetadataLocation = configuration["SAML:IdpMetadataLocation"]
                            ,LoadMetadata = true
                        });
                    options.SPOptions.ServiceCertificates.Add(new X509Certificate2(configuration["SAML:CertificateFilename"], configuration["SAML:CertificatePassword"]));
                    // ignore unsolved bug that throws exception occasionally
                    // https://github.com/Sustainsys/Saml2/commit/15bdb4784830a877d7b7b3cfd91bb7e6043fabf4
                    options.Notifications.Unsafe.IgnoreUnexpectedInResponseTo = (r, c) =>
                    {
                        return true;
                    };
                });
            return services;
        }
    }
}