using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using ReferenceDataApi.V1.Factories;
using System.Collections.Generic;

namespace ReferenceDataApi.V1.Gateways
{
    public class DynamoDbGateway : IExampleGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbGateway> _logger;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, ILogger<DynamoDbGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _logger = logger;
        }

        public List<Domain.ReferenceData> GetAll()
        {
            return new List<Domain.ReferenceData>();
        }

        public Domain.ReferenceData GetEntityById(int id)
        {
            var result = _dynamoDbContext.LoadAsync<Infrastructure.ReferenceDataDb>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }
    }
}
