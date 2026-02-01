namespace FlightDocSystem.Models
{
    public class RevokedToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = null!;

        public DateTime ExpiredAt { get; set; }

        public DateTime RevokedAt { get; set; }
    }
}