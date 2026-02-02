using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IDocumentTemplateService
    {
        List<DocumentTemplateResponse> GetAll();
        DocumentTemplateResponse GetById(Guid id);
        void Create(DocumentTemplateDto dto);
        void Update(Guid id, DocumentTemplateDto dto);
        void Delete(Guid id);
    }
}
