namespace FlightDocSystem.ViewModels
{
    public class UploadDocument
    {
        public string Title { get; set; }
        public Guid CategoryId { get; set; }

        public List<IFormFile> Files { get; set; }
    }
}
