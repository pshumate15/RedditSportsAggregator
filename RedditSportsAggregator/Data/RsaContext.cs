using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RedditSportsAggregator.Models;

namespace RedditSportsAggregator.Models
{
    public class RsaContext
    {
        public RsaContext()
        {
            Sports = new List<Sport>();

            AddSports();
            AddLeagues();
        }

        public List<Sport> Sports { get; set; }
        public List<League> Leagues
        {
            get
            {
                var flatLeagues = new List<League>();

                foreach(List<League> leagueList in Sports.Select(s => s.Leagues).ToList())
                {
                    foreach(League league in leagueList)
                    {
                        flatLeagues.Add(league);
                    }
                }

                return flatLeagues;
            }
        }

        private void AddSports()
        {
            Sports.Add(new Sport
            {
                Name = "Baseball",
            });

            Sports.Add(new Sport
            {
                Name = "Hockey"
            });
        }

        private void AddLeagues()
        {
            // Add baseball leagues
            var mlb = new League
            {
                Name = "Mlb",
                Url = "https://www.reddit.com/r/MLBStreams",
                Sport = Sports.Where(s => s.Name == "Baseball").FirstOrDefault()
            };

            Sports.Where(s => s.Name == "Baseball").FirstOrDefault().Leagues.Add(mlb);

            // Add hockey leagues
            var nhl = new League
            {
                Name = "Nhl",
                Url = "https://www.reddit.com/r/NHLStreams",
                Sport = Sports.Where(s => s.Name == "Hockey").FirstOrDefault()
            };
            Sports.Where(s => s.Name == "Hockey").FirstOrDefault().Leagues.Add(nhl);
        }
    }
}