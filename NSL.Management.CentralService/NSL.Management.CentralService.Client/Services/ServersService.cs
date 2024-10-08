using Blazored.LocalStorage;
using NSL.ASPNET.Identity.ClientIdentity.Providers;
using NSL.Generators.HttpEndPointGenerator.Shared.Attributes;
using NSL.Management.CentralService.Shared.Controllers;

namespace NSL.Management.CentralService.Client.Services
{
    [HttpEndPointImplementGenerate(typeof(IServerController))]
    [HttpEndPointImplementGenerate(typeof(ILogController))]
    [HttpEndPointImplementGenerate(typeof(IMetricController))]
    public partial class ServersService(IdentityStateProvider identityStateProvider
        , IHttpClientFactory httpClientFactory
        , ILocalStorageService localStorage)
    {
        protected partial System.Net.Http.HttpClient CreateEndPointClient(string url)
            => httpClientFactory.CreateClient("ServerAPI");
    }
}
