using FlightDocSystem.Service;
using FlightDocSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentCategoriesController : ControllerBase
    {
        private readonly IDocumentCategoryService _service;

        public DocumentCategoriesController(IDocumentCategoryService service)
        {
            _service = service;
        }
        //[Authorize(Policy = "CATEGORY_VIEW")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        //[Authorize(Policy = "CATEGORY_MANAGE")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateDocumentCategoryDto dto)
        {
            _service.Create(dto);
            return Ok(new { message = "Created successfully" });
        }
        //[Authorize(Policy = "CATEGORY_MANAGE")]
        [HttpPut("{id}/disable")]
        public IActionResult Disable(Guid id)
        {
            _service.Disable(id);
            return Ok(new { message = "Disabled successfully" });
        }
    }
}
