using Microsoft.Extensions.Logging;
using Nest;
using System.Collections.Generic;

namespace ReferenceDataApi.V1.Gateways
{
    public class ElasticSearchGateway : IExampleGateway
    {
        private readonly IElasticClient _esClient;
        private readonly ILogger<ElasticSearchGateway> _logger;

        public ElasticSearchGateway(IElasticClient esClient, ILogger<ElasticSearchGateway> logger)
        {
            _esClient = esClient;
            _logger = logger;
        }

        public List<Domain.ReferenceData> GetAll()
        {
            return new List<Domain.ReferenceData>();
        }
    }
}
