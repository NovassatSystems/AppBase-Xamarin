using Newtonsoft.Json;
using System;

namespace Core
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class ApiChachedResult<T>
    {
        [JsonProperty("dtUpdate")]
        public DateTime DtUpdate { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }
    }
}
