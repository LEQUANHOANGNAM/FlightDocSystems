using FlightDocSystem.Data;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;

namespace FlightDocSystem.Service
{
    public class FlightSvc : IFlightSvc
    {
        private AppDbContext _context;
        private readonly IMemoryCache memoryCache;
        public FlightSvc(AppDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            this.memoryCache = memoryCache;
        }
        public string AddFlight(FlightRequest flight)
        {
            Flights newFlight = new Flights()
            {
                Id = Guid.NewGuid(),
                FlightNumber = flight.FlightNumber,
                FlightDate = flight.FlightDate,
            };
            _context.Flights.Add(newFlight);
            _context.SaveChanges();
            return "added";
        }

        public async Task<List<FlightResponse>> GetAllFlightsAsync()
        {
            const string cacheKey = "Flight:All";

            return await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);

                return await _context.Flights
                    .AsNoTracking()
                    .Select(f => new FlightResponse
                    {
                        FlightNumber = f.FlightNumber,
                        FlightDate = f.FlightDate
                    })
                    .ToListAsync();
            });
        }


        public async Task<List<FlightResponse>> GetFlightByDateAsync(DateTime flightDate)
        {
            var startUtc = DateTime.SpecifyKind(flightDate.Date, DateTimeKind.Utc);
            var endUtc = startUtc.AddDays(1);
            var flights = await _context.Flights
                .Where(flight => flight.FlightDate >= startUtc && flight.FlightDate < endUtc)
                .Select(f => new FlightResponse
                {
                    FlightNumber = f.FlightNumber,
                    FlightDate = f.FlightDate
                }).ToListAsync();
            return flights;
        }

        public async Task<FlightResponse?> GetFlightByIdAsync(string flightCode)
        {
            if (string.IsNullOrWhiteSpace(flightCode))
                return null;

            var cacheKey = $"Flight:{flightCode}";

            return await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);

                var flight = await _context.Flights
                    .AsNoTracking()
                    .Where(f => f.FlightNumber == flightCode)
                    .Select(f => new FlightResponse
                    {
                        FlightNumber = f.FlightNumber,
                        FlightDate = f.FlightDate
                    })
                    .FirstOrDefaultAsync();

                // Optional: nếu null thì cache ngắn hơn
                if (flight == null)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);

                return flight;
            });
        }

        public string DeleteFlight(string flightNumber)
        {
            _context.Flights.RemoveRange(_context.Flights.Where(f => f.FlightNumber == flightNumber));
            _context.SaveChanges();
            return "deleted";
        }

        public string UpdateFlight(FlightRequest flight)
        {
            
            var existingFlight = _context.Flights.FirstOrDefault(f => f.FlightNumber == flight.FlightNumber);
            if (existingFlight != null)
            {
                existingFlight.FlightDate = flight.FlightDate;
                _context.SaveChanges();
                return "updated";
            }
            return "flight not found";
        }
    }
}
