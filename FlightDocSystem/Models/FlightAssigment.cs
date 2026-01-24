using FlightDocSystem.Models;

namespace FlightDocSystem.Models
{
    public class FlightAssigment
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public Guid FlightId { get; set; }
        public Flights Flight { get; set; }

        public DateTime AssignedAt { get; set; }
    }

}
