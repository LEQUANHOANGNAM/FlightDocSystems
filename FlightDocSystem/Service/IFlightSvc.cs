using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IFlightSvc
    {
        string AddFlight(FlightRequest flight);

        Task<List<FlightResponse>> GetAllFlightsAsync();

        Task<FlightResponse> GetFlightByIdAsync(string flightCode);
        Task<List<FlightResponse>> GetFlightByDateAsync(DateTime flightDate);

        string DeleteFlight(string flightNumber);
        string UpdateFlight(FlightRequest flight);
    }
}
