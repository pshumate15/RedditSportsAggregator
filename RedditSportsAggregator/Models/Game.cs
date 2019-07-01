using System;
using System.Collections.Generic;

namespace RedditSportsAggregator.Models
{
    public class Game
    {
        public Game()
        {
            Posts = new List<Post>();
        }

        public string GameId { get; set; }
        public string Name { get; set; }
        // URL of game thread
        //public string Url { get; set; }
        public DateTime CreatedUtc { get; set; }
        public League League { get; set; }
        public List<Post> Posts { get; set; }
    }
}