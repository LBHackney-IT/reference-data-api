using Nest;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Infrastructure;

namespace ReferenceDataApi.V1.Gateways
{
    public class SearchReferenceDataQueryContainerOrchestrator : ISearchReferenceDataQueryContainerOrchestrator
    {
        public QueryContainer Create(GetReferenceDataQuery apiQuery, QueryContainerDescriptor<QueryableReferenceData> queryContainerDescriptor)
        {
            var queryContainer = queryContainerDescriptor.Match(m => m.Field(f => f.Category).Query(apiQuery.Category));
            if (!string.IsNullOrEmpty(apiQuery.SubCategory))
                queryContainer = queryContainer && queryContainerDescriptor.Match(m => m.Field(f => f.SubCategory).Query(apiQuery.SubCategory));

            bool includeInactive = apiQuery.IncludeInactive.HasValue && apiQuery.IncludeInactive.Value;
            if (!includeInactive)
                queryContainer = queryContainer && queryContainerDescriptor.Term(m => m.IsActive, true);
            return queryContainer;
        }
    }
}
