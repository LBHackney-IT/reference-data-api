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

        [Fact]
        public void ServiceReturnsTheRequestedReferenceDataForCategoryAndSubCategory()
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Category, _testFixture.SubCategory))
                .Then(t => _steps.ThenTheSubCategoryReferenceDataIsReturned(_testFixture, _testFixture.Category, _testFixture.SubCategory))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsTheRequestedReferenceDataForCategory()
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Category))
                .Then(t => _steps.ThenTheCategoryReferenceDataIsReturned(_testFixture, _testFixture.Category))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNoResultsWhenCategoryAndSubCategoryNotFound()
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Category, _testFixture.Unknown))
                .Then(t => _steps.ThenNoDataIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNoResultsWhenCategoryNotFound()
        {
            this.Given(g => _testFixture.GivenDataAlreadyExists())
                .When(w => _steps.WhenReferenceDataIsRequested(_testFixture.Unknown))
                .Then(t => _steps.ThenNoDataIsReturned())
                .BDDfy();
        }
    }
}
