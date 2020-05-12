
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public Airport getAirportByIcao(string ic)
        {
            string icao = ic.ToUpper().Trim();
            return airports.Find(f => f.Icao == icao);
            
        }

        public IEnumerable getAirports()
        {
            
            return airports;

        }

        private static async void InitAirports()
        {
            var path = string.Empty;

            switch (isProduction)
            {
                case true:
                    path= "/data/staticfiles/AirportsGermany.CSV";
                    break;
                //need in development and single container
                case false:
                    path= Path.GetFullPath(@".\Data\Static\AirportsGermany.CSV");
                    break;
            }
           
            
            var result = Task.Run(() =>
            {
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
            });
            await result;
            
        }

    }
}