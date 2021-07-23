using Nest;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Infrastructure;

namespace ReferenceDataApi.V1.Gateways
{
    public class SearchReferenceDataQueryContainerOrchestrator : ISearchReferenceDataQueryContainerOrchestrator
    {
        public QueryContainer Create(GetReferenceDataQuery apiQuery, QueryContainerDescriptor<QueryableReferenceData> queryContainerDescriptor)
        {
            var queryContainer = queryContainerDescriptor.Term(term => term.Field(field => field.Category.Suffix("keyword"))
                                                                           .Value(apiQuery.Category));
            if (!string.IsNullOrEmpty(apiQuery.SubCategory))
                queryContainer = queryContainer && queryContainerDescriptor.Term(term => term.Field(field => field.SubCategory.Suffix("keyword"))
                                                                                             .Value(apiQuery.SubCategory));

            bool includeInactive = apiQuery.IncludeInactive.HasValue && apiQuery.IncludeInactive.Value;
            if (!includeInactive)
                queryContainer = queryContainer && queryContainerDescriptor.Term(field => field.IsActive, true);
            return queryContainer;
        }
    }
}
