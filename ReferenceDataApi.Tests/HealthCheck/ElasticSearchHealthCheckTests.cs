using FluentAssertions;
using ReferenceDataApi.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Nest;
using Elasticsearch.Net;

namespace ReferenceDataApi.Tests.HealthCheck
{
    public class ElasticSearchHealthCheckTests
    {
        private readonly Mock<IElasticClient> _mockClient;
        private readonly ElasticSearchHealthCheck _sut;
        private const string NodeUri = "http://localhost:9200";

        public ElasticSearchHealthCheckTests()
        {
            _mockClient = new Mock<IElasticClient>();

            var mockConnectionSettings = new Mock<IConnectionSettingsValues>();
            mockConnectionSettings.SetupGet(x => x.ConnectionPool).Returns(new SingleNodeConnectionPool(new Uri(NodeUri)));
            _mockClient.SetupGet(x => x.ConnectionSettings).Returns(mockConnectionSettings.Object);

            _sut = new ElasticSearchHealthCheck(_mockClient.Object);
        }

        private Mock<PingResponse> ConstructPingResponse(int statusCode)
        {
            var mockCallDetails = new Mock<IApiCallDetails>();
            mockCallDetails.SetupGet(x => x.HttpStatusCode).Returns(statusCode);

            var mockResponse = new Mock<PingResponse>();
            mockResponse.SetupGet(x => x.ApiCall).Returns(mockCallDetails.Object);
            return mockResponse;
        }

        [Fact]
        public void DynamoDbHealthCheckConstructorTestSuccess()
        {
            (new ElasticSearchHealthCheck(_mockClient.Object)).Should().NotBeNull();
        }

        [Fact]
        public async Task CheckHealthAsyncTestClientCallSucceeds()
        {
            var mockPingResponse = ConstructPingResponse(200);
            _mockClient.Setup(x => x.PingAsync((Func<PingDescriptor, IPingRequest>) null, default))
                       .ReturnsAsync(mockPingResponse.Object);

            var result = await _sut.CheckHealthAsync(new HealthCheckContext()).ConfigureAwait(false);
            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [Fact]
        public async Task CheckHealthAsyncTestClientCallFails()
        {
            var mockPingResponse = ConstructPingResponse(400);
            _mockClient.Setup(x => x.PingAsync((Func<PingDescriptor, IPingRequest>) null, default))
                       .ReturnsAsync(mockPingResponse.Object);

            var result = await _sut.CheckHealthAsync(new HealthCheckContext()).ConfigureAwait(false);
            result.Status.Should().Be(HealthStatus.Unhealthy);
        }

        [Fact]
        public async Task CheckHealthAsyncTestFailsFromException()
        {
            var ex = new Exception("Something bad happened");
            _mockClient.Setup(x => x.PingAsync((Func<PingDescriptor, IPingRequest>) null, default)).ThrowsAsync(ex);

            var result = await _sut.CheckHealthAsync(new HealthCheckContext()).ConfigureAwait(false);
            result.Status.Should().Be(HealthStatus.Unhealthy);
            result.Exception.Should().Be(ex);
        }
    }
}
