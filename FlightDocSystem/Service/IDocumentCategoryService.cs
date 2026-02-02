using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IDocumentCategoryService
    {
        List<DocumentCategoryDto> GetAll();
        void Create(CreateDocumentCategoryDto dto);
        void Disable(Guid id);
    }
}
