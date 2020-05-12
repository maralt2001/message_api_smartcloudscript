
using System.Collections;
using backend_api.Model;

namespace backend_api.Data
{
    public interface IAirportsRepo
    {
        Airport getAirport(int id);

        Airport getAirportByIcao(string icao);
        
        IEnumerable getAirports();
      
    }
}