namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class ClearMetricsRequestModel
    {
        public Guid ServerId { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
