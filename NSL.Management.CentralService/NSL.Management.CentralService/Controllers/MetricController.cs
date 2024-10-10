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
        public async Task<IActionResult> CalculateMin([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerMetrics
                .Include(x => x.Server)
                .Filter(x => x.Where(x => x.Server.OwnerId == uid), query);

                var r = dbq.Count > 0 ? await dbq.Data.MinAsync(x => x.Value) : 0;


                return this.DataResponse(r);
            });

        [HttpPostAction]
        public async Task<IActionResult> CalculateAvg([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerMetrics
                .Include(x => x.Server)
                .Filter(x => x.Where(x => x.Server.OwnerId == uid), query);

                var r = dbq.Count > 0 ? await dbq.Data.AverageAsync(x => x.Value) : 0;

                return this.DataResponse(r);
            });

        [HttpPostAction]
        public async Task<IActionResult> CalculateMax([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerMetrics
                .Include(x => x.Server)
                .Filter(x => x.Where(x => x.Server.OwnerId == uid), query);

                var r = dbq.Count > 0 ? await dbq.Data.MaxAsync(x => x.Value) : 0;

                return this.DataResponse(r);
            });

        [HttpPostAction]
        public async Task<IActionResult> GetCount([FromBody] Guid serverId)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var count = await dbContext.ServerMetrics
                .Where(x => x.ServerId == serverId && x.Server.OwnerId == uid)
                .CountAsync();

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
        public async Task<IActionResult> Clear([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.ServerLogs
                .Include(x => x.Server)
                .Filter(x => x.Where(x => x.Server.OwnerId == uid), query);

                var count = dbq.Count;

                await dbContext.BulkDeleteAsync(dbq.Data);

                return this.DataResponse(count);
            });
    }
}
