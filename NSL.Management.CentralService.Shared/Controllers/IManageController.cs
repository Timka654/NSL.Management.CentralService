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

namespace NSL.Management.CentralService.Shared.Controllers
{
    [HttpEndPointContainerGenerate("api/[controller]")]
    public interface IServerController
    {
        //[HttpEndPointGenerate(typeof(DataResponse<string>))] Task<IActionResult> Login([FromBody] IdentityLoginRequestModel query);

        //[HttpEndPointGenerate(typeof(DataResponse<string>))] Task<IActionResult> Register([FromBody] IdentityRegisterRequestModel query);
        Task<IActionResult> Create([FromBody] CreateServerRequestModel query);
        Task<IActionResult> Edit([FromBody] EditServerRequestModel query);
        Task<IActionResult> Get([FromBody] BaseFilteredQueryModel query);
        Task<IActionResult> GetDetails([FromBody] Guid query);
        Task<IActionResult> Remove([FromBody] Guid query);
    }
}
