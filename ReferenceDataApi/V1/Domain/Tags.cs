using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReferenceDataApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Tags
    {
        housing
    }
}
