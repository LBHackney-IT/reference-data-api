using System;
using System.Collections.Generic;

namespace ReferenceDataApi.V1.Domain
{
    public class ReferenceData
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
