using DowntimeAlerter.Domain.Enums;
using System;

namespace DowntimeAlerter.Application.Models.Target
{
    public class CheckResultVm
    {
        public DateTime ExecutionTime { get; set; }
        public HealthCheckResultEnum Result { get; set; }
        public int StatusCode { get; set; }
    }
}
