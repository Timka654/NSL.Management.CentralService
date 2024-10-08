using NSL.Generators.SelectTypeGenerator.Attributes;

namespace NSL.Management.CentralService.Shared.Models
{
    public partial class ServerMetricsModel
    {
        [SelectGenerateInclude("Get")]
        public Guid Id { get; set; }

        [SelectGenerateInclude("Get")]
        public string Name { get; set; }

        [SelectGenerateInclude("Get")]
        public long Value { get; set; }

        [SelectGenerateInclude("Get")]
        public DateTime CreateTime { get; set; }

        public Guid ServerId { get; set; }

        public virtual ServerModel? Server { get; set; }
    }
}
