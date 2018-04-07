using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Classes.EntityDefinitionClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackEndServer.Services;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;

namespace BackEndServer
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
            services.AddMvc().AddSessionStateTempDataProvider();
            //Add session support
            services.AddSession();
            // Allow the use of the MySQL Database as a service in this project.
            // Uses connection string from the project configuration
            // Passing it as a service ensures everything in the project will use this query service and use this connection string
            DatabaseQueryService dbQueryService = new DatabaseQueryService(Configuration.GetConnectionString("DefaultConnection"));
            
            // Other services are constructed using the database query service, meaning they all use the same connection string
            AbstractCameraService cameraService = new CameraService(dbQueryService);
            AbstractAuthenticationService authenticationService = new AuthenticationService(dbQueryService);
            AbstractDataMessageService dataMessageService = new DataMessageService(dbQueryService);
            AbstractLocationService locationService = new LocationService(dbQueryService);
            // Service not yet implemented and we are using a placeholder right now
            // When service is actually implemented, change PlaceholderGraphStatisticsService to GraphStatisticsService
            // Nothing else would need to be changed if things are done correctly
            AbstractGraphStatisticService graphStatisticService = new PlaceholderGraphStatisticsService(cameraService);
            
            services.Add(new ServiceDescriptor(typeof(AbstractAuthenticationService), authenticationService));
            services.Add(new ServiceDescriptor(typeof(AbstractCameraService), cameraService));
            services.Add(new ServiceDescriptor(typeof(AbstractDataMessageService), dataMessageService));
            services.Add(new ServiceDescriptor(typeof(AbstractLocationService), locationService));
            services.Add(new ServiceDescriptor(typeof(AbstractGraphStatisticService), graphStatisticService));
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=SignIn}/{id?}");
            });
        }
    }
}
