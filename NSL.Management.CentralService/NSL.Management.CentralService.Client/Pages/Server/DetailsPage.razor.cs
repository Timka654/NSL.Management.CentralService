using Microsoft.AspNetCore.Components;
using NSL.Database.EntityFramework.Filter;
using NSL.Management.CentralService.Client.Services;
using NSL.Management.CentralService.Shared.Models;

namespace NSL.Management.CentralService.Client.Pages.Server
{
    public partial class DetailsPage : ComponentBase
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

        bool haveNewLogs { get; set; }

        async Task LoadLogs()
        {
            if (Logs != null)
                return;

            var filter = NavigationFilterBuilder
                .Create()
                .SetOffset(0)
                .SetCount(30)
                .AddOrder(nameof(ServerLogModel.CreateTime), false);

            var response = await ServersService.LogGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            logsCount = response.Data.Count;

            Logs = response.Data.Data.Reverse().ToList();

        }
    }
}
