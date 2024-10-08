#if CLIENT

using NSL.Management.CentralService.ExternalClient.Data.Enums;

namespace NSL.Management.CentralService.Shared.Models
{
    public partial class ServerLogModel
    {
        public LogLevelEnum LogLevel { get; set; }
    }
}

#endif