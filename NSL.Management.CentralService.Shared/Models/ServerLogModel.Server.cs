#if SERVER

using NSL.Generators.FillTypeGenerator.Attributes;
using NSL.Generators.SelectTypeGenerator.Attributes;
using NSL.Management.CentralService.Shared.Enums;
using NSL.Management.CentralService.Shared.Models.RequestModels;

namespace NSL.Management.CentralService.Shared.Models
{
    [SelectGenerate("Get", "Details")]
    [SelectGenerateModelJoin("Details", "Get")]
    [FillTypeFromGenerate(typeof(SyncReportLogDataModel))]
    public partial class ServerLogModel
    {
        [SelectGenerateInclude("Get")]
        public LogLevelEnum LogLevel { get; set; }
    }
}

#endif