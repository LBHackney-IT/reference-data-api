using ReferenceDataApi.V1.Domain;
using System;

namespace ReferenceDataApi.V1.Infrastructure
{
    public class ReferenceDataDb
    {
        public string Category { get; set; }

        public Guid Id { get; set; }

        public SubCategory SubCategory { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public Tags Tags { get; set; }
    }
}
