using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedditSportsAggregator.Models.Json
{
    public class ChildJsonObj
    {
        [JsonProperty("data")]
        public ChildDataJsonObj ChildData { get; set; }
    }
}