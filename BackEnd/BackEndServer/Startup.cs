using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackEndServer.Services;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using BackEndServer.Services.PlaceholderServices;
using System.IO;

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

            bool alertMonitoringEnabled = true;
            if (alertMonitoringEnabled)
            {
                Thread alertMonitoringThread = new Thread(delegate()
                {
                    AlertMonitoringService alertMonitoringService = new AlertMonitoringService(dbQueryService);
                    alertMonitoringService.StartMonitoring();
                });
                alertMonitoringThread.Start();
            }

            // Other services are constructed using the database query service, meaning they all use the same connection string
            AbstractGraphStatisticService graphStatisticService = new GraphStatisticService(dbQueryService);
            AbstractLocationService locationService = new LocationService(dbQueryService);
            AbstractCameraService cameraService = new CameraService(dbQueryService, graphStatisticService, locationService);
            AbstractAuthenticationService authenticationService = new AuthenticationService(dbQueryService);
            AbstractDataMessageService dataMessageService = new DataMessageService(dbQueryService);
            AbstractAlertService alertService = new AlertService(dbQueryService, cameraService);
            AbstractNotificationService notificationService = new NotificationService(dbQueryService);
            AbstractUserService userService = new UserService(dbQueryService);
            
            services.Add(new ServiceDescriptor(typeof(AbstractAuthenticationService), authenticationService));
            services.Add(new ServiceDescriptor(typeof(AbstractCameraService), cameraService));
            services.Add(new ServiceDescriptor(typeof(AbstractDataMessageService), dataMessageService));
            services.Add(new ServiceDescriptor(typeof(AbstractLocationService), locationService));
            services.Add(new ServiceDescriptor(typeof(AbstractGraphStatisticService), graphStatisticService));
            services.Add(new ServiceDescriptor(typeof(AbstractAlertService), alertService));
            services.Add(new ServiceDescriptor(typeof(AbstractNotificationService), notificationService));
            services.Add(new ServiceDescriptor(typeof(AbstractUserService), userService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

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

        // This code is called when the application is stopped.
        private void OnShutdown()
        {
            // Deletes all files copied into the temp folder when testing. Prevents uploading them to GitHub.
            ClearWWWRootTempFolder();
        }

        private void ClearWWWRootTempFolder()
        {
            DirectoryInfo di = new DirectoryInfo(RootDirectoryTools.GetWWWRootTempFolderPhysicalPath());

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

    }
}
