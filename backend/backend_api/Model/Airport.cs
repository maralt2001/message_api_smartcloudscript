using Geolocation;
namespace backend_api.Model
{
    public class Airport
    {
        public int Id { get; set; }
        public string Icao {get; set;}
        public string Type { get; set; }
        public string Name { get; set; }

        public Coordinate Coordinate {get; set;}

        public string Continent {get; set;}

        public string Country {get; set;}

        public string Region {get; set;}

        public string Info {get; set;}

        public string Iata {get; set;}
        
    }
}