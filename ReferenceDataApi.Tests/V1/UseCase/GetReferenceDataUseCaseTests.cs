using AutoFixture;
using FluentAssertions;
using Moq;
using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.UseCase;
using System.Linq;
using Xunit;

namespace ReferenceDataApi.Tests.V1.UseCase
{
    public class GetReferenceDataUseCaseTests
    {
        private readonly Mock<IReferenceDataGateway> _mockGateway;
        private readonly GetReferenceDataUseCase _classUnderTest;
        private readonly Fixture _fixture;

        public GetReferenceDataUseCaseTests()
        {
            _mockGateway = new Mock<IReferenceDataGateway>();
            _classUnderTest = new GetReferenceDataUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public void GetsAllFromTheGateway()
        {
            var stubbedEntities = _fixture.CreateMany<ReferenceData>().ToList();
            _mockGateway.Setup(x => x.GetAll()).Returns(stubbedEntities);

            var expectedResponse = new ResponseObjectList { ResponseObjects = stubbedEntities.ToResponse() };

            _classUnderTest.Execute().Should().BeEquivalentTo(expectedResponse);
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
