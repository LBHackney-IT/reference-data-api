using ReferenceDataApi.Tests.V1.E2ETests.Steps;
using System;
using TestStack.BDDfy;
using Xunit;

namespace ReferenceDataApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Service",
        IWant = "an endpoint to return reference data details",
        SoThat = "it is possible to retrieve reference data from a central location")]
    [Collection("ElasticSearch collection")]
    public class GetReferenceDataTests : IDisposable
    {
        private readonly ElasticSearchFixture<Startup> _testFixture;
        private readonly GetReferenceDataSteps _steps;

        public GetReferenceDataTests(ElasticSearchFixture<Startup> testFixture)
        {
            _testFixture = testFixture;
            _steps = new GetReferenceDataSteps(_testFixture.Client);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void ServiceReturnsTheRequestedReferenceDataForCategoryAndSubCategory(bool? includeInactive)
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Category, _testFixture.SubCategory, includeInactive))
                .Then(t => _steps.ThenTheSubCategoryReferenceDataIsReturned(_testFixture, _testFixture.Category, _testFixture.SubCategory, includeInactive))
                .BDDfy();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void ServiceReturnsTheRequestedReferenceDataForCategory(bool? includeInactive)
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Category, includeInactive))
                .Then(t => _steps.ThenTheCategoryReferenceDataIsReturned(_testFixture, _testFixture.Category, includeInactive))
                .BDDfy();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void ServiceReturnsNoResultsWhenCategoryAndSubCategoryNotFound(bool? includeInactive)
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Category, _testFixture.Unknown, includeInactive))
                .Then(t => _steps.ThenNoDataIsReturned())
                .BDDfy();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void ServiceReturnsNoResultsWhenCategoryNotFound(bool? includeInactive)
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Unknown, includeInactive))
                .Then(t => _steps.ThenNoDataIsReturned())
                .BDDfy();
        }
    }
}
