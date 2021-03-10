
using Microsoft.AspNetCore.Mvc;
using backend_api.Model;
using Microsoft.AspNetCore.Hosting;
using System;
using backend_api.Database;
using System.Threading.Tasks;
using backend_api.MetricsDefinition;
using System.Diagnostics;
using KeyVault;

namespace backend_api.Controllers
{
    [ApiController]
    public class AirportController : ControllerBase
    {
        
        private readonly IVaultProvider _vaultprovider;
        private readonly IDBContextService _dbContextService;
        public static string hostEnvironment;
        public Stopwatch _stopwatch = new Stopwatch();


        public AirportController(IVaultProvider vaultProvider, IDBContextService dbContextService)
        {
            _vaultprovider = vaultProvider;
            _dbContextService = dbContextService;
            
        }

        [HttpGet]
        [Route("/api/airport")]
        public async Task<IActionResult> GetAirportQuery([FromQuery(Name="icao")]string fieldValue)
        {
            var _db = _dbContextService.GetDBContext(hostEnvironment, _vaultprovider);


            if (fieldValue != string.Empty)
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
            var _db = _dbContextService.GetDBContext(hostEnvironment, _vaultprovider);
            var result = await _db.LoadRecordsAsync<Airport>("airports");
            return Ok(result);
        }

        
    }
}