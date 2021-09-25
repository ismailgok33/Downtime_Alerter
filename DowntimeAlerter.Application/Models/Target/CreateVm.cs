namespace DowntimeAlerter.Application.Models.Target
{
    public class CreateVm
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int MonitoringIntervalInMinutes { get; set; }
    }
}
