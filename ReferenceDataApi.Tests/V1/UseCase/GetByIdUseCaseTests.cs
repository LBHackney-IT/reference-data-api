using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.UseCase;
using Moq;
using Xunit;

namespace ReferenceDataApi.Tests.V1.UseCase
{
    public class GetByIdUseCaseTests
    {
        private readonly Mock<IExampleGateway> _mockGateway;
        private readonly GetByIdUseCase _classUnderTest;

        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IExampleGateway>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object);
        }

        //TODO: test to check that the use case retrieves the correct record from the database.
        //Guidance on unit testing and example of mocking can be found here https://github.com/LBHackney-IT/lbh-reference-data-api/wiki/Writing-Unit-Tests
    }
}
