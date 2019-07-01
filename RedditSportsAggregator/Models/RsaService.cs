using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedditSportsAggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace RedditSportsAggregator.Models
{
    public class RsaService
    {
        private readonly RsaContext _context;

        public RsaService(RsaContext context)
        {
            _context = context;
        }

        public List<Sport> GetSports()
        {
            return _context.Sports;
        }

        public List<League> GetLeagues()
        {
            return _context.Leagues;
        }

        public League GetLeague(string league)
        {
            return GetLeagues().Where(l => l.Name.ToLower() == league.ToLower()).FirstOrDefault();
        }

        public List<League> GetLeaguesBySport(string sport)
        {
            return _context.Leagues.Where(l => l.Sport.Name.ToLower() == sport.ToLower()).ToList();
        }

        public List<Game> GetGames(string leagueName)
        {
            List<Game> games = new List<Game>();

            League league = GetLeague(leagueName);
            dynamic jsonObj = GetJsonResponse(league);

            if (jsonObj == null) return games;

            foreach (var obj in jsonObj.data.children)
            {
                // If the flair is Game Thread and the game thread was saved today
                if (obj.data.link_flair_text.ToString() == "Game Thread" &&
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(double.Parse(obj.data.created_utc.ToString()))
                        > DateTime.Today)
                {
                    Game game = new Game();

                    game.Name = obj.data.title;
                    string gameId = obj.data.name;
                    game.GameId = gameId.Split('_').Last();
                    game.CreatedUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        .AddSeconds(double.Parse(obj.data.created_utc.ToString()));
                    game.League = league;

                    games.Add(game);
                }
            }

            return games;
        }

        public List<Post> GetPosts(string leagueName, string gameId)
        {
            string regex = @"(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#\/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[A-Z0-9+&@#\/%=~_|$])";

            List<Post> posts = new List<Post>();

            League league = GetLeague(leagueName);
            dynamic jsonObj = GetJsonResponse(league, gameId);

            if (jsonObj == null) return posts;

            foreach (var obj in jsonObj.data.children)
            {
                if (Regex.Match(obj.data.body.ToString(), regex).Success)
                {
                    Post post = new Post();
                    MatchCollection matches = Regex.Matches(obj.data.body.ToString(), regex);

                    post.Author = obj.data.author;

                    foreach(Match match in matches)
                    {
                        post.Streams.Add(match.Value);
                    }

                    post.Game = new Game
                    {
                        GameId = gameId,
                        Name = obj.data.children[0].title,
                        CreatedUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(double.Parse(obj.data.children[0].created_utc.ToString())),
                        League = league
                    };

                    posts.Add(post);
                }
            }

            return posts;
        }

        private dynamic GetJsonResponse(League league, string gameId = null)
        {
            var url = gameId == null ? league.Url : league.Url + $"/comments/{gameId}/";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject(
                        response.Content.ReadAsStringAsync().Result);
                }
            }

            return null;
        }
    }
}