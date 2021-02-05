
using Microsoft.AspNetCore.Mvc;
using backend_api.Model;
using Microsoft.AspNetCore.Hosting;
using System;
using backend_api.Database;
using System.Threading.Tasks;
using App.Metrics;
using backend_api.MetricsDefinition;

namespace backend_api.Controllers
{
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IDBContext _db;
        

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
                Airport airport = await _db.LoadRecordAsync<Airport>("airports","icao",fieldValue);
                MetricsRegistry.BackendAirportRequestSuccess.Inc();
                return Ok(airport);
            }
            else
            {
                MetricsRegistry.BackendAirportRequestFailed.Inc();
                return BadRequest();
            }
        }

        
    }
}