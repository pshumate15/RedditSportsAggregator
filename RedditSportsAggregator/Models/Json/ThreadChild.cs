using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedditSportsAggregator.Models.Json
{
    public class ThreadChild
    {
        [JsonProperty("data")]
        public ThreadChildData ChildData { get; set; }
    }
}