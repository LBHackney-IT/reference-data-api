using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReferenceDataApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/residents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class ReferenceDataApiController : BaseController
    {
        private readonly IGetAllUseCase _getAllUseCase;

        public ReferenceDataApiController(IGetAllUseCase getAllUseCase)
        {
            _getAllUseCase = getAllUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-reference-data-api/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListContacts()
        {
            return Ok(_getAllUseCase.Execute());
        }
    }
}
