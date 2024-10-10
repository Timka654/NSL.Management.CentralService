using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSL.ASPNET.Identity.Host;
using NSL.ASPNET.Mvc;
using NSL.ASPNET.Mvc.Route.Attributes;
using NSL.Database.EntityFramework.Filter.Host;
using NSL.Database.EntityFramework.Filter.Models;
using NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels;
using NSL.Management.CentralService.Shared.Controllers;
using NSL.Management.CentralService.Shared.Models;
using NSL.Management.CentralService.Shared.Models.RequestModels;
using NSL.Management.CentralService.Shared.Server.Data;
using NSL.Management.CentralService.Utils.Sync;
using NSL.Management.CentralService.Utils.Sync.Attributes;

namespace NSL.Management.CentralService.Controllers
{
    [Route("api/[controller]")]
    [SyncIdentityFilter]
    public class SyncController(ApplicationDbContext dbContext,
        IConfiguration configuration
        ) : Controller, ISyncController
    {
        [HttpPostAction]
        public async Task<IActionResult> LogReport([FromBody] SyncReportLogsRequestModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var serverId = this.GetSyncServerId();

                dbContext.ServerLogs.AddRange(query.Logs.Select(x =>
                {
                    var log = new ServerLogModel();

                    log.FillFrom(x);

                    log.ServerId = serverId;

                    return log;
                }));
                
                await dbContext.SaveChangesAsync();

                return Ok();

            });
        [HttpPostAction]
        public async Task<IActionResult> MetricsReport([FromBody] SyncReportMetricsRequestModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var serverId = this.GetSyncServerId();

                var dbMetrics = dbContext.ServerMetrics.Where(x => x.ServerId == serverId);

                foreach (var item in query.Metrics.Where(x=>x.OperationType == MetricsOperationType.Increment).ToArray())
                {
                    var dbitem = await dbMetrics.FirstOrDefaultAsync(x => x.Name == item.Name 
                    && (item.TimeInterval.HasValue && x.CreateTime == item.CreateTime));

                    if (dbitem != null)
                    {
                        dbitem.Value += item.Value;
                        continue;
                    }

                    item.OperationType = MetricsOperationType.Isolate;
                }

                dbContext.ServerMetrics.AddRange(query.Metrics
                    .Where(x=>x.OperationType == MetricsOperationType.Isolate)
                    .Select(x =>
                {
                    var log = new ServerMetricsModel();

                    log.FillFrom(x);

                    log.ServerId = serverId;

                    return log;
                }));
                
                await dbContext.SaveChangesAsync();

                return Ok();

            });
    }
}
