using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RedditSportsAggregator.Models
{
    public class League
    {
        public League()
        {
            Games = new List<Game>();
        }

        public string Name { get; set; }
        // URL of league subreddit
        public string Url { get; set; }
        public Sport Sport { get; set; } 
        public List<Game> Games { get; set; }
    }
}