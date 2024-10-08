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
using NSL.Management.CentralService.Shared.Server.Manages;

namespace NSL.Management.CentralService.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ServerController(ApplicationDbContext dbContext,
        IConfiguration configuration
        ) : Controller, IServerController
    {
        [HttpPostAction]
        public async Task<IActionResult> Get([FromBody] EntityFilterQueryModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var dbq = dbContext.Servers.Filter(x => x.Where(x => x.OwnerId == uid), query);

                return this.DataResponse(await dbq.ToDataResultAsync(x => x.SelectGet()));

            });

        [HttpPostAction]
        public async Task<IActionResult> GetDetails([FromBody] Guid query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var details = await dbContext.Servers
                    .Where(x => x.OwnerId == uid && x.Id == query)
                    .SelectDetails()
                    .FirstOrDefaultAsync();

                if (details == null)
                    return this.NotFoundResponse();


                return this.DataResponse((object)details);
            });

        [HttpPostAction]
        public async Task<IActionResult> Create([FromBody] CreateServerRequestModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var instance = new ServerModel();

                query.FillTo(instance);

                instance.OwnerId = uid;

                dbContext.Servers.Add(instance);

                await dbContext.SaveChangesAsync();

                return this.IdResponse(instance.Id);
            });

        [HttpPostAction]
        public async Task<IActionResult> Edit([FromBody] EditServerRequestModel query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var details = await dbContext.Servers
                    .Where(x => x.OwnerId == uid && x.Id == query.Id)
                    .FirstOrDefaultAsync();

                if (details != null)
                {
                    query.FillTo(details);

                    await dbContext.SaveChangesAsync();
                }

                return Ok();
            });

        [HttpPostAction]
        public async Task<IActionResult> Remove([FromBody] Guid query)
            => await this.ProcessRequestAsync(async () =>
            {
                var uid = User.GetId();

                var count = await dbContext.Servers
                    .Where(x => x.OwnerId == uid && x.Id == query)
                    .ExecuteDeleteAsync();

                return Ok();
            });
    }
}
