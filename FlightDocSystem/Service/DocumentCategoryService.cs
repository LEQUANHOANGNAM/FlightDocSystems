using FlightDocSystem.Data;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FlightDocSystem.Service
{
    public class DocumentCategoryService : IDocumentCategoryService
    {
        private readonly AppDbContext _context;
        public DocumentCategoryService(AppDbContext context)
        {
            _context = context;
        }

        public List<DocumentCategoryDto> GetAll()
        {
            return _context.DocumentCategories
                .Where(c => c.IsActive)
                .Select(c => new DocumentCategoryDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToList();
        }

        public void Create(CreateDocumentCategoryDto dto)
        {
            var category = new DocumentCategory
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };

            _context.DocumentCategories.Add(category);
            _context.SaveChanges();
        }

        public void Disable(Guid id)
        {
            var category = _context.DocumentCategories.Find(id);
            if (category == null) return;

            category.IsActive = false;
            _context.SaveChanges();
        }
    }
}

