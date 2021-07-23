using AutoFixture;
using FluentAssertions;
using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Factories;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ReferenceDataApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void CanMapADomainEntityCollectionToAResponseObject(int subCategoriesCount)
        {
            List<ReferenceData> domainRefData = new List<ReferenceData>();
            for (int i = 1; i <= subCategoriesCount; i++)
            {
                domainRefData.AddRange(_fixture.Build<ReferenceData>()
                                               .With(x => x.SubCategory, _fixture.Create<string>())
                                               .CreateMany(5));
            }

            var responseObject = domainRefData.ToResponse();

            var actualSubCategoryKeysList = responseObject.Keys.Select(x => x);
            var expectedSubCategoryKeysList = domainRefData.OrderBy(y => y.SubCategory).Select(x => x.SubCategory).Distinct();
            actualSubCategoryKeysList.Should().BeEquivalentTo(expectedSubCategoryKeysList, config => config.WithStrictOrdering());

            foreach (var subCategory in responseObject)
            {
                var expectedSubCategoryObjects = domainRefData.Where(x => x.SubCategory == subCategory.Key).OrderBy(y => y.Value);
                subCategory.Value.Should().BeEquivalentTo(expectedSubCategoryObjects, config => config.WithStrictOrdering());
            }
        }
    }
}
