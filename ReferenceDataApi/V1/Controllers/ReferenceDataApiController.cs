using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace ReferenceDataApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/reference-data")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ReferenceDataApiController : BaseController
    {
        private readonly IGetReferenceDataUseCase _getRefDataUseCase;

        public ReferenceDataApiController(IGetReferenceDataUseCase getRefDataUseCase)
        {
            _getRefDataUseCase = getRefDataUseCase;
        }

        [ProducesResponseType(typeof(ReferenceDataResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ReferenceDataResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ReferenceDataResponseObject), StatusCodes.Status500InternalServerError)]
        [HttpGet, MapToApiVersion("1")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetReferenceDataAsync(GetReferenceDataQuery query)
        {
            return Ok(await _getRefDataUseCase.ExecuteAsync(query));
        }
    }
}
