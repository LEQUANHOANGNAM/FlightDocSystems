using FlightDocSystem.Models;

namespace FlightDocSystem.Models
{
    public class Flights
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; }
        public DateTime FlightDate { get; set; }

        public ICollection<FlightDocuments> Documents { get; set; }
        public ICollection<FlightAssigment> FlightAssigments { get; set; }
    }
}
