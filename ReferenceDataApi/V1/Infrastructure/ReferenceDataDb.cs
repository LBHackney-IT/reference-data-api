using Amazon.DynamoDBv2.DataModel;
using System;

namespace ReferenceDataApi.V1.Infrastructure
{
    [DynamoDBTable("ReferenceData", LowerCamelCaseProperties = true)]
    public class ReferenceDataDb
    {
        [DynamoDBHashKey]
        public string Category { get; set; }

        [DynamoDBRangeKey]
        public Guid Id { get; set; }
    }
}
