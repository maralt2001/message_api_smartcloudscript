
using Microsoft.AspNetCore.Mvc;
using backend_api.Data;
using backend_api.Model;


namespace backend_api.Controllers
{
    public class AirportController : ControllerBase
    {
        private readonly IAirportsRepo _repo;
        
        public AirportController(IAirportsRepo repo)
        {
            _repo = repo;
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
    }
}