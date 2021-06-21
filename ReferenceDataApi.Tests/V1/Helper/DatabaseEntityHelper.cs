using AutoFixture;
using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Factories;
using ReferenceDataApi.V1.Infrastructure;

namespace ReferenceDataApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static ReferenceDataDb CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<ReferenceData>();
            return entity.ToDatabase();
        }
    }
}
