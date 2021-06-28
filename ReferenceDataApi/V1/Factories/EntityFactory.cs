using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Infrastructure;

namespace ReferenceDataApi.V1.Factories
{
    public static class EntityFactory
    {
        public static ReferenceData ToDomain(this ReferenceDataDb databaseEntity)
        {

            return new ReferenceData
            {
                Id = databaseEntity.Id,
                Category = databaseEntity.Category,
                SubCategory = databaseEntity.SubCategory.ReferenceData.SubCategory,
                Code = databaseEntity.Code,
                CreatedAt = databaseEntity.CreatedAt,
                Description = databaseEntity.Description,
                IsActive = databaseEntity.IsActive,
                Tags = databaseEntity.Tags,
                Value = databaseEntity.Value

            };
        }

        public static ReferenceDataDb ToDatabase(this ReferenceData entity)
        {

            return new ReferenceDataDb
            {
                Id = entity.Id,
                Category = entity.Category,
                Value = entity.Value,
                Tags = entity.Tags,
                IsActive = entity.IsActive,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                Code = entity.Code,
                SubCategory = entity.SubCategory.ReferenceData.SubCategory
            };
        }
    }
}
