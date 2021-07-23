using ReferenceDataApi.V1.Domain;
using ReferenceDataApi.V1.Infrastructure;

namespace ReferenceDataApi.V1.Factories
{
    public static class EntityFactory
    {
        public static ReferenceData ToDomain(this QueryableReferenceData databaseEntity)
        {
            return new ReferenceData
            {
                Id = databaseEntity.Id,
                Category = databaseEntity.Category,
                SubCategory = databaseEntity.SubCategory,
                Code = databaseEntity.Code,
                CreatedAt = databaseEntity.CreatedAt,
                Description = databaseEntity.Description,
                IsActive = databaseEntity.IsActive,
                Tags = databaseEntity.Tags,
                Value = databaseEntity.Value
            };
        }
    }
}
