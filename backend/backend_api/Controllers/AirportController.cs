
using Microsoft.AspNetCore.Mvc;
using backend_api.Data;
using backend_api.Model;
using Microsoft.AspNetCore.Hosting;
using System;
using backend_api.Database;
using System.Threading.Tasks;

namespace backend_api.Controllers
{
    public class AirportController : ControllerBase
    {
        private readonly IAirportsRepo _repo;
        private readonly IDBContext _db;
        
        public AirportController(IAirportsRepo repo, IDBContext db)
        {
            _repo = repo;
            _db = db;
            
        }

        [HttpGet]
        [Route("/api/airport/{id}")]
        public IActionResult GetAirportById(int id)
        {
            Airport airport =_repo.getAirport(id);
            return Ok(airport);
        }

        [HttpGet]
        [Route("/api/airports")]
        public IActionResult GetAirports()
        {
            var airports = _repo.getAirports();
            return Ok(airports);
            
        }

        [HttpGet]
        [Route("/api/airport")]

        public IActionResult GetAirportByIcao([FromQuery]string icao)
        {
            
            if(icao != string.Empty)
            {
                var airport = _repo.getAirportByIcao(icao);
                return Ok(airport);
            }
            else
            {
                return NotFound();
            }
        }

        
    }
}