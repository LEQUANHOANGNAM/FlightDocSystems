using FlightDocSystem.Models;

namespace FlightDocSystem.Service
{
    public interface IFlightAssignmentService
    {
        void AssignUserToFlight(Guid flightId, int userId);
        void UnassignUserFromFlight(Guid flightId, int userId);

        bool IsUserAssigned(Guid flightId, int userId);

        List<User> GetUsersByFlight(Guid flightId);
        List<Flights> GetFlightsByUser(int userId);
        Task ImportFromExcel(IFormFile file);
    }
}
