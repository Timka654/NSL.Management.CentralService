﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSL.ASPNET.Identity.Host;
using NSL.ASPNET.Mvc;
using NSL.ASPNET.Mvc.Route.Attributes;
using NSL.Database.EntityFramework.Filter.Host;
using NSL.Database.EntityFramework.Filter.Models;
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
    }
}
