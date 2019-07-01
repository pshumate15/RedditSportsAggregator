using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditSportsAggregator.Models
{
    public abstract class LinkedResourceBase
    {
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
