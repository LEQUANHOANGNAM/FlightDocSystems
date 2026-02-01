using FlightDocSystem.Data;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public class DocumentTemplateService:IDocumentTemplateService
    {
        private readonly AppDbContext _context;

        public DocumentTemplateService(AppDbContext context)
        {
            _context = context;
        }

        public List<DocumentTemplateResponse> GetAll()
        {
            return _context.DocumentTemplates
                .Where(t => t.IsActive)
                .Select(t => new DocumentTemplateResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    Category = t.Category,
                    FileUrl = t.FileUrl
                })
                .ToList();
        }

        public DocumentTemplateResponse GetById(Guid id)
        {
            var template = _context.DocumentTemplates
                .FirstOrDefault(t => t.Id == id && t.IsActive);

            if (template == null)
                throw new KeyNotFoundException("Template not found");

            return new DocumentTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Category = template.Category,
                FileUrl = template.FileUrl
            };
        }

        public void Create(DocumentTemplateDto dto)
        {
            var template = new DocumentTemplate
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Category = dto.Category,
                FileUrl = dto.FileUrl,
                IsActive = true
            };

            _context.DocumentTemplates.Add(template);
            _context.SaveChanges();
        }

        public void Update(Guid id, DocumentTemplateDto dto)
        {
            var template = _context.DocumentTemplates.Find(id);
            if (template == null || !template.IsActive)
                throw new KeyNotFoundException("Template not found");

            template.Name = dto.Name;
            template.Category = dto.Category;
            template.FileUrl = dto.FileUrl;

            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var template = _context.DocumentTemplates.Find(id);
            if (template == null)
                return;

            template.IsActive = false; // soft delete
            _context.SaveChanges();
        }
    }
}
