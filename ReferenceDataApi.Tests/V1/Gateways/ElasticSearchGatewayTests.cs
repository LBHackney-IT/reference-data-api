using Elasticsearch.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nest;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ReferenceDataApi.Tests.V1.Gateways
{
    [Collection("ElasticSearch collection")]
    public class ElasticSearchGatewayTests
    {
        private readonly ElasticSearchFixture<Startup> _testFixture;
        private IElasticClient EsClient => _testFixture?.ElasticSearchClient;

        private readonly Mock<ILogger<ElasticSearchGateway>> _mockLogger;

        public ElasticSearchGatewayTests(ElasticSearchFixture<Startup> testFixture)
        {
            _testFixture = testFixture;

            _mockLogger = new Mock<ILogger<ElasticSearchGateway>>();
        }

        private GetReferenceDataQuery ConstructQuery()
        {
            return new GetReferenceDataQuery()
            {
                Category = _testFixture.Category,
                SubCategory = _testFixture.SubCategory
            };
        }

        [Fact]
        public void GetReferenceDataAsyncSearchExceptionThrown()
        {
            var mockConnectionSettings = new Mock<IConnectionSettingsValues>();
            mockConnectionSettings.SetupGet(x => x.ConnectionPool).Returns(new SingleNodeConnectionPool(new Uri(_testFixture.NodeUri)));
            var mockClient = new Mock<IElasticClient>();
            mockClient.SetupGet(x => x.ConnectionSettings).Returns(mockConnectionSettings.Object);

            var mockContainerOrchestrator = new Mock<ISearchReferenceDataQueryContainerOrchestrator>();

            var sut = new ElasticSearchGateway(mockClient.Object,
                                               mockContainerOrchestrator.Object,
                                               _mockLogger.Object);

            var query = ConstructQuery();

            var exMessage = "Some exception message";
            var ex = new Exception(exMessage);
            mockClient.Setup(x => x.SearchAsync(It.IsAny<Func<SearchDescriptor<QueryableReferenceData>, ISearchRequest>>(), default))
                       .ThrowsAsync(ex);

            Func<Task<IEnumerable<ReferenceData>>> func =
                 async () => await sut.GetReferenceDataAsync(query);
            func.Should().ThrowAsync<Exception>().WithMessage(exMessage);

            mockContainerOrchestrator.Verify(x => x.Create(query, It.IsAny<QueryContainerDescriptor<QueryableReferenceData>>()), Times.Once);
        }

        [Fact]
        public async Task GetReferenceDataAsyncReturnsSearchResult()
        {
            var query = ConstructQuery();

            var sut = new ElasticSearchGateway(EsClient, new SearchReferenceDataQueryContainerOrchestrator(), _mockLogger.Object);

            var result = await sut.GetReferenceDataAsync(query);
            result.Should().BeEquivalentTo(_testFixture.Data.Where(r => (r.Category == query.Category) && (r.SubCategory == query.SubCategory))
                                                            .Select(x => x));

            _mockLogger.VerifyContains(LogLevel.Debug, $"ElasticSearch Search begins using {_testFixture.NodeUri}", Times.Once());
        }
    }
}
