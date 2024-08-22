using NSL.Generators.FillTypeGenerator.Attributes;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    [FillTypeGenerate(typeof(ServerModel))]
    public partial class EditServerRequestModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
