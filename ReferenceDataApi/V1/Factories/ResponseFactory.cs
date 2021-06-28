using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.Domain;

namespace ReferenceDataApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ReferenceDataResponseObject ToResponse(this ReferenceData domain)
        {
            if (domain == null) return null;

            return new ReferenceDataResponseObject
            {
                SubCategory = domain.SubCategory
            };
        }
        public static List<ReferenceDataResponseObject> ToResponse(this IEnumerable<ReferenceData> domainList)
        {
            if (null == domainList) return new List<ReferenceDataResponseObject>();
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
