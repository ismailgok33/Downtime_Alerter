using DowntimeAlerter.Domain.Enums;
using System;

namespace DowntimeAlerter.Domain.Entities
{
    public class HealthCheckResult: Base
    {
        public Guid TargetId { get; set; }
        public DateTime ExecutionTime { get; set; }
        public HealthCheckResultEnum Result { get; set; }
        public int StatusCode { get; set; }
    }
}
