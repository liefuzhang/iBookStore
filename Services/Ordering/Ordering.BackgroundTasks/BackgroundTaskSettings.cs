using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.BackgroundTasks
{
    public class BackgroundTaskSettings
    {
        public string ConnectionString { get; set; }

        public int GracePeriodTimeInSecond { get; set; }

        public int CheckUpdateTimeInSecond { get; set; }

        public string OrderUrl { get; set; }
    }
}
