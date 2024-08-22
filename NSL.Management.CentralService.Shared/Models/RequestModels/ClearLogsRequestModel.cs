namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class ClearLogsRequestModel
    {
        public Guid ServerId { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
