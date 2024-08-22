using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NSL.Database.EntityFramework.Filter;
using NSL.Database.EntityFramework.Filter.Models;
using NSL.Management.CentralService.Client.Services;
using NSL.Management.CentralService.Shared.Models;
using NSL.Management.CentralService.Shared.Models.RequestModels;

namespace NSL.Management.CentralService.Client.Pages.Server
{
    public partial class ManagePage : ComponentBase
    {
        [Inject] ServersService ServersService { get; set; }

        private Grid<ServerModel> gridRef;

        public async Task<GridDataProviderResult<ServerModel>> GridDataProvider(GridDataProviderRequest<ServerModel> request)
        {
            var query = NavigationFilterBuilder.Create()
                .SetOffsetFromPage(request.PageNumber, request.PageSize)
                //.CreateFilterBlock()
                .ClearEmptyFilter()
                .ToFilter();

            var response = await ServersService.ServerGetPostRequest(query);

            if (!response.IsSuccess)
                return new GridDataProviderResult<ServerModel>();

            return new GridDataProviderResult<ServerModel>() { Data = response.Data.Data, TotalCount = (int)response.Data.Count };
        }

        async Task Remove(ServerModel item)
        { 
        
        }


        CreateServerRequestModel createRequestData { get; set; } = new CreateServerRequestModel();

        public async Task CreateHandle()
        {
            createRequestData.IdentityKey = string.Join("", Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray());

            var response = await ServersService.ServerCreatePostRequest(createRequestData);

            if (response.IsSuccess) await gridRef.RefreshDataAsync();
        }
    }
}
