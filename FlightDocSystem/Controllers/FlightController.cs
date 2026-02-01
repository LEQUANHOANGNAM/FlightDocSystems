    using FlightDocSystem.Service;
    using FlightDocSystem.ViewModels;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    namespace FlightDocSystem.Controllers
    {
        [Route("api/flights")]
        [ApiController]
        public class FlightController : ControllerBase
        {
            private IFlightSvc _flightSvc;
            public FlightController(IFlightSvc flightSvc)
            {
                _flightSvc = flightSvc;
            }
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
            [HttpPost()]
            public IActionResult AddFlight([FromBody] ViewModels.FlightRequest flight)
            {
                var result = _flightSvc.AddFlight(flight);
                return Ok(result);
            }
            [HttpGet("all")]
            public async Task<IActionResult> GetAllFlights()
            {
                var result = await _flightSvc.GetAllFlightsAsync();
                return Ok(result);
            }
            [HttpGet()]
            public async Task<IActionResult> GetFlightByDate([FromQuery] DateTime date)
            {
                var result = await _flightSvc.GetFlightByDateAsync(date);
                return Ok(result);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetFlightByNumber([FromRoute] string id)
            {
                var result = await _flightSvc.GetFlightByIdAsync(id);
                return Ok(result);
            }
            [HttpDelete("{id}")]
            public IActionResult DeleteFlight([FromRoute] string id)
            {
                var result = _flightSvc.DeleteFlight(id);
                return Ok(result);
            }
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Staff")]
            [HttpPut("UpdateFlight")]
            public IActionResult UpdateFlight([FromBody] FlightRequest flight)
            {
                var result = _flightSvc.UpdateFlight(flight);
                return Ok(result);
            }
        }
    }
