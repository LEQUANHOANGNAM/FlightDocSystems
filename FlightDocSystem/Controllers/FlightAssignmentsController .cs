using FlightDocSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightAssignmentsController : ControllerBase
    {
        private readonly IFlightAssignmentService _flightAssignmentService;

        public FlightAssignmentsController(
            IFlightAssignmentService flightAssignmentService)
        {
            _flightAssignmentService = flightAssignmentService;
        }

        // PHÂN CÔNG USER → FLIGHT
    
        [HttpPost("assign")]
        public IActionResult AssignUserToFlight(
            [FromQuery] Guid flightId,
            [FromQuery] int userId)
        {
            _flightAssignmentService.AssignUserToFlight(flightId, userId);
            return Ok(new { message = "User assigned to flight successfully" });
        }

        //  HỦY PHÂN CÔNG
        
        [HttpDelete("unassign")]
        public IActionResult UnassignUserFromFlight(
            [FromQuery] Guid flightId,
            [FromQuery] int userId)
        {
            _flightAssignmentService.UnassignUserFromFlight(flightId, userId);
            return Ok(new { message = "User unassigned from flight successfully" });
        }

        // KIỂM TRA USER CÓ THUỘC FLIGHT KHÔNG
   
        [HttpGet("is-assigned")]
        public IActionResult IsUserAssigned(
            [FromQuery] Guid flightId,
            [FromQuery] int userId)
        {
            var result = _flightAssignmentService
                .IsUserAssigned(flightId, userId);

            return Ok(new { assigned = result });
        }

        // LẤY USER CỦA 1 FLIGHT

        [HttpGet("flight/{flightId}/users")]
        public IActionResult GetUsersByFlight(Guid flightId)
        {
            var users = _flightAssignmentService.GetUsersByFlight(flightId);
            return Ok(users);
        }

        // LẤY FLIGHT CỦA 1 USER
       
        [HttpGet("user/{userId}/flights")]
        public IActionResult GetFlightsByUser(int userId)
        {
            var flights = _flightAssignmentService.GetFlightsByUser(userId);
            return Ok(flights);
        }
        [Authorize]
        [HttpGet("me/flights")]
        public IActionResult GetMyFlights()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            return Ok(_flightAssignmentService.GetFlightsByUser(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        public IActionResult ImportAssignments(IFormFile file)
        {
            _flightAssignmentService.ImportFromExcel(file);
            return Ok("Import success");
        }
    }
}
