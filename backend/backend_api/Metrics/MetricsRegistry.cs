
using Prometheus;


namespace backend_api.MetricsDefinition
{
    public class MetricsRegistry
    {
        

        public static Counter BackendDBConnectionDown = Metrics.CreateCounter("backend_dbconnection_down", "total number dbconnection dwon");

        public static Counter BackendDBConnectionUp = Metrics.CreateCounter("backend_dbconnection_up", "total number dbconnection up");

        public static Counter ProcessedJobCount = Metrics.CreateCounter("backend_request_operation_total", "total number processed in backend");

        public static Histogram LoginRequestHistogram = 
                Metrics.CreateHistogram(
                        "backend_loginRequest_duration_MilliSeconds",
                        "Histogram for the LoginDuration in ms in backend.",
                        new HistogramConfiguration
                        {
                            Buckets = Histogram.LinearBuckets(start: 20, width: 10, count: 5)
                            
                        });

        public static Summary BackendDurationGetAirportSummary = Metrics.CreateSummary("backend_duration_get_ariprot", "Summary of duration get Airport in ms");

        public static Counter BackendLoginRequestSuccess = Metrics.CreateCounter("backend_loginRequest_success", "total number login request success");

        public static Counter BackendLoginRequestFailed = Metrics.CreateCounter("backend_loginRequest_failed", "total number login request failed");

        public static Counter BackendRegisterRequestSuccess = Metrics.CreateCounter("backend_registerRequest_success", "total number register request success");

        public static Counter BackendRegisterRequestFailed = Metrics.CreateCounter("backend_registerRequest_failed", "total number register request failed");


        public static Counter BackendAirportRequestSuccess = Metrics.CreateCounter("backend_airportRequest_success", "total number airport request success");
        public static Counter BackendAirportRequestFailed = Metrics.CreateCounter("backend_airportRequest_failed", "total number airport request failed");


        
    }
}
