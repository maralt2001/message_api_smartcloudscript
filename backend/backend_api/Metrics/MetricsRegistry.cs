using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Formatters.Prometheus;
using Prometheus.HttpClientMetrics;
using Prometheus;
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

        public static Prometheus.Counter ProcessedJobCount = Prometheus.Metrics.CreateCounter("backend_request_operation_total", "total number processed in backend");

        public static Prometheus.Histogram LoginRequestHistogram = 
            Prometheus.Metrics.CreateHistogram(
                        "backend_loginRequest_duration_MilliSeconds",
                        "Histogram for the LoginDuration in ms in backend.",
                        new HistogramConfiguration
                        {
                            Buckets = Prometheus.Histogram.LinearBuckets(start: 2.0, width: 2, count: 5)
                        });

        public static CounterOptions LoginRequestSuccess => new CounterOptions
        {
            Name = "LoginRequestSuccess",
            Context = "Login",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions LoginRequestFailed => new CounterOptions
        {
            Name = "LoginRequestFailed",
            Context = "Login",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions RegisterRequestSuccess => new CounterOptions
        { 
            Name = "RegisterRequestSuccess",
            Context = "Register",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions RegisterRequestFailed => new CounterOptions
        {
            Name = "RegisterRequestFailed",
            Context = "Register",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions AirportRequest => new CounterOptions
        {
            Name = "AirportRequestSum",
            Context = "AirportController",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions AirportBadRequest => new CounterOptions
        {
            Name = "AirportBadRequestSum",
            Context = "AirportController",
            MeasurementUnit = Unit.Calls
        };
    }
}
