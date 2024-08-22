using Microsoft.AspNetCore.Mvc.Filters;
using NSL.Database.EntityFramework;
using NSL.Management.CentralService.Shared.Server.Data;
using NSL.Management.CentralService.Utils.Sync.Data;
using System.Collections.Concurrent;

namespace NSL.Management.CentralService.Utils.Sync.Services
{
    public class SyncIdentityService(IServiceProvider serviceProvider)
    {
        internal async Task<bool> TrySignAsync(ActionExecutingContext context, IHeaderDictionary headers)
        {
            if (!headers.TryGetValue("server_id", out var _serverId))
                return false;

            if (!headers.TryGetValue("identity_token", out var identityToken))
                return false;

            var serverId = Guid.Parse(_serverId);

            if (store.TryGetValue(serverId, out var identityData))
            {
                identityData.LastGet = DateTime.UtcNow;
                if (identityData.Token != identityToken)
                    return false;

                context.HttpContext.Items["serverId"] = serverId;

                return true;
            }

            await serviceProvider.InvokeDbTransactionAsync<ApplicationDbContext>(async (context, _iter) =>
            {
                var server = await context.Servers.FindAsync(serverId);

                if (server != null)
                    store.TryAdd(serverId, identityData = new SyncIdentityKeyData() { ServerId = serverId, Token = server.IdentityKey });

                return false;
            });

            if (identityData == null || identityData.Token != identityToken)
                return false;

            return true;
        }

        private ConcurrentDictionary<Guid, SyncIdentityKeyData> store = new();

        //private ConcurrentQueue<SyncIdentityKeyData> clearQueue = new();


        public void ClearCache()
        {
            //clearQueue.Clear();

            store.Clear();
        }

        public void ClearCache(Guid serverId)
        {
            store.TryRemove(serverId, out var identityData);
        }

        Timer clearTim;

        public Task LoadAsync()
        {
            clearTim = new Timer(e => clearTimTick(), null, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3));

            return Task.CompletedTask;
        }

        private void clearTimTick()
        {
            var to = DateTime.UtcNow.AddMinutes(-5);

            foreach (var item in store.Values.ToArray())
            {
                if (item.LastGet > to)
                    continue;

                store.TryRemove(item.ServerId, out _);
            }
        }
    }
}
