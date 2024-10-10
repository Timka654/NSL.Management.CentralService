using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Authorization;
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

namespace NSL.Management.CentralService.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class LogController(ApplicationDbContext dbContext,
        IConfiguration configuration
        ) : Controller, ILogController
    {
        [HttpPostAction]
        public async Task<IActionResult> Get([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerLogs
                .Include(x => x.Server)
                .Filter(x => x.Where(x => x.Server.OwnerId == uid), query);

                return this.DataResponse(await dbq.ToDataResultAsync(x => x.SelectGet()));

            });
        [HttpPostAction]
        public async Task<IActionResult> GetCount([FromBody] Guid serverId)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var count = await dbContext.ServerLogs
                .Where(x => x.ServerId == serverId && x.Server.OwnerId == uid)
                .CountAsync();

                return this.DataResponse(count);

            });

        [HttpPostAction]
        public async Task<IActionResult> GetDetails([FromBody] Guid query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var details = await dbContext.ServerLogs
                .Include(x => x.Server)
                    .Where(x => x.Server.OwnerId == uid && x.Id == query)
                    .SelectDetails()
                    .FirstOrDefaultAsync();

                if (details == null)
                    return this.NotFoundResponse();


                return this.DataResponse((object)details);
            });

        [HttpPostAction]
        public async Task<IActionResult> Clear([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerLogs
                .Include(x => x.Server)
                .Filter(x => x.Where(x => x.Server.OwnerId == uid), query);

                var c = dbq.Count;

                await dbContext.BulkDeleteAsync(dbq.Data);

                return Ok(c);
            });
    }
}
