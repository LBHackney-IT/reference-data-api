using AutoFixture;
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
    [Collection("LogCall collection")]
    public class ElasticSearchGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IElasticClient> _mockClient;
        private readonly Mock<ISearchReferenceDataQueryContainerOrchestrator> _mockContainerOrchestrator;
        private readonly Mock<ILogger<ElasticSearchGateway>> _mockLogger;

        private const string NodeUri = "http://localhost:9200/";
        private const string Category = "SomeCategory";
        private const string SubCategory = "SomeSubCategory";

        private readonly ElasticSearchGateway _sut;

        public ElasticSearchGatewayTests()
        {
            var mockConnectionSettings = new Mock<IConnectionSettingsValues>();
            mockConnectionSettings.SetupGet(x => x.ConnectionPool).Returns(new SingleNodeConnectionPool(new Uri(NodeUri)));
            _mockClient = new Mock<IElasticClient>();
            _mockClient.SetupGet(x => x.ConnectionSettings).Returns(mockConnectionSettings.Object);

            _mockContainerOrchestrator = new Mock<ISearchReferenceDataQueryContainerOrchestrator>();
            _mockLogger = new Mock<ILogger<ElasticSearchGateway>>();

            _sut = new ElasticSearchGateway(_mockClient.Object,
                                            _mockContainerOrchestrator.Object,
                                            _mockLogger.Object);
        }

        private GetReferenceDataQuery ConstructQuery()
        {
            return new GetReferenceDataQuery()
            {
                Category = Category,
                SubCategory = SubCategory
            };
        }

        [Fact]
        public void GetReferenceDataAsyncSearchExceptionThrown()
        {
            var query = ConstructQuery();

            var exMessage = "Some exception message";
            var ex = new Exception(exMessage);
            _mockClient.Setup(x =>
                            x.SearchAsync(It.IsAny<Func<SearchDescriptor<QueryableReferenceData>, ISearchRequest>>(), default))
                        .ThrowsAsync(ex);

            Func<Task<IEnumerable<ReferenceData>>> func =
                 async () => await _sut.GetReferenceDataAsync(query);
            func.Should().ThrowAsync<Exception>().WithMessage(exMessage);
        }

        [Fact]
        public async Task GetReferenceDataAsyncReturnsSearchResult()
        {
            var query = ConstructQuery();

            var mockQueryContainer = new Mock<QueryContainer>();
            _mockContainerOrchestrator.Setup(x => x.Create(query, It.IsAny<QueryContainerDescriptor<QueryableReferenceData>>()))
                                       .Returns(mockQueryContainer.Object);


            var searchResults = _fixture.Build<QueryableReferenceData>()
                                        .With(x => x.Category, Category)
                                        .With(x => x.SubCategory, SubCategory)
                                        .CreateMany(10);

            var mockSearchResponse = new Mock<ISearchResponse<QueryableReferenceData>>();
            mockSearchResponse.SetupGet(x => x.Documents).Returns(searchResults.ToList().AsReadOnly());
            _mockClient.Setup(x => x.SearchAsync<QueryableReferenceData>(It.IsAny<ISearchRequest>(), default))
                       .ReturnsAsync(mockSearchResponse.Object);

            var result = await _sut.GetReferenceDataAsync(query);
            result.Should().BeEquivalentTo(searchResults.Select(x => x));

            _mockLogger.VerifyExact(LogLevel.Debug, $"ElasticSearch Search begins using {NodeUri}", Times.Once());

            _mockContainerOrchestrator.Verify(x => x.Create(query, It.IsAny<QueryContainerDescriptor<QueryableReferenceData>>()), Times.Once);
            _mockClient.Verify(x => x.SearchAsync<QueryableReferenceData>(It.Is<ISearchRequest>(
                                    y => VerifySearch(y, mockQueryContainer.Object)), default),
                               Times.Once);
        }

        private bool VerifySearch(ISearchRequest sr, QueryContainer expectedQueryContainer)
        {
            sr.Index.ToString().Should().Be("Count: 1 [(1: referencedata)]");
            sr.Size.Should().Be(int.MaxValue);
            sr.Query.Should().Be(expectedQueryContainer);
            return true;
        }
    }
}
