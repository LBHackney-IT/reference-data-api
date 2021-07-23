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
            return HasTermExpression(qc.Term, $"field => field.{nameof(QueryableReferenceData.IsActive)}", true);
        }

        private bool HasCategoryFilter(IQueryContainer qc, string val)
        {
            return HasTermExpression(qc.Term, $"field => field.{nameof(QueryableReferenceData.Category)}.Suffix(\"keyword\")", val);
        }

        private bool HasSubCategoryFilter(IQueryContainer qc, string val)
        {
            return HasTermExpression(qc.Term, $"field => field.{nameof(QueryableReferenceData.SubCategory)}.Suffix(\"keyword\")", val);
        }

        private bool HasTermExpression(ITermQuery term, string field, object val)
        {
            if (term is null) return false;

            return (term.Field.Expression.ToString() == field)
                && (term.Value.ToString() == val.ToString());
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
            var qc = (result as IQueryContainer);
            if (!string.IsNullOrEmpty(subCategory))
            {
                qc.Bool.Must.Any(x => HasCategoryFilter(x as IQueryContainer, apiQuery.Category)).Should().BeTrue();
                qc.Bool.Must.Any(x => HasSubCategoryFilter(x as IQueryContainer, apiQuery.SubCategory)).Should().BeTrue();
                qc.Bool.Must.Any(x => HasIsActiveFilter(x as IQueryContainer)).Should().BeTrue();
            }
            else
            {
                qc.Bool.Must.Any(x => HasCategoryFilter(x as IQueryContainer, apiQuery.Category)).Should().BeTrue();
                qc.Bool.Must.Any(x => HasSubCategoryFilter(x as IQueryContainer, apiQuery.SubCategory)).Should().BeFalse();
                qc.Bool.Must.Any(x => HasIsActiveFilter(x as IQueryContainer)).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void ShouldReturnQueryThatSearchesCategory(bool? includeInactive)
        {
            var apiQuery = new GetReferenceDataQuery
            {
                Category = Category,
                IncludeInactive = includeInactive
            };
            var result = _sut.Create(apiQuery, new QueryContainerDescriptor<QueryableReferenceData>());

            // Assert
            var qc = (result as IQueryContainer);
            if (!includeInactive.HasValue || !includeInactive.Value)
            {
                qc.Term.Should().BeNull();
                qc.Bool.Must.Any(x => HasCategoryFilter(x as IQueryContainer, apiQuery.Category)).Should().BeTrue();
                qc.Bool.Must.Any(x => HasSubCategoryFilter(x as IQueryContainer, apiQuery.SubCategory)).Should().BeFalse();
                qc.Bool.Must.Any(x => HasIsActiveFilter(x as IQueryContainer)).Should().BeTrue();
            }
            else
            {
                HasCategoryFilter(qc, apiQuery.Category).Should().BeTrue();
                qc.Bool.Should().BeNull();
            }
        }
    }
}
