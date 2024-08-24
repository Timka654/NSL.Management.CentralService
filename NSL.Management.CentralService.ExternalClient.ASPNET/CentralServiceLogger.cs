using NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels;

namespace NSL.Management.CentralService.ExternalClient.ASPNET
{
    public class CentralServiceLogger : ILogger
    {
        private readonly string name;
        private readonly CentralServiceLogProvider loggerProvider;

        public CentralServiceLogger(string name, CentralServiceLogProvider loggerProvider)
        {
            this.name = name;
            this.loggerProvider = loggerProvider;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
            => default!;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var createTime = DateTime.UtcNow;

            loggerProvider.EnqueueLog(new SyncReportLogDataModel()
            {
                Content = $"[{eventId.Id,2}]     {name} - {formatter(state, exception)}",
                CreateTime = createTime,
                LogLevel = (Data.Enums.LogLevelEnum)(int)logLevel
            });
        }
    }
}
