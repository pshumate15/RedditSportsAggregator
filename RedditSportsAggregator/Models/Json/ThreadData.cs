using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedditSportsAggregator.Models.Json
{
    public class ThreadData
    {
        [JsonProperty("children")]
        public List<ThreadChild> Children { get; set; }
    }
}