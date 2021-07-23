using AutoFixture;
using FluentAssertions;
using Moq;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.UseCase;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReferenceDataApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetReferenceDataUseCaseTests
    {
        private readonly Mock<IReferenceDataGateway> _mockGateway;
        private readonly GetReferenceDataUseCase _classUnderTest;
        private readonly Fixture _fixture;
        private const string Category = "SomeCategory";

        public GetReferenceDataUseCaseTests()
        {
            _mockGateway = new Mock<IReferenceDataGateway>();
            _classUnderTest = new GetReferenceDataUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        private GetReferenceDataQuery CreateQuery()
        {
            return new GetReferenceDataQuery
            {
                Category = Category
            };
        }

        [Fact]
        public async Task ExecuteAsyncReturnsResponse()
        {
            var queryParam = CreateQuery();

            var refData = _fixture.Build<ReferenceData>()
                                  .With(x => x.Category, Category)
                                  .With(x => x.SubCategory, _fixture.Create<string>())
                                  .CreateMany(5);
            _mockGateway.Setup(x => x.GetReferenceDataAsync(queryParam)).ReturnsAsync(refData);

            var response = await _classUnderTest.ExecuteAsync(queryParam).ConfigureAwait(false);

            var expected = refData.ToResponse();
            response.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ExecuteAsyncThrowsException()
        {
            var queryParam = CreateQuery();
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.GetReferenceDataAsync(queryParam)).ThrowsAsync(exception);

            Func<Task<ReferenceDataResponseObject>> func = async ()
                => await _classUnderTest.ExecuteAsync(queryParam).ConfigureAwait(false);
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
