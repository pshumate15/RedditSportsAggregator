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
            var parentJsonObj = JsonConvert.DeserializeObject<ParentJsonObj>(json);

            if (parentJsonObj == null) return games;

            foreach (var post in parentJsonObj.Data.Children)
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
			List<string> invalidAuthors = new List<string>
			{
				"AutoModerator"
			};

			List<Post> posts = new List<Post>();

            League league = GetLeague(leagueName);
            string json = GetJsonResponse(league, gameId);
            var parentJsonObjs = JsonConvert.DeserializeObject<List<ParentJsonObj>>(json);

            if (parentJsonObjs == null) return posts;

			// Filter deserialized response to get comments that do not contain an invalid author
			var comments = parentJsonObjs[1].Data.Children;
			comments = comments.Where(c => !invalidAuthors.Contains(c.ChildData.Author)).ToList();

            // Loop over comments to find those that have stream links
            foreach (var comment in comments)
            {
				MatchCollection matches = Regex.Matches(comment.ChildData.Body, linkFinderRegex);

				if (matches.Count > 0)
                {
                    Post post = new Post()
                    {
                        Author = comment.ChildData.Author
                    };

                    foreach (Match match in matches)
                    {
                        post.Streams.Add(match.Value);
                    }

                    post.Game = new Game
                    {
                        GameId = gameId,
                        Name = parentJsonObjs[0].Data.Children[0].ChildData.Title,
                        CreatedUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(parentJsonObjs[0].Data.Children[0].ChildData.CreatedUtc),
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
				// Need to implement async properly
				var response = client.GetAsync(url).Result; // .Get() does not exist. Have to use async

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result; // ReadAsString() does not exist. Have to use async
                }
            }

            return null;
        }
    }
}