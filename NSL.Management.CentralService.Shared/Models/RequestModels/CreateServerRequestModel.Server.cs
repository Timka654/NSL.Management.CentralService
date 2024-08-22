#if SERVER

using NSL.Generators.FillTypeGenerator.Attributes;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    [FillTypeGenerate(typeof(ServerModel))]
    public partial class CreateServerRequestModel
    {
    }
}

#endif