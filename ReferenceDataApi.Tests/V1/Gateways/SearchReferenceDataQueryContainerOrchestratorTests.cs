using FluentAssertions;
using Nest;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.Infrastructure;
using System.Linq;
using Xunit;

namespace ReferenceDataApi.Tests.V1.Gateways
{
    public class SearchReferenceDataQueryContainerOrchestratorTests
    {
        private readonly SearchReferenceDataQueryContainerOrchestrator _sut;

        private const string Category = "SomeCategory";

        public SearchReferenceDataQueryContainerOrchestratorTests()
        {
            _sut = new SearchReferenceDataQueryContainerOrchestrator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("SomeSubCategory")]
        public void ShouldReturnQueryThatSearchesCategoryAndSubCategory(string subCategory)
        {
            var apiQuery = new GetReferenceDataQuery
            {
                Category = Category,
                SubCategory = subCategory
            };
            var result = _sut.Create(apiQuery, new QueryContainerDescriptor<QueryableReferenceData>());

            // Assert

            if (!string.IsNullOrEmpty(subCategory))
            {
                (result as IQueryContainer).Bool.Must.Any(x => (x as IQueryContainer).Match.Query == apiQuery.Category).Should().BeTrue();
                (result as IQueryContainer).Bool.Must.Any(x => (x as IQueryContainer).Match.Query == apiQuery.SubCategory).Should().BeTrue();
            }
            else
            {
                (result as IQueryContainer).Match.Query.Should().Be(apiQuery.Category);
            }
        }
    }
}
