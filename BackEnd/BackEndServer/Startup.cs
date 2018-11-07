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
using Castle.Core.Internal;
using NLog;

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
            //Configure logger ASAP so that if the app crashes latter we can have logs of the crash
            ConfigureLogger();
            
            services.AddMvc().AddSessionStateTempDataProvider();
            //Add session support
            services.AddSession();
            // Allow the use of the MySQL Database as a service in this project.
            // Uses connection string from the project configuration
            // Passing it as a service ensures everything in the project will use this query service and use this connection string
            DatabaseQueryService dbQueryService = new DatabaseQueryService(Configuration.GetConnectionString("DefaultConnection"));
            EmailService emailService = new EmailService(Configuration.GetSection("EmailServiceConfiguration")["SourceEmailAddress"], 
                Configuration.GetSection("EmailServiceConfiguration")["SourceEmailPassword"]);
            bool alertMonitoringEnabled = true;
            if (alertMonitoringEnabled)
            {
                Thread alertMonitoringThread = new Thread(delegate()
                {
                    AlertMonitoringService alertMonitoringService = new AlertMonitoringService(dbQueryService, emailService);
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
            AbstractAPIKeyService apiKeyService = new APIKeyService(dbQueryService);
            AbstractUserService userService = new UserService(dbQueryService,notificationService,Configuration.GetSection("WebServiceConfiguration")["Hostname"],emailService);

            services.Add(new ServiceDescriptor(typeof(AbstractAuthenticationService), authenticationService));
            services.Add(new ServiceDescriptor(typeof(AbstractCameraService), cameraService));
            services.Add(new ServiceDescriptor(typeof(AbstractDataMessageService), dataMessageService));
            services.Add(new ServiceDescriptor(typeof(AbstractLocationService), locationService));
            services.Add(new ServiceDescriptor(typeof(AbstractGraphStatisticService), graphStatisticService));
            services.Add(new ServiceDescriptor(typeof(AbstractAlertService), alertService));
            services.Add(new ServiceDescriptor(typeof(AbstractNotificationService), notificationService));
            services.Add(new ServiceDescriptor(typeof(AbstractUserService), userService));
            services.Add(new ServiceDescriptor(typeof(AbstractAPIKeyService), apiKeyService));
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

        private void ConfigureLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            LogManager.ThrowExceptions = true;
            
            string fileName = TryGetLogPath();
            if (fileName.IsNullOrEmpty())
            {
                fileName = "${basedir}/logs/jetson_server.log";
            }
            
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = fileName };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            
            LogManager.Configuration = config;
            //dot net core logs are available at /var/log/dotnet.out.log
            LogManager.GetLogger("Startup").Info($"Trying to configure logger to log at {fileName} ...");
            LogManager.ThrowExceptions = false;
        }

        private string TryGetLogPath()
        {
            IConfigurationSection section = Configuration.GetSection("LoggerConfiguration");
            if (section != null)
            {
                return section["FileName"];
            }

            return null;
        }
    }
}
