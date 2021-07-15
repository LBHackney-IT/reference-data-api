using System.Collections.Generic;
using System.Threading.Tasks;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Domain;

namespace ReferenceDataApi.V1.Gateways
{
    public interface IReferenceDataGateway
    {
        Task<IEnumerable<ReferenceData>> GetReferenceDataAsync(GetReferenceDataQuery query);
    }
}
