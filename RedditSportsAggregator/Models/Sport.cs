using System;
using System.Collections.Generic;

namespace RedditSportsAggregator.Models
{
    public class Sport
    {
        public Sport()
        {
            Leagues = new List<League>();
        }

        public string Name { get; set; }
        public List<League> Leagues { get; set; }
    }
}
