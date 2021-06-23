using FluentValidation.TestHelper;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Boundary.Request.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ReferenceDataApi.Tests.V1.Boundary.Request.Validation
{
    public class GetReferenceDataQueryValidatorTests
    {
        private readonly GetReferenceDataQueryValidator _sut;

        public GetReferenceDataQueryValidatorTests()
        {
            _sut = new GetReferenceDataQueryValidator();
        }

        private static GetReferenceDataQuery CreateValidQuery(string subCategory = null)
        {
            return new GetReferenceDataQuery()
            {
                Category = "Something",
                SubCategory = subCategory
            };
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("SomeSubCategory")]
        public void ValidQueryShouldNotError(string subCategory)
        {
            var query = CreateValidQuery(subCategory);
            var result = _sut.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Something<sometag>")]
        public void QueryShouldErrorWithInvalidCategory(string invalid)
        {
            var query = CreateValidQuery();
            query.Category = invalid;
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Category);
        }

        [Fact]
        public void QueryShouldErrorWithInvalidSubCategory()
        {
            var query = CreateValidQuery("Something <sometag>");
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.SubCategory);
        }
    }
}
