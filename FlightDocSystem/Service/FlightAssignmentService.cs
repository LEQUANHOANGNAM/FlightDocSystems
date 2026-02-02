using FlightDocSystem.Data;
using FlightDocSystem.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace FlightDocSystem.Service
{
    public class FlightAssignmentService:IFlightAssignmentService
    {
        private readonly AppDbContext _context;

        public FlightAssignmentService(AppDbContext context)
        {
            _context = context;
        }

        // =====PHÂN CÔNG USER → FLIGHT =====
        public void AssignUserToFlight(Guid flightId, int userId)
        {
            //  Kiểm tra flight tồn tại
            var flight = _context.Flights
                .FirstOrDefault(f => f.Id == flightId);

            if (flight == null)
                throw new ArgumentException("Flight not found");

            // Kiểm tra user tồn tại
            var userExists = _context.Users.Any(u => u.Id == userId);
            if (!userExists)
                throw new ArgumentException("User not found");

            // Check đã được phân công flight này chưa
            var alreadyAssigned = _context.FlightAssigments
                .Any(a => a.FlightId == flightId && a.UserId == userId);

            if (alreadyAssigned)
                return;

            // KIỂM TRA ĐIỀU KIỆN 4 TIẾNG
            var assignedFlights = _context.FlightAssigments
                .Where(a => a.UserId == userId)
                .Select(a => a.Flight)
                .ToList();

            foreach (var assignedFlight in assignedFlights)
            {
                var diffHours = Math.Abs(
                    (flight.FlightDate - assignedFlight.FlightDate).TotalHours);

                if (diffHours < 4)
                {
                    throw new InvalidOperationException(
                        "User already assigned to another flight within 4 hours");
                }
            }

            // 5. Phân công
            var assignment = new FlightAssigment
            {
                FlightId = flightId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow
            };

            _context.FlightAssigments.Add(assignment);
            _context.SaveChanges();
        }

        // ===== HỦY PHÂN CÔNG =====
        public void UnassignUserFromFlight(Guid flightId, int userId)
        {
            var assignment = _context.FlightAssigments
                .FirstOrDefault(a => a.FlightId == flightId && a.UserId == userId);

            if (assignment == null)
                return;

            _context.FlightAssigments.Remove(assignment);
            _context.SaveChanges();
        }

        // ===== KIỂM TRA USER CÓ THUỘC FLIGHT KHÔNG =====
        public bool IsUserAssigned(Guid flightId, int userId)
        {
            return _context.FlightAssigments
                .Any(a => a.FlightId == flightId && a.UserId == userId);
        }

        // ===== LẤY DANH SÁCH USER CỦA 1 FLIGHT =====
        public List<User> GetUsersByFlight(Guid flightId)
        {
            return _context.FlightAssigments
                .Where(a => a.FlightId == flightId)
                .Select(a => a.User)
                .ToList();
        }

        // ===== LẤY DANH SÁCH FLIGHT CỦA 1 USER =====
        public List<Flights> GetFlightsByUser(int userId)
        {
            return _context.FlightAssigments.Where(x => x.UserId == userId).Select(x => x.Flight).ToList();

        }


public async Task ImportFromExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("File is empty");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(1);

        foreach (var row in sheet.RowsUsed().Skip(1)) // bỏ header
        {
            int rowNumber = row.RowNumber();

            // ---- Read dữ liệu an toàn ----
            if (!int.TryParse(row.Cell(1).GetString(), out int userId))
                continue;

            if (!Guid.TryParse(row.Cell(2).GetString(), out Guid flightId))
                continue;

            DateTime assignedAt = row.Cell(3).IsEmpty()
                ? DateTime.UtcNow
                : row.Cell(3).GetDateTime();

            // ---- Validate ----
            if (!_context.Users.Any(u => u.Id == userId))
                continue;

            if (!_context.Flights.Any(f => f.Id == flightId))
                continue;

            bool exists = _context.FlightAssigments.Any(a =>
                a.UserId == userId && a.FlightId == flightId);

            if (exists)
                continue;

            // ---- Add ----
            _context.FlightAssigments.Add(new FlightAssigment
            {
                UserId = userId,
                FlightId = flightId,
                AssignedAt = assignedAt
            });
        }

        await _context.SaveChangesAsync();
    }

}
}
