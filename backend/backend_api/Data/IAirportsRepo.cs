
using System.Collections;
using backend_api.Model;

namespace backend_api.Data
{
    public interface IAirportsRepo
    {
        Airport getAirport(int id);
        
        IEnumerable getAirports();
      
    }
}