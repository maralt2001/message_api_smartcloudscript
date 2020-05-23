
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using backend_api.Model;
using Geolocation;

namespace backend_api.Data
{
    public class AirportsRepo : IAirportsRepo
    {
        private static List<Airport> airports = new List<Airport>();
        public static bool isProduction = false;

        public AirportsRepo()
        {
            InitAirportsFromCSV();
        }
        
        public Airport getAirport(int id)
        {

            return airports.Find(f => f.Id == id);
        }

        public Airport getAirportByIcao(string ic)
        {
            
            string icao = ic.ToUpper().Trim();
            return airports.Find(f => f.Icao == icao);
            
        }

        public List<Airport> getAirports()
        {
            
            return airports;            

        }

        public static async void InitAirportsFromCSV()
        {
            var path = string.Empty;

            switch (isProduction)
            {
                case true:
                    path= "/data/staticfiles/airportworld.csv";
                    break;
                //need in development and single container
                case false:
                    path= Path.GetFullPath(@".\Data\Static\airportworld.csv");
                    break;
            }
           
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
            
        }

       

    }
}