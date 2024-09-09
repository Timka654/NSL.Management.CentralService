using Microsoft.AspNetCore.Components;
using NSL.Database.EntityFramework.Filter;
using NSL.Database.EntityFramework.Filter.Models;
using NSL.Management.CentralService.Client.Services;
using NSL.Management.CentralService.Shared.Enums;
using NSL.Management.CentralService.Shared.Models;

namespace NSL.Management.CentralService.Client.Pages.Server
{
    public partial class DetailsPage : ComponentBase, IDisposable
    {
        [Parameter] public Guid DetailId { get; set; }

        [Inject] public ServersService ServersService { get; set; }


        ServerModel Details { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var response = await ServersService.ServerGetDetailsPostRequest(DetailId);

            if (!response.IsSuccess)
                return;

            Details = response.Data;

        }

        List<ServerLogModel> Logs { get; set; }

        long logsCount;

        long newLogsCount;

        bool haveNextLogs => logsCount != newLogsCount;

        bool havePrevLogs => logsCount > Logs.Count;

        DateTime? filterFrom = null;

        DateTime? filterTo = null;

        LogLevelEnum? filterLevel = null;

        bool HaveFilter => filterFrom != null || filterTo != null || filterLevel != null;

        async Task clearFilter()
        {
            filterFrom = null;
            filterTo = null;
            filterLevel = null;

            await _loadLogs();
        }

        async Task LoadLogs()
        {
            if (Logs != null)
                return;

            await _loadLogs();

            logsNewCountGetting();
        }

        private const int loadCount = 100;

        async Task _loadLogs()
        {
            var filter = NavigationFilterBuilder
                .Create()
                .SetOffset(0)
                .SetCount(loadCount)
                .CreateFilterBlock(x => SetFilters(x).AddFilter(nameof(ServerLogModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerLogModel.CreateTime), false);

            var response = await ServersService.LogGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            newLogsCount = logsCount = response.Data.Count;

            Logs = response.Data.Data.Reverse().ToList();
        }


        private async Task logsLoadPrev()
        {
            var newOffset = logsCount - Logs.Count - loadCount;

            var count = (long)loadCount;

            if (newOffset < 0)
            {
                count += newOffset;
                newOffset = 0;
            }

            var filter = NavigationFilterBuilder
                .Create()
                .SetOffset((int)newOffset)
                .SetCount((int)count)
                .CreateFilterBlock(x => SetFilters(x).AddFilter(nameof(ServerLogModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerLogModel.CreateTime));

            var response = await ServersService.LogGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            newLogsCount = response.Data.Count;

            Logs = response.Data.Data.Concat(Logs).ToList();
        }

        private async Task logsLoadNext()
        {
            var nlc = newLogsCount;

            var filter = NavigationFilterBuilder
                .Create()
                .SetOffset((int)logsCount)
                .SetCount((int)(nlc - logsCount))
                .CreateFilterBlock(x => SetFilters(x).AddFilter(nameof(ServerLogModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerLogModel.CreateTime));

            var response = await ServersService.LogGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            logsCount = nlc;

            newLogsCount = response.Data.Count;

            Logs = Logs.Concat(response.Data.Data).ToList();
        }

        private FilterBlockViewModel SetFilters(FilterBlockViewModel block)
        {
            if (HaveFilter)
            {
                if (filterFrom.HasValue)
                    block.AddFilter(nameof(ServerLogModel.CreateTime), Database.EntityFramework.Filter.Enums.CompareType.More, filterFrom.Value);
                if (filterTo.HasValue)
                    block.AddFilter(nameof(ServerLogModel.CreateTime), Database.EntityFramework.Filter.Enums.CompareType.Less, filterTo.Value);
                if (filterLevel.HasValue)
                    block.AddFilter(nameof(ServerLogModel.LogLevel), Database.EntityFramework.Filter.Enums.CompareType.Equals, filterLevel.Value);
            }

            return block;
        }

        private async void logsNewCountGetting()
        {
            try
            {
                logsCTS = new CancellationTokenSource();
                var token = logsCTS.Token;
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(5_000, token);

                    var response = await ServersService.LogGetCountPostRequest(DetailId);

                    if (response.IsSuccess)
                    {
                        newLogsCount = response.Data;

                        if (haveNextLogs)
                        {
                            StateHasChanged();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private CancellationTokenSource? logsCTS;

        public void Dispose()
        {
            logsCTS?.Cancel();
        }
    }
}
