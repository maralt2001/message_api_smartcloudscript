
using Microsoft.AspNetCore.Mvc;
using backend_api.Model;
using Microsoft.AspNetCore.Hosting;
using System;
using backend_api.Database;
using System.Threading.Tasks;
using App.Metrics;
using backend_api.MetricsDefinition;
using System.Diagnostics;
using System.Collections.Generic;

namespace backend_api.Controllers
{
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IDBContext _db;
        public Stopwatch _stopwatch = new Stopwatch();


        public AirportController(IDBContext db)
        {
            _db = db;
            
        }

        [HttpGet]
        [Route("/api/airport")]
        public async Task<IActionResult> GetAirportQuery([FromQuery(Name="icao")]string fieldValue)
        {
             
            if(fieldValue != string.Empty)
            {
                _stopwatch.Start();
                Airport airport = await _db.LoadRecordAsync<Airport>("airports","icao",fieldValue);
                MetricsRegistry.BackendAirportRequestSuccess.Inc();
                _stopwatch.Stop();
                MetricsRegistry.BackendDurationGetAirportSummary.Observe(_stopwatch.Elapsed.TotalMilliseconds);

                return Ok(airport);
            }
            else
            {
                MetricsRegistry.BackendAirportRequestFailed.Inc();
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/api/airports")]
        public async Task<IActionResult> GetAirports()
        {
            var result = await _db.LoadRecordsAsync<Airport>("airports");
            return Ok(result);
        }

        
    }
}