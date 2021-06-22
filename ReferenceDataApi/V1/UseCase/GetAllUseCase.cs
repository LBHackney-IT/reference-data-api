using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.UseCase.Interfaces;

namespace ReferenceDataApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class GetAllUseCase : IGetAllUseCase
    {
        private readonly IExampleGateway _gateway;
        public GetAllUseCase(IExampleGateway gateway)
        {
            _gateway = gateway;
        }

        public ResponseObjectList Execute()
        {
            return new ResponseObjectList { ResponseObjects = _gateway.GetAll().ToResponse() };
        }
    }
}
