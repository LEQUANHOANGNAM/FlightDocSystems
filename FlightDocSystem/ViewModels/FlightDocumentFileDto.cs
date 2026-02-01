namespace FlightDocSystem.ViewModels
{
    public class FlightDocumentFileDto
    {
        public Guid DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string CategoryName { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
