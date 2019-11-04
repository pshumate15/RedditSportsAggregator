using Newtonsoft.Json;
using RedditSportsAggregator.Models;
using RedditSportsAggregator.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace RedditSportsAggregator.Services
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
            string json = GetJsonResponse(league);
            var thread = JsonConvert.DeserializeObject<Thread>(json);

            if (thread == null) return games;

            foreach (var post in thread.Data.Children)
            {
                if (post.ChildData.LinkFlairText == "Game Thread")
                {
                    Game game = new Game();
					
					// Remove "Game Thread: " from name as it messes with display
                    game.Name = post.ChildData.Title.Replace("Game Thread: ", "");
                    string gameId = post.ChildData.Name;
                    game.GameId = gameId.Split('_').Last();
                    game.CreatedUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        .AddSeconds(post.ChildData.CreatedUtc);
                    game.League = league;

                    games.Add(game);
                }
            }

            return games;
        }

        public List<Post> GetPosts(string leagueName, string gameId)
        {
            string linkFinderRegex = @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";

            List<Post> posts = new List<Post>();

            League league = GetLeague(leagueName);
            string json = GetJsonResponse(league, gameId);
            var thread = JsonConvert.DeserializeObject<List<Thread>>(json);

            if (thread == null) return posts;

            // Loop over comments to find those that have stream links
            foreach (var comment in thread[1].Data.Children)
            {
                // AutoModerator comments do not contain useful links
                if (Regex.Match(comment.ChildData.Body, linkFinderRegex).Success && comment.ChildData.Author != "AutoModerator")
                {
                    Post post = new Post()
                    {
                        Author = comment.ChildData.Author
                    };

                    MatchCollection matches = Regex.Matches(comment.ChildData.Body, linkFinderRegex);

                    foreach (Match match in matches)
                    {
                        post.Streams.Add(match.Value);
                    }

                    post.Game = new Game
                    {
                        GameId = gameId,
                        Name = thread[0].Data.Children[0].ChildData.Title,
                        CreatedUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(thread[0].Data.Children[0].ChildData.CreatedUtc),
                        League = league
                    };

                    posts.Add(post);
                }
            }

            return posts;
        }

        private string GetJsonResponse(League league, string gameId = null)
        {
            var url = gameId == null ? league.Url + ".json" : league.Url + $"/comments/{gameId}.json";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
            }

            return null;
        }
    }
}