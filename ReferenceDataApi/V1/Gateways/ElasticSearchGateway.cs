using Microsoft.Extensions.Logging;
using Nest;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReferenceDataApi.V1.Gateways
{
    public class ElasticSearchGateway : IReferenceDataGateway
    {
        private readonly IElasticClient _esClient;
        private readonly ILogger<ElasticSearchGateway> _logger;

        public ElasticSearchGateway(IElasticClient esClient, ILogger<ElasticSearchGateway> logger)
        {
            _esClient = esClient;
            _logger = logger;
        }

        public async Task<ReferenceData> GetReferenceDataAsync(GetReferenceDataQuery query)
        {
            return await Task.FromResult(new ReferenceData());
        }
    }
}
