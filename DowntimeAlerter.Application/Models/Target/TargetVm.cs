using System;

namespace DowntimeAlerter.Application.Models.Target
{
    public class TargetVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int MonitoringIntervalInMinutes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate{ get; set; }
        public DateTime LastUpdateDate { get; set; }

    }
}
