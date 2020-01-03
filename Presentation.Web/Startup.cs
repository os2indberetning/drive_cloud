using Infrastructure.DataAccess;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Web.Auth;
using Presentation.Web.Config;
using System.IO;

namespace Presentation.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDependencies(Configuration);
            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession();
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Configuration["DataProtectionPath"]));
            services.AddIdentity<IdentityPerson, IdentityRole>()
                .AddUserStore<PersonUserStore>()
                .AddRoleStore<PersonRoleStore>();              
            services.AddOData();
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSAMLAuthentication(Configuration);
            services.AddAuthentication(o => o.AddScheme(APIAuthenticationHandler.AuthenticationScheme, a => a.HandlerType = typeof(APIAuthenticationHandler)));

            services.AddJobs(Configuration);
            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            InitializeDatabase(app);
            app.UseAuthentication();
            app.UseResponseCaching();
            app.UseMvc(r => RouteConfig.Use(r));
            app.UseStaticFiles();
            app.UseFileServer(enableDirectoryBrowsing: false);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<DataContext>().Database.Migrate();
            }
        }
    }
}