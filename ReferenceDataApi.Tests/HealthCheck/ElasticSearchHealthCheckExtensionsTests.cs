using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using ReferenceDataApi.HealthCheck;
using System.Linq;
using Xunit;

namespace ReferenceDataApi.Tests.HealthCheck
{
    public class ElasticSearchHealthCheckExtensionsTests
    {
        [Fact]
        public void RegisterElasticSearchHealthCheckTest()
        {
            var services = new ServiceCollection();
            _ = services.RegisterElasticSearchHealthCheck();

            services.Any(x => (x.ServiceType == typeof(IHealthCheck))
                           && (x.ImplementationType == typeof(ElasticSearchHealthCheck))).Should().BeTrue();
        }

        [Fact]
        public void ServiceCollectionAddElasticSearchHealthCheckTest()
        {
            var services = new ServiceCollection();
            _ = services.AddElasticSearchHealthCheck();

            services.Any(x => (x.ServiceType == typeof(IHealthCheck))
                           && (x.ImplementationType == typeof(ElasticSearchHealthCheck))).Should().BeTrue();

            // We can't explicitly verify the Healthcheck builder reigstration here as it is not accessible.
            // We have to reply on the test below to do that for us.
        }

        [Fact]
        public void HealthChecksBuilderAddElasticSearchHealthCheckTest()
        {
            var mockBuilder = new Mock<IHealthChecksBuilder>();
            _ = mockBuilder.Object.AddElasticSearchHealthCheck();

            mockBuilder.Verify(x => x.Add(It.Is<HealthCheckRegistration>(hcr => hcr.Name == "Elastic search"
                                                                             && hcr.Factory != null)), Times.Once);
        }
    }
}
