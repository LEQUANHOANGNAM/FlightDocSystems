namespace FlightDocSystem.ViewModels
{
    public class DocumentCategoryDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
