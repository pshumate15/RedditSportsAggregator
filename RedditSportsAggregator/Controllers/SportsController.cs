using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RedditSportsAggregator.Models;

namespace RedditSportsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportsController : ControllerBase
    {
        private readonly RsaService _rsaService;
        private readonly IUrlHelper _urlHelper;

        public SportsController(RsaService rsaService, IUrlHelper urlHelper/*LinkGenerator linkGenerator*/)
        {
            _rsaService = rsaService;
            //_linkGenerator = linkGenerator;
            _urlHelper = urlHelper;
        }
        
        // GET: api/sports
        [HttpGet(Name = "GetSports")]
        public ActionResult<List<SportDto>> GetSports()
        {
            List<SportDto> sportDtos = _rsaService.GetSports().Select(s => CreateSportDtoWithLinks(s)).ToList();

            return sportDtos;
        }

        // GET: api/sports/{sport}/leagues
        [HttpGet("{sport}/leagues", Name ="GetLeagues")]
        public ActionResult<List<LeagueDto>> GetLeagues(string sport)
        {
            if (!_rsaService.GetSports().Any(s => s.Name.ToLower() == sport.ToLower()))
            {
                return BadRequest();
            }

            List<LeagueDto> leagueDtos = _rsaService.GetLeaguesBySport(sport).Select(l => CreateLeagueDtoWithLinks(l)).ToList();

            return leagueDtos;
        }

        // GET: api/sports/{sport}/leagues/{league}/games
        [HttpGet("{sport}/leagues/{league}/games", Name = "GetGames")]
        public ActionResult<List<GameDto>> GetGames(string sport, string league)
        {
            if (!_rsaService.GetSports().Any(s => s.Name.ToLower() == sport.ToLower()) ||
                !_rsaService.GetLeagues().Any(l => l.Name.ToLower() == league.ToLower()))
            {
                return BadRequest();
            }

            List<GameDto> gameDtos = _rsaService.GetGames(league).Select(g => CreateGameDtoWithLinks(g)).ToList();

            return gameDtos;
        }

        // GET: api/sports/{sport}/leagues/{league}/games/game/{gameId}"
        [HttpGet("{sport}/leagues/{league}/games/game/{gameId}", Name = "GetPosts")]
        public ActionResult<List<PostDto>> GetPosts(string sport, string league, string gameId)
        {
            if (!_rsaService.GetSports().Any(s => s.Name.ToLower() == sport.ToLower()) ||
                !_rsaService.GetLeagues().Any(l => l.Name.ToLower() == league.ToLower()))
            {
                return BadRequest();
            }

            List<PostDto> postDtos = _rsaService.GetPosts(league, gameId).Select(p => CreatePostDtoWithLinks(p)).ToList();

            return postDtos;
        }

        private SportDto CreateSportDtoWithLinks(Sport sport)
        {
            SportDto sportDto = new SportDto
            {
                Name = sport.Name
            };

            sportDto.Links.Add(new Link(
                _urlHelper.Link(nameof(GetLeagues),
                    new { sport = sport.Name }).ToLower(),
                "leagues",
                "GET"));

            return sportDto;
        }

        private LeagueDto CreateLeagueDtoWithLinks(League league)
        {
            LeagueDto leagueDto = new LeagueDto
            {
                Name = league.Name
            };

            leagueDto.Links.Add(new Link(
                _urlHelper.Link(nameof(GetGames),
                new { sport = league.Sport.Name, league = league.Name }).ToLower(),
                "games",
                "GET"));

            return leagueDto;
        }

        private GameDto CreateGameDtoWithLinks(Game game)
        {
            GameDto gameDto = new GameDto
            {
                Name = game.Name,
                CreatedUtc = game.CreatedUtc
            };

            gameDto.Links.Add(new Link(
                _urlHelper.Link(nameof(GetPosts), 
                    new { sport = game.League.Sport.Name, league = game.League.Name, gameId = game.GameId }).ToLower(),
                "posts",
                "GET"));

            return gameDto;
        }

        private PostDto CreatePostDtoWithLinks(Post post)
        {
            PostDto postDto = new PostDto
            {
                Author = post.Author,
                Streams = post.Streams
            };

            return postDto;
        }
    }
}