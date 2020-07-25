using App.Metrics;
using App.Metrics.Counter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_api.Metrics
{
    public class MetricsRegistry
    {
        public static CounterOptions DBConnectionUp => new CounterOptions
        {
            Name = "RequestDBConnectionIsUp",
            Context = "Database",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions DBConnectionDown => new CounterOptions
        {
            Name = "RequestDBConnectionIsDown",
            Context = "Database",
            MeasurementUnit = Unit.Calls
        };
    }
}
