using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DowntimeAlerter.Domain.Enums
{
    public enum HealthCheckResultEnum
    {
        Error = -1,
        UnSuccess = 0,
        Success = 1,
        NotFound=2
    }
}
