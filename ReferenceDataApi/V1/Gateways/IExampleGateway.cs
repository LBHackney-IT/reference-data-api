using System.Collections.Generic;
using ReferenceDataApi.V1.Domain;

namespace ReferenceDataApi.V1.Gateways
{
    public interface IExampleGateway
    {
        List<ReferenceData> GetAll();
    }
}
