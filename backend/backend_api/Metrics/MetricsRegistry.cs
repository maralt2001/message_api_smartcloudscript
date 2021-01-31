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
