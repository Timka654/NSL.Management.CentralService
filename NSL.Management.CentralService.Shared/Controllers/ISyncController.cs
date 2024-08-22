#if SERVER
using Microsoft.AspNetCore.Mvc;
#else
using NSL.Generators.HttpEndPointGenerator.Shared.Fake.Attributes;
using NSL.Generators.HttpEndPointGenerator.Shared.Fake.Interfaces;
#endif

using NSL.Generators.HttpEndPointGenerator.Shared.Attributes;
using NSL.Management.CentralService.Shared.Models.RequestModels;
using NSL.HttpClient.Models;

namespace NSL.Management.CentralService.Shared.Controllers
{
    [HttpEndPointContainerGenerate("api/[controller]")]
    public interface ISyncController
    {
        //[HttpEndPointGenerate(typeof(DataResponse<string>))] Task<IActionResult> Login([FromBody] IdentityLoginRequestModel query);

        //[HttpEndPointGenerate(typeof(DataResponse<string>))] Task<IActionResult> Register([FromBody] IdentityRegisterRequestModel query);
    }
}
