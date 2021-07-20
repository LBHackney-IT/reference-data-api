using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using Nest;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReferenceDataApi.V1.Gateways
{
    public class ElasticSearchGateway : IReferenceDataGateway
    {
        private readonly IElasticClient _esClient;
        private readonly ISearchReferenceDataQueryContainerOrchestrator _containerOrchestrator;
        private readonly ILogger<ElasticSearchGateway> _logger;

        private readonly Indices.ManyIndices _indices;
        private const int MaxResults = 1000;

        public static string EsIndex => "reference_data";

        public ElasticSearchGateway(IElasticClient esClient,
            ISearchReferenceDataQueryContainerOrchestrator containerOrchestrator,
            ILogger<ElasticSearchGateway> logger)
        {
            _esClient = esClient;
            _containerOrchestrator = containerOrchestrator;
            _logger = logger;

            _indices = Indices.Index(new List<IndexName> { EsIndex });
        }

        [LogCall]
        public async Task<IEnumerable<ReferenceData>> GetReferenceDataAsync(GetReferenceDataQuery query)
        {
            var esNodes = string.Join(';', _esClient.ConnectionSettings.ConnectionPool.Nodes.Select(x => x.Uri));
            _logger.LogDebug($"ElasticSearch Search begins using {esNodes}");

            var results = await _esClient.SearchAsync<QueryableReferenceData>(ConstructSearchRequest(query))
                .ConfigureAwait(false);

            return new List<ReferenceData>(results.Documents.Select(x => x.ToDomain()));
        }

        private ISearchRequest ConstructSearchRequest(GetReferenceDataQuery query)
        {
            return new SearchRequest(_indices)
            {
                Query = _containerOrchestrator.Create(query, new QueryContainerDescriptor<QueryableReferenceData>()),
                Size = MaxResults
            };
        }
    }
}
