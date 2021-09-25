using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DowntimeAlerter.Web.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HangfireAuthorizationFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public bool Authorize(DashboardContext context)
        {
            
            //var user = new HttpContextAccessor().HttpContext.User
            //var httpContext = context.GetHttpContext();
            var result= _httpContextAccessor.HttpContext.User.IsInRole("ADMINISTRATOR");
            //_httpContextAccessor.HttpContext.User.Identity.Name;
            return true;
            // Allow only admin authenticated users to see the Dashboard (potentially dangerous).
           // return httpContext.User.IsInRole("ADMINISTRATOR");
        }
    }
}
