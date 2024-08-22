#if SERVER

using NSL.Generators.SelectTypeGenerator.Attributes;

namespace NSL.Management.CentralService.Shared.Models
{
    [SelectGenerate("Get", "Details")]
    [SelectGenerateModelJoin("Details", "Get")]
    public partial class ServerLogModel
    {
    }
}

#endif