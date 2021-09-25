using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DowntimeAlerter.Application.Interfaces
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(string to, string subject, string html);
    }
}
