using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditSportsAggregator.Models.Json
{
    public class ThreadChildData
    {
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }
        [JsonProperty("created_utc")]
        public double CreatedUtc { get; set; }
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty("link_flair_text")]
        public string LinkFlairText { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
