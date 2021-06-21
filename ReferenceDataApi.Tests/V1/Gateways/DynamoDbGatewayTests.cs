using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReferenceDataApi.Tests.V1.Helper;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using Xunit;

namespace ReferenceDataApi.Tests.V1.Gateways
{
    [Collection("DynamoDb collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        //private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<DynamoDbGateway>> _logger;
        private readonly IDynamoDBContext _dynamoDb;
        private readonly DynamoDbGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();

        public DynamoDbGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _dynamoDb = dbTestFixture.DynamoDbContext;
            _classUnderTest = new DynamoDbGateway(_dynamoDb, _logger.Object);
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
                foreach (var action in _cleanup)
                    action();

                _disposed = true;
            }
        }

        //[Fact]
        //public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        //{
        //    var response = _classUnderTest.GetEntityById(123);

        //    response.Should().BeNull();
        //}

        //[Fact]
        //public void GetEntityByIdReturnsTheEntityIfItExists()
        //{
        //    var entity = _fixture.Create<ReferenceDataApi.V1.Domain.ReferenceData>();
        //    var dbEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

        //    _dynamoDb.Setup(x => x.LoadAsync<ReferenceDataDb>(entity.Id, default))
        //             .ReturnsAsync(dbEntity);

        //    var response = _classUnderTest.GetEntityById(entity.Id);

        //    _dynamoDb.Verify(x => x.LoadAsync<ReferenceDataApi.V1.Infrastructure.ReferenceDataDb>(entity.Id, default), Times.Once);

        //    entity.Id.Should().Be(response.Id);
        //    entity.CreatedAt.Should().BeSameDateAs(response.CreatedAt);
        //}
    }
}
