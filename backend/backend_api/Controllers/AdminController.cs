using Microsoft.AspNetCore.Mvc;
using backend_api.Model;
using backend_api.Database;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System;

namespace backend_api.Controllers
{
    public class AdminController: ControllerBase
    {
        private readonly IDBContext _db;
        public static bool isProduction = false;

        public AdminController(IDBContext db)
        {
            _db = db;
            
        }

        [HttpGet]
        [Route("/api/admin/dbstatus")]
        public async Task<IActionResult> GetDBStatus()
        {
           var result = await _db.IsConnectionUp();
           return result ? Ok(new {state = "connection ist up"}) : Ok(new {state = "connection is down"});

           
        }

        [HttpGet]
        [Route("/api/admin/job/bulkinsert")]
        public async Task<IActionResult> InsertMany([FromQuery] string filename)
        {
            List<Airport> airports = new List<Airport>();
            var path = string.Empty;
            
            switch (isProduction)
            {
                case true:
                    path= "/data/staticfiles/"+ filename;
                    break;
                //need in development and single container
                case false:
                    path= Path.GetFullPath(@".\Data\Static\"+ filename);
                    break;
            }
            if(filename != string.Empty && System.IO.File.Exists(path) && Path.GetExtension(path)== ".csv")
            {
                
                int index = 1;
                var result = Task.Run(() => 
                {
                    using var rd = new StreamReader(path);
                    while (!rd.EndOfStream)
                    {
                        string[] splits = rd.ReadLine().Split(';');
                        Airport airport = new Airport
                        {
                            Id = index,
                            Icao = splits[0],
                            Type = splits[1],
                            Name = splits[2],
                            GeoPosition = new GeoPosition(splits[3], splits[4]),
                            Continent = splits[5],
                            Country = splits[6],
                            Region = splits[7],
                            Info = splits[8],
                            Iata = splits[9]
                        };
                        index++;
                        airports.Add(airport);

                    }

                });
                await result;
                int dbinsert = await _db.BulkInsert<Airport>(airports,"airports");
                return Ok(new {insertCount = dbinsert});
            }
            else
            {
                return BadRequest(new {state = "file not found"});
            }
            


            
        }

        [HttpGet]
        [Route("/api/admin/job/airports/createindex")]
        public async Task<IActionResult> CreateAirportIndex([FromQuery(Name ="key")]string key)
        {
            var result = await _db.CreateIndex<Airport>("airports", key);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/admin/job/airports/dropindex")]

        public async Task<IActionResult> DropAirportIndex([FromQuery(Name = "index")] string indexname)
        {
            var result = await _db.DropIndex<Airport>("airports", indexname);
            return Ok(new { state = result });
        }

    }
  
        
       
}