using System.Collections.Generic;

namespace Ckm.Models
{
    public interface ICkmRepo
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetAllTripsWithStops();
        void AddTrip(Trip newTrip);
        bool SaveAll();
        //Trip GetTripByName(string tripName);
        Trip GetTripByName(string tripName, string userName);

        void AddStop(string tripName, string userName, Stop newStop);

        IEnumerable<Trip> GetUserTripsWithStops(string name);
    }
}
