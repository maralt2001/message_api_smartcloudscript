
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using backend_api.Model;

namespace backend_api.Data
{
    public class AirportsRepo : IAirportsRepo
    {
        private static List<Airport> airports = new List<Airport>();
        public static bool isProduction = false;

        public AirportsRepo()
        {
            InitAirports();
        }
        public Airport getAirport(int id)
        {

            return airports.Find(i => i.Id == id);
        }

        public IEnumerable getAirports()
        {
            
            return airports;

        }

        private static void InitAirports()
        {
            var path = string.Empty;

            if(!isProduction) {
                path = Path.GetFullPath(@".\Data\Static\AirportsGermany.CSV");
            }
            else
            {
                path = "/data/staticfiles/AirportsGermany.CSV";
            }
            
            int index = 1;
            using (var rd = new StreamReader(path))
            {
                while (!rd.EndOfStream)
                {
                    var splits = rd.ReadLine().Split(';');
                    var airport = new Airport{Id=index, Country=splits[0], City=splits[1], Icao=splits[2]};
                    index++;
                    airports.Add(airport);
                }
            }
        }

    }
}