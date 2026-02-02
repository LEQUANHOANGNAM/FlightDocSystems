using FlightDocSystem.Service;
using FlightDocSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightDocumentsController : ControllerBase
    {
        private readonly IFlightDocumentService _service;

        public FlightDocumentsController(IFlightDocumentService service)
        {
            _service = service;
        }

        //[Authorize(Policy = "DOC_VIEW")]
        [HttpGet]
        public IActionResult Get(Guid flightId)
        {
            return Ok(_service.GetByFlight(flightId));
        }

        //[Authorize(Policy = "DOC_UPLOAD")]
        [HttpPost]
        public IActionResult Upload(Guid flightId, [FromForm] UploadDocument dto)
        {
            _service.Upload(flightId, dto);
            return Ok();
        }

        //[Authorize(Policy = "DOC_VIEW")]
        [HttpGet("{documentId}/download")]
        public IActionResult Download(Guid documentId)
        {
            var zip = _service.DownloadZip(documentId);
            return File(zip, "application/zip", "documents.zip");
        }

        [HttpGet("{flightId}/files")]
        public IActionResult GetFiles(Guid flightId)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);

            var files = _service
                .GetFilesForUser(flightId, userId);

            return Ok(files);
        }

    }
}
