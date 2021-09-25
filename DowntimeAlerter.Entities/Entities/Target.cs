using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DowntimeAlerter.Domain.Entities
{
    public class Target: Base
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int MonitoringIntervalInMinutes { get; set; }
        public string NotificationAdress { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
