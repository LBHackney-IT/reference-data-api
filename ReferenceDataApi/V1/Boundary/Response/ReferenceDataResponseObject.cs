using ReferenceDataApi.V1.Domain;
using System.Collections.Generic;

namespace ReferenceDataApi.V1.Boundary.Response
{
    public class ReferenceDataResponseObject : Dictionary<string, IEnumerable<ReferenceData>>
    { }
}
