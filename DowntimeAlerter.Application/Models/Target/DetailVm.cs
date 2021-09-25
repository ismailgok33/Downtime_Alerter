using System.Collections.Generic;

namespace DowntimeAlerter.Application.Models.Target
{
    public class DetailVm
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int MonitoringIntervalInMinutes { get; set; }
        public IList<CheckResultVm> CheckResults { get; set; }
    }
}
