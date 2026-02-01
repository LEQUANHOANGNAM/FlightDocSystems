using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IFlightDocumentService
    {
        List<FlightDocument> GetByFlight(Guid flightId);
        void Upload(Guid flightId, UploadDocument dto);
        byte[] DownloadZip(Guid documentId);

        List<FlightDocumentFileDto> GetFilesForUser(Guid flightId, int userId);
    }
}
