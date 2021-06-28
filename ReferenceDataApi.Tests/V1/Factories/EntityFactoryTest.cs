using AutoFixture;
using FluentAssertions;
using ReferenceDataApi.V1.Factories;
using Xunit;

namespace ReferenceDataApi.Tests.V1.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        //TODO: add assertions for all the fields being mapped in `EntityFactory.ToDomain()`. Also be sure to add test cases for
        // any edge cases that might exist.
        [Fact(Skip = "TODO")]
        
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Create<ReferenceDataApi.V1.Infrastructure.ReferenceDataDb>();
            var entity = databaseEntity.ToDomain();

            databaseEntity.Id.Should().Be(entity.Id);
            databaseEntity.Category.Should().Be(entity.Category);
        }

        //TODO: add assertions for all the fields being mapped in `EntityFactory.ToDatabase()`. Also be sure to add test cases for
        // any edge cases that might exist.
        [Fact(Skip = "TODO")]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<ReferenceDataApi.V1.Domain.ReferenceData>();
            var databaseEntity = entity.ToDatabase();

            entity.Id.Should().Be(databaseEntity.Id);
            entity.Category.Should().Be(databaseEntity.Category);
        }
    }
}
