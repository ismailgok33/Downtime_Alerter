using DowntimeAlerter.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace DowntimeAlerter.Application.Interfaces
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResultEnum> RunHealthCheck(Guid targetId, string userMail);        
    }
}
