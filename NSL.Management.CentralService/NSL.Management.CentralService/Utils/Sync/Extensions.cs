using Microsoft.AspNetCore.Mvc;
using NSL.Management.CentralService.Utils.Sync.Services;

namespace NSL.Management.CentralService.Utils.Sync
{
    public static class Extensions
    {
        public static Guid GetSyncServerId(this ControllerBase controller)
        {
            return (Guid)controller.HttpContext.Items["serverId"];
        }

        public static IServiceCollection AddSyncIdentityService(this IServiceCollection collection)
        {
            collection.AddSingleton<SyncIdentityService>();

            return collection;
        }

        public static async Task<IApplicationBuilder> LoadSyncIdentityServiceAsync(this IApplicationBuilder applicationBuilder)
        {
            await applicationBuilder.ApplicationServices.GetRequiredService<SyncIdentityService>().LoadAsync(); 

            return applicationBuilder;
        }
    }
}
