using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSL.ASPNET.Identity.Host;
using NSL.ASPNET.Mvc;
using NSL.ASPNET.Mvc.Route.Attributes;
using NSL.Database.EntityFramework.Filter.Host;
using NSL.Database.EntityFramework.Filter.Models;
using NSL.Management.CentralService.Shared.Controllers;
using NSL.Management.CentralService.Shared.Models.RequestModels;
using NSL.Management.CentralService.Shared.Server.Data;

namespace NSL.Management.CentralService.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MetricController(ApplicationDbContext dbContext,
        IConfiguration configuration
        ) : Controller, IMetricController
    {
        [HttpPostAction]
        public async Task<IActionResult> Get([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerMetrics
                .Include(x => x.Server)
                .Filter(x => x.Where(x => x.Server.OwnerId == uid), query);

                return this.DataResponse(await dbq.ToDataResultAsync(x => x.SelectGet()));

            });
        [HttpPostAction]
        public async Task<IActionResult> GetCount([FromBody] Guid serverId)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var count = await dbContext.ServerMetrics
                .Where(x => x.ServerId == serverId && x.Server.OwnerId == uid)
                .CountAsync(); ;

                return this.DataResponse(count);

            });

        [HttpPostAction]
        public async Task<IActionResult> GetDetails([FromBody] Guid query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var details = await dbContext.ServerMetrics
                .Include(x => x.Server)
                    .Where(x => x.Server.OwnerId == uid && x.Id == query)
                    .SelectDetails()
                    .FirstOrDefaultAsync();

                if (details == null)
                    return this.NotFoundResponse();


                return this.DataResponse((object)details);
            });

        [HttpPostAction]
        public async Task<IActionResult> Clear([FromBody] ClearMetricsRequestModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerMetrics
                .Include(x => x.Server)
                    .Where(x => x.Server.OwnerId == uid && x.ServerId == query.ServerId);

                if (query.From.HasValue)
                    dbq = dbq.Where(x => x.CreateTime > query.From.Value);

                if (query.To.HasValue)
                    dbq = dbq.Where(x => x.CreateTime < query.To.Value);

                var count = await dbq
                    .ExecuteDeleteAsync();

                return Ok();
            });
    }
}
