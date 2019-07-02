using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditSportsAggregator.Models.Json
{
    public class Thread
    {
        [JsonProperty("data")]
        public ThreadData Data { get; set; }
    }
}
