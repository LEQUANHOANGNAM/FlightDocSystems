namespace FlightDocSystem.Models
{
    public class DocumentCategory
    {
        public Guid Id { get; set; }

        public string Code { get; set; }    
        public string Name { get; set; }     
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public ICollection<FlightDocuments> FlightDocuments { get; set; }
    }
}