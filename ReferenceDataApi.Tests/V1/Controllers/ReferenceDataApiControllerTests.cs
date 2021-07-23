using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Controllers;
using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReferenceDataApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class ReferenceDataApiControllerTests
    {
        private readonly Mock<IGetReferenceDataUseCase> _mockUseCase;
        private readonly ReferenceDataApiController _sut;

        private readonly Fixture _fixture = new Fixture();
        private const string Category = "SomeCategory";

        public ReferenceDataApiControllerTests()
        {
            _mockUseCase = new Mock<IGetReferenceDataUseCase>();
            _sut = new ReferenceDataApiController(_mockUseCase.Object);
        }

        private GetReferenceDataQuery CreateQuery()
        {
            return new GetReferenceDataQuery
            {
                Category = Category
            };
        }

        [Fact]
        public async Task GetReferenceDataAsyncReturnsResponse()
        {
            var queryParam = CreateQuery();

            var refData = _fixture.Build<ReferenceData>()
                                  .With(x => x.Category, Category)
                                  .With(x => x.SubCategory, _fixture.Create<string>())
                                  .CreateMany(5);
            var responseList = refData.ToResponse();
            _mockUseCase.Setup(x => x.ExecuteAsync(queryParam)).ReturnsAsync(responseList);

            var response = await _sut.GetReferenceDataAsync(queryParam).ConfigureAwait(false);
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(responseList);
        }

        [Fact]
        public void GetReferenceDataAsyncThrowsException()
        {
            var queryParam = CreateQuery();
            var exception = new ApplicationException("Test Exception");
            _mockUseCase.Setup(x => x.ExecuteAsync(queryParam)).ThrowsAsync(exception);

            Func<Task<IActionResult>> func = async () => await _sut.GetReferenceDataAsync(queryParam).ConfigureAwait(false);
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
