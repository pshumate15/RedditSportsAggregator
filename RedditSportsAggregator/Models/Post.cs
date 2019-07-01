using System;
using System.Collections.Generic;

namespace RedditSportsAggregator.Models
{
    public class Post
    {
        public Post()
        {
            Streams = new List<string>();
        }

        public string Author { get; set; }
        public List<string> Streams { get; set; }
        public Game Game { get; set; }
    }
}