using System;
using System.ComponentModel;
using Geolocation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend_api.Model
{
    public class Airport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [ReadOnly(true)]
        public string _id { get; set; }
        [BsonElement("id")]
        public int Id { get; set; }
        [BsonElement("icao")]
        public string Icao {get; set;}
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("geoposition")]
        public GeoPosition GeoPosition {get; set;}
        [BsonElement("continent")]
        public string Continent {get; set;}
        [BsonElement("country")]
        public string Country {get; set;}
        [BsonElement("region")]
        public string Region {get; set;}

        [BsonElement("info")]
        public string Info {get; set;}

        [BsonElement("iata")]
        public string Iata {get; set;}

        
        
    }

    public class GeoPosition
    {
        public string latitude {get; set;}
        public string longitude {get; set;}
        

        public GeoPosition(string latitude, string longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        internal void Deconstruct(out string longitude, out string latitude)
        {
            latitude = this.latitude;
            longitude = this.longitude;

        }

        public Coordinate ConvertToCoordinate()
        {
            var coordinate = new Coordinate();
            coordinate.Latitude = Convert.ToDouble(this.latitude);
            coordinate.Longitude = Convert.ToDouble(this.latitude);
            return coordinate;
        }


    }

    
}