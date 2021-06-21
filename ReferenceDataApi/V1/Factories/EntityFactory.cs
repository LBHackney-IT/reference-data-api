using ReferenceDataApi.V1.Domain;

namespace ReferenceDataApi.V1.Factories
{
    public static class EntityFactory
    {
        public static ReferenceData ToDomain(this Infrastructure.ReferenceDataDb databaseEntity)
        {
            //TODO: Map the rest of the fields in the domain object.

            return new ReferenceData
            {
                Id = databaseEntity.Id,
                Category = databaseEntity.Category
            };
        }

        public static Infrastructure.ReferenceDataDb ToDatabase(this Domain.ReferenceData entity)
        {
            //TODO: Map the rest of the fields in the database object.

            return new Infrastructure.ReferenceDataDb
            {
                Id = entity.Id,
                Category = entity.Category
            };
        }
    }
}
