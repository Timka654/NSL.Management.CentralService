using NSL.Management.CentralService.ExternalClient.Data.Enums;

namespace NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels
{
    public partial class SyncReportLogsRequestModel
    {
        public SyncReportLogDataModel[] Logs { get; set; }
    }


    public partial class SyncReportLogDataModel
    {
        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public LogLevelEnum LogLevel { get; set; }
    }
    public partial class SyncReportMetricsRequestModel
    {
        public SyncReportMetricDataModel[] Metrics { get; set; }
    }


    public partial class SyncReportMetricDataModel
    {
        public string Name { get; set; }

        public long Value { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        public MetricsOperationType OperationType { get; set; }

        public TimeSpan? ValidInterval { get; set; }
    }

    public enum MetricsOperationType
    {
        Increment,
        New
    }
}
