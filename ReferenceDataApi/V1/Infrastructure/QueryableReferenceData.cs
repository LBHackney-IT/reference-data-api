using Nest;
using System;
using System.Collections.Generic;

namespace ReferenceDataApi.V1.Infrastructure
{
    public class QueryableReferenceData
    {
        public Guid Id { get; set; }

        [Text(Name = "category")]
        public string Category { get; set; }

        [Text(Name = "subCategory")]
        public string SubCategory { get; set; }

        public string Code { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        [Boolean(Name = "isActive")]
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
