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

        private bool HasIsActiveFilter(IQueryContainer qc)
        {
            if (qc.Term is null) return false;

            return (qc.Term.Field.Expression.ToString() == $"m => m.{nameof(QueryableReferenceData.IsActive)}")
                && (bool) qc.Term.Value;
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
                (result as IQueryContainer).Match.Should().BeNull();
                (result as IQueryContainer).Bool.Must.Any(x => (x as IQueryContainer).Match.Query == apiQuery.Category).Should().BeTrue();
                (result as IQueryContainer).Bool.Must.Any(x => HasIsActiveFilter(x as IQueryContainer)).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void ShouldReturnQueryThatSearchesCategoryAndInactive(bool? includeInactive)
        {
            var apiQuery = new GetReferenceDataQuery
            {
                Category = Category,
                IncludeInactive = includeInactive
            };
            var result = _sut.Create(apiQuery, new QueryContainerDescriptor<QueryableReferenceData>());

            // Assert
            if (!includeInactive.HasValue || !includeInactive.Value)
            {
                (result as IQueryContainer).Match.Should().BeNull();
                (result as IQueryContainer).Bool.Must.Any(x => (x as IQueryContainer).Match.Query == apiQuery.Category).Should().BeTrue();
                (result as IQueryContainer).Bool.Must.Any(x => HasIsActiveFilter(x as IQueryContainer)).Should().BeTrue();
            }
            else
            {
                (result as IQueryContainer).Match.Query.Should().Be(apiQuery.Category);
                (result as IQueryContainer).Bool.Should().BeNull();
            }
        }
    }
}
