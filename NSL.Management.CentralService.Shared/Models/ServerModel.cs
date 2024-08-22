using NSL.Generators.FillTypeGenerator.Attributes;
using NSL.Generators.SelectTypeGenerator.Attributes;
using NSL.Management.CentralService.Shared.Models.RequestModels;

namespace NSL.Management.CentralService.Shared.Models
{
    [FillTypeGenerate(typeof(EditServerRequestModel))]
    public partial class ServerModel
    {
        [SelectGenerateInclude("Get")]
        public Guid Id { get; set; }

        [SelectGenerateInclude("Get")]
        public string Name { get; set; }

        [SelectGenerateInclude("Get")]
        public string IdentityKey { get; set; }

        public virtual UserModel? Owner { get; set; }
    }
}
