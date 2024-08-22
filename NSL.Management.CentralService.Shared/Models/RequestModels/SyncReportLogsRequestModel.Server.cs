#if SERVER

using NSL.Generators.FillTypeGenerator.Attributes;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class SyncReportLogsRequestModel
    {
        public SyncReportLogDataModel[] Logs { get; set; }
    }
    [FillTypeGenerate(typeof(ServerLogModel))]
    public partial class SyncReportLogDataModel
    {
        public string Content { get; set; }

        public DateTime CreateTime { get; set; }
    }
}

#endif