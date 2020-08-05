
using Microsoft.AspNetCore.Mvc;
using backend_api.Model;
using Microsoft.AspNetCore.Hosting;
using System;
using backend_api.Database;
using System.Threading.Tasks;
using App.Metrics;
using backend_api.Metrics;

namespace backend_api.Controllers
{
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IDBContext _db;
        private readonly IMetrics _metrics;

        public AirportController(IDBContext db, IMetrics metrics)
        {
            _db = db;
            _metrics = metrics;
        }

        [HttpGet]
        [Route("/api/airport")]
        public async Task<IActionResult> GetAirportQuery([FromQuery(Name="icao")]string fieldValue)
        {
            
            if(fieldValue != string.Empty)
            {
                Airport airport = await _db.LoadRecordAsync<Airport>("airports","icao",fieldValue);
                _metrics.Measure.Counter.Increment(MetricsRegistry.AirportRequest);
                return Ok(airport);
            }
            else
            {
                _metrics.Measure.Counter.Increment(MetricsRegistry.AirportBadRequest);
                return BadRequest();
            }
        }

        
    }
}