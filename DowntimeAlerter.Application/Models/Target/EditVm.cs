using System;

namespace DowntimeAlerter.Application.Models.Target
{
    public class EditVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int MonitoringIntervalInMinutes { get; set; }
    }
}
