﻿using System;
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
			get => Sports.SelectMany(s => s.Leagues).ToList();
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

			Sports.Add(new Sport
			{
				Name = "Football"
			});
		}

		private void AddLeagues()
		{
			// Add baseball leagues
			var mlb = new League
			{
				Name = "MLB",
				Url = "https://www.reddit.com/r/MLBStreams",
				Sport = Sports.Where(s => s.Name == "Baseball").FirstOrDefault()
			};
			Sports.Where(s => s.Name == "Baseball").FirstOrDefault().Leagues.Add(mlb);

			// Add hockey leagues
			var nhl = new League
			{
				Name = "NHL",
				Url = "https://www.reddit.com/r/NHLStreams",
				Sport = Sports.Where(s => s.Name == "Hockey").FirstOrDefault()
			};
			Sports.Where(s => s.Name == "Hockey").FirstOrDefault().Leagues.Add(nhl);

			// Add football leagues
			var nfl = new League
			{
				Name = "NFL",
				Url = "https://www.reddit.com/r/nflstreams",
				Sport = Sports.Where(s => s.Name == "Football").FirstOrDefault()
			};
			Sports.Where(s => s.Name == "Football").FirstOrDefault().Leagues.Add(nfl);
		}
	}
}