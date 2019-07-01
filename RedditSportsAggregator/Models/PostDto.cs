using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditSportsAggregator.Models
{
    public class PostDto : LinkedResourceBase
    {
        public string Author { get; set; }
        public List<string> Streams { get; set; }
    }
}
