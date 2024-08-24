﻿using NSL.Management.CentralService.Shared.Enums;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class SyncReportLogsRequestModel
    {
        public SyncReportLogDataModel[] Logs { get; set; }
    }

    public partial class SyncReportLogDataModel
    {
        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public LogLevelEnum LogLevel { get; set; }
    }
}
