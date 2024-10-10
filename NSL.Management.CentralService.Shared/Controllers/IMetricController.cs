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
    public interface IMetricController
    {
        [HttpEndPointGenerate(typeof(DataResponse<int>))] Task<IActionResult> Clear([FromBody] EntityFilterQueryModel query);

        [HttpEndPointGenerate(typeof(DataResponse<FilterResultModel<ServerMetricsModel>>))] Task<IActionResult> Get([FromBody] EntityFilterQueryModel query);
        [HttpEndPointGenerate(typeof(DataResponse<long>))] Task<IActionResult> GetCount([FromBody] Guid serverId);

        [HttpEndPointGenerate(typeof(DataResponse<ServerMetricsModel>))] Task<IActionResult> GetDetails([FromBody] Guid query);
        [HttpEndPointGenerate(typeof(DataResponse<long>))] Task<IActionResult> CalculateAvg([FromBody] EntityFilterQueryModel query);
        [HttpEndPointGenerate(typeof(DataResponse<long>))] Task<IActionResult> CalculateMax([FromBody] EntityFilterQueryModel query);
        [HttpEndPointGenerate(typeof(DataResponse<long>))] Task<IActionResult> CalculateMin([FromBody] EntityFilterQueryModel query);
    }
}
