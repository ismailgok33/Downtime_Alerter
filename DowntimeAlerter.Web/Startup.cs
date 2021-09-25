using DowntimeAlerter.Application.Interfaces;
using DowntimeAlerter.Application.Services;
using DowntimeAlerter.EntityFrameworkCore.ApplicationIdentity;
using DowntimeAlerter.EntityFrameworkCore.TargetDb;
using DowntimeAlerter.Infrastructure.Notification;
using DowntimeAlerter.Web.Extensions;
using DowntimeAlerter.Web.Filters;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace DowntimeAlerter.Web
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
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DatabaseConnection")));

            services.AddHttpContextAccessor();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>();
         
            services.AddControllersWithViews();

            //services.AddScoped<HttpClient>();
            services.AddHttpClient();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddDbContext<TargetDbContext>(option =>
             option.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection"))
         );

            services.AddScoped<INotificationSender, NotificationSender>();

            services.Configure<NotificationSettings>(Configuration.GetSection("NotificationSettings"));

            services.AddTransient<ITargetService, TargetService>();
            services.AddTransient<IHealthCheckService, HealthCheckService>();

            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                PrepareSchemaIfNecessary = true
            }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddAutoMapper(typeof(Startup));

            //services.AddSingleton<Serilog.ILogger>
            //         (x => new LoggerConfiguration()
            //               .WriteTo.MSSqlServer(connectionString, "ApplicationLog")
            //               .CreateLogger());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseGlobalExceptionHandler(logger
                                    , errorPagePath: "/Home/Error"
                                    , respondWithJsonErrorDetails: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();


            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseHangfireDashboard(
                pathMatch:"/hangfire", 
                options:new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizationFilter(
                    app.ApplicationServices.GetService<IHttpContextAccessor>()
                    ) }
            });
            
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHangfireDashboard();
            });
        }


    }
}
