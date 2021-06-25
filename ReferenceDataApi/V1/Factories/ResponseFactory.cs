using System.Collections.Generic;
using System.Linq;
using ReferenceDataApi.V1.Boundary.Response;
using ReferenceDataApi.V1.Domain;

namespace ReferenceDataApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ReferenceDataResponseObject ToResponse(this IEnumerable<ReferenceData> refDataList)
        {
            var response = new ReferenceDataResponseObject();
            foreach (var subCategory in refDataList.OrderBy(y => y.SubCategory).Select(x => x.SubCategory).Distinct())
            {
                response.Add(subCategory, refDataList.Where(x => x.SubCategory == subCategory)
                                                     .OrderBy(y => y.Value)
                                                     .ToList());
            }
            return response;
        }
    }
}
