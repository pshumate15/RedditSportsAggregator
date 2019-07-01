using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditSportsAggregator.Models
{
    public class GameDto : LinkedResourceBase
    {
        public string Name { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
