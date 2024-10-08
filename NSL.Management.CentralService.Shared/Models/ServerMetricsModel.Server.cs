#if SERVER

using NSL.Generators.FillTypeGenerator.Attributes;
using NSL.Generators.SelectTypeGenerator.Attributes;
using NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels;

namespace NSL.Management.CentralService.Shared.Models
{
    [SelectGenerate("Get", "Details")]
    [SelectGenerateModelJoin("Details", "Get")]
    [FillTypeFromGenerate(typeof(SyncReportMetricDataModel))]
    public partial class ServerMetricsModel
    {
    }
}

#endif