using Microsoft.AspNetCore.Mvc;

namespace ReferenceDataApi.V1.Boundary.Request
{
    public class GetReferenceDataQuery
    {
        [FromQuery]
        public string Category { get; set; }
        [FromQuery]
        public string SubCategory { get; set; }
        [FromQuery]
        public bool? IncludeInactive { get; set; }
    }
}
