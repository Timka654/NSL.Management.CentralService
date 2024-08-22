#if SERVER
using Microsoft.AspNetCore.Mvc;
#else
using NSL.Generators.HttpEndPointGenerator.Shared.Fake.Attributes;
using NSL.Generators.HttpEndPointGenerator.Shared.Fake.Interfaces;
#endif

using NSL.Generators.HttpEndPointGenerator.Shared.Attributes;
using NSL.Management.CentralService.Shared.Models.RequestModels;
using NSL.HttpClient.Models;
using NSL.Database.EntityFramework.Filter.Models;
using NSL.Management.CentralService.Shared.Models;

namespace NSL.Management.CentralService.Shared.Controllers
{
    [HttpEndPointContainerGenerate("api/[controller]")]
    public interface IServerController
    {
        [HttpEndPointGenerate(typeof(IdResponse<Guid>))] Task<IActionResult> Create([FromBody] CreateServerRequestModel query);

        [HttpEndPointGenerate(typeof(BaseResponse))] Task<IActionResult> Edit([FromBody] EditServerRequestModel query);

        [HttpEndPointGenerate(typeof(DataResponse<FilterResultModel<ServerModel>>))] Task<IActionResult> Get([FromBody] BaseFilteredQueryModel query);

        [HttpEndPointGenerate(typeof(DataResponse<ServerModel>))] Task<IActionResult> GetDetails([FromBody] Guid query);

        [HttpEndPointGenerate(typeof(BaseResponse))] Task<IActionResult> Remove([FromBody] Guid query);
    }
}
