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
    public interface ILogController
    {
        [HttpEndPointGenerate(typeof(BaseResponse))] Task<IActionResult> Clear([FromBody] ClearLogsRequestModel query);

        [HttpEndPointGenerate(typeof(DataResponse<FilterResultModel<ServerLogModel>>))] Task<IActionResult> Get([FromBody] BaseFilteredQueryModel query);

        [HttpEndPointGenerate(typeof(DataResponse<long>))] Task<IActionResult> GetCount([FromBody] Guid serverId);

        [HttpEndPointGenerate(typeof(DataResponse<ServerLogModel>))] Task<IActionResult> GetDetails([FromBody] Guid query);
    }
}
