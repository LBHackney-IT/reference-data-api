using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ReferenceDataApi.Versioning;
using Xunit;

namespace ReferenceDataApi.Tests.Versioning
{
    public class ApiVersionDescriptionExtensionsTests
    {
        [Fact]
        public void GetFormattedApiVersionTest()
        {
            var version = new ApiVersion(1, 1);
            ApiVersionDescription sut = new ApiVersionDescription(version, null, false);
            sut.GetFormattedApiVersion().Should().Be($"v{version.ToString()}");
        }
    }
}
