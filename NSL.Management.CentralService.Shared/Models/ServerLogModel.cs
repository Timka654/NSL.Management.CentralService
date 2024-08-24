using NSL.Generators.FillTypeGenerator.Attributes;
using NSL.Generators.SelectTypeGenerator.Attributes;
using NSL.Management.CentralService.Shared.Enums;

namespace NSL.Management.CentralService.Shared.Models
{
    public partial class ServerLogModel
    {
        [SelectGenerateInclude("Get")]
        public Guid Id { get; set; }

        [SelectGenerateInclude("Get")]
        public string Content { get; set; }

        [SelectGenerateInclude("Get")]
        public DateTime CreateTime { get; set; }


        public Guid ServerId { get; set; }

        public virtual ServerModel? Server { get; set; }
    }
}
