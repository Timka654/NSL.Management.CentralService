#if SERVER

using NSL.Generators.SelectTypeGenerator.Attributes;

namespace NSL.Management.CentralService.Shared.Models
{
    [SelectGenerate("Get", "Details")]
    [SelectGenerateModelJoin("Details", "Get")]
    public partial class ServerModel
    {
        public virtual string OwnerId { get; set; }

        public virtual List<ServerLogModel>? Logs { get; set; }
    }
}

#endif