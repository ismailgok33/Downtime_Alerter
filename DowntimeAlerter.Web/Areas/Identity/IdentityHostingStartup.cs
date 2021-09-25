using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(DowntimeAlerter.Web.Areas.Identity.IdentityHostingStartup))]
namespace DowntimeAlerter.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}