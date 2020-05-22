
using System.Collections;
using backend_api.Model;
using System.Collections.Generic;

namespace backend_api.Data
{
    public interface IAirportsRepo
    {
        Airport getAirport(int id);

        Airport getAirportByIcao(string icao);
        
        List<Airport> getAirports();
      
    }
}