#if CLIENT

using NSL.Management.CentralService.Shared.Enums;

namespace NSL.Management.CentralService.Shared.Models
{
    public partial class ServerLogModel
    {
        public LogLevelEnum LogLevel { get; set; }
    }
}

#endif