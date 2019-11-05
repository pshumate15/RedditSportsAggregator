using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedditSportsAggregator.Models.Json
{
    public class ParentDataJsonObj
    {
        [JsonProperty("children")]
        public List<ChildJsonObj> Children { get; set; }
    }
}