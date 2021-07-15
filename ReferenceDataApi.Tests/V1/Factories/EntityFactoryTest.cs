using AutoFixture;
using FluentAssertions;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.Infrastructure;
using Xunit;

namespace ReferenceDataApi.Tests.V1.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapAQueryableReferenceDataToADomainObject()
        {
            var queryableObject = _fixture.Create<QueryableReferenceData>();
            var entity = queryableObject.ToDomain();

            entity.Should().BeEquivalentTo(queryableObject);
        }
    }
}
