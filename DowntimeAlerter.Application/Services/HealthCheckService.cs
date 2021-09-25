using DowntimeAlerter.Application.Interfaces;
using DowntimeAlerter.EntityFrameworkCore.TargetDb;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DowntimeAlerter.Application.Services
{
    public class HealthCheckService:IHealthCheckService
    {
        private readonly TargetDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly INotificationSender _notificationSender; 

        public HealthCheckService(TargetDbContext context, IHttpClientFactory clientFactory , 
            INotificationSender notificationSender)
        {
            this._context = context;
            _clientFactory = clientFactory;
            _notificationSender = notificationSender;
        }

        public async Task<Domain.Enums.HealthCheckResultEnum> RunHealthCheck(Guid targetId, string userMail)
        {
            var target = await _context.Target.FindAsync(targetId);
            var check = new Domain.Entities.HealthCheckResult
            {
                TargetId = targetId,
                StatusCode = 0,
                ExecutionTime = DateTime.UtcNow,
                Result = target == null ? Domain.Enums.HealthCheckResultEnum.NotFound: Domain.Enums.HealthCheckResultEnum.Error
            };

            if (target != null)
            {
                using (var client = _clientFactory.CreateClient())
                {
                    using (var response = await client.GetAsync(target.Url))
                    {
                        check.StatusCode = (int)response.StatusCode;
                        check.ExecutionTime = DateTime.UtcNow;
                        check.Result = response.IsSuccessStatusCode ? Domain.Enums.HealthCheckResultEnum.Success: Domain.Enums.HealthCheckResultEnum.UnSuccess;
                        if (!response.IsSuccessStatusCode)
                        {
                            var subject = $"Downtime Alerter : {target.Name} service is down";
                            var message = $"<p>{target.Url} could not be reached at {check.ExecutionTime}</p>";
                            await _notificationSender.SendNotificationAsync(userMail, subject, message);
                        }
                    }
                }
            }
            await _context.HealthCheckResult.AddAsync(check);
            await _context.SaveChangesAsync();
            return check.Result;
        }
    }
}
