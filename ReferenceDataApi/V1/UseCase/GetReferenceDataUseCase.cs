using Hackney.Core.Logging;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace ReferenceDataApi.V1.UseCase
{
    public class GetReferenceDataUseCase : IGetReferenceDataUseCase
    {
        private readonly IReferenceDataGateway _gateway;
        public GetReferenceDataUseCase(IReferenceDataGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<ReferenceDataResponseObject> ExecuteAsync(GetReferenceDataQuery query)
        {
            var refDataResults = await _gateway.GetReferenceDataAsync(query).ConfigureAwait(false);
            return refDataResults.ToResponse();
        }
    }
}
