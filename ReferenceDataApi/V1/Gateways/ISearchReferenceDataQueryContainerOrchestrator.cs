using Nest;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Infrastructure;

namespace ReferenceDataApi.V1.Gateways
{
    public interface ISearchReferenceDataQueryContainerOrchestrator
    {
        QueryContainer Create(GetReferenceDataQuery apiQuery, QueryContainerDescriptor<QueryableReferenceData> queryContainerDescriptor);
    }
}
