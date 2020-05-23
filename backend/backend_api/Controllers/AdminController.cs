using Microsoft.AspNetCore.Mvc;
using backend_api.Data;
using backend_api.Model;
using Microsoft.AspNetCore.Hosting;
using System;
using backend_api.Database;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Geolocation;

namespace backend_api.Controllers
{
    public class AdminController: ControllerBase
    {
        private readonly IDBContext _db;
        public static bool isProduction = true;

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
                    using (var rd = new StreamReader(path))
                    while (!rd.EndOfStream)
                    {
                        var splits = rd.ReadLine().Split(';');
                        var airport = new Airport {
                        Id = index,
                        Icao = splits[0],
                        Type = splits[1],
                        Name = splits[2],
                        GeoPosition = new GeoPosition(splits[3],splits[4]),
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
                var x = await _db.BulkInsert<Airport>(airports,"airports");
                return Ok(new {state = x});
            }
            else
            {
                return Ok(new {state = "file not found"});
            }
            


            
        }

    }
  
        
       
}