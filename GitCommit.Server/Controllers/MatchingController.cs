using System;
using System.Collections.Generic;
using System.Linq;
using GitCommit.Shared.Models;
using GitCommit.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GitCommit.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MatchingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _logFilePath;
        private readonly string _matchLogFilePath;
        private static readonly List<LikeAction> _likes = new List<LikeAction>();
        private static readonly List<DislikeAction> _dislikes = new List<DislikeAction>();
        private static readonly List<Match> _matches = new List<Match>();
        private static int _nextLikeId = 1;
        private static int _nextDislikeId = 1;
        private static int _nextMatchId = 1;

        // Reference to users (in a real app, this would be a database)
        private static readonly Dictionary<int, User> _users = new Dictionary<int, User>();

        public MatchingController(IConfiguration configuration)
        {
            _configuration = configuration;
            _logFilePath = _configuration["LogFilePath"] ?? "logs/matching.log";
            _matchLogFilePath = _configuration["MatchLogFilePath"] ?? "logs/matches.log";
        }

        [HttpPost("like")]
        public IActionResult Like([FromBody] LikeAction likeAction)
        {
            Logger.LogReceive(_logFilePath, likeAction);

            // Prevent duplicate likes
            if (_likes.Any(l => l.LikerId == likeAction.LikerId && l.LikedId == likeAction.LikedId))
            {
                return BadRequest(new { Success = false, Message = "You have already liked this profile" });
            }

            likeAction.LikeId = _nextLikeId++;
            likeAction.LikedAt = DateTime.Now;
            _likes.Add(likeAction);

            // Check if there's a match (the other user has already liked this user)
            var otherLike = _likes.FirstOrDefault(l => l.LikerId == likeAction.LikedId && l.LikedId == likeAction.LikerId);
            if (otherLike != null)
            {
                // Create a match
                var match = new Match
                {
                    MatchId = _nextMatchId++,
                    User1Id = likeAction.LikerId,
                    User2Id = likeAction.LikedId,
                    MatchedAt = DateTime.Now
                };

                _matches.Add(match);

                // Log the match
                Logger.LogMatch(_matchLogFilePath, match);

                var response = new { Success = true, IsMatch = true, Match = match };
                Logger.LogTransmit(_logFilePath, response);
                return Ok(response);
            }

            var noMatchResponse = new { Success = true, IsMatch = false };
            Logger.LogTransmit(_logFilePath, noMatchResponse);
            return Ok(noMatchResponse);
        }

        [HttpPost("dislike")]
        public IActionResult Dislike([FromBody] DislikeAction dislikeAction)
        {
            Logger.LogReceive(_logFilePath, dislikeAction);

            // Prevent duplicate dislikes
            if (_dislikes.Any(d => d.DislikerId == dislikeAction.DislikerId && d.DislikedId == dislikeAction.DislikedId))
            {
                return BadRequest(new { Success = false, Message = "You have already disliked this profile" });
            }

            dislikeAction.DislikeId = _nextDislikeId++;
            dislikeAction.DislikedAt = DateTime.Now;
            _dislikes.Add(dislikeAction);

            var response = new { Success = true };
            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        [HttpGet("matches/{userId}")]
        public IActionResult GetMatches(int userId)
        {
            Logger.LogReceive(_logFilePath, new { UserId = userId });

            var userMatches = _matches
                .Where(m => m.User1Id == userId || m.User2Id == userId)
                .ToList();

            // Populate matched user info
            foreach (var match in userMatches)
            {
                int matchedUserId = match.User1Id == userId ? match.User2Id : match.User1Id;
                if (_users.ContainsKey(matchedUserId))
                {
                    match.MatchedUser = _users[matchedUserId];
                }
            }

            var response = new MatchesResponse { Matches = userMatches };
            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        // For admin panel - get all matches
        [HttpGet("all")]
        public IActionResult GetAllMatches()
        {
            var response = new { Matches = _matches, Likes = _likes, Dislikes = _dislikes };
            return Ok(response);
        }

        // For admin panel - get user activity
        [HttpGet("activity/{userId}")]
        public IActionResult GetUserActivity(int userId)
        {
            var likes = _likes.Where(l => l.LikerId == userId).ToList();
            var dislikes = _dislikes.Where(d => d.DislikerId == userId).ToList();
            var matches = _matches.Where(m => m.User1Id == userId || m.User2Id == userId).ToList();

            var response = new { Likes = likes, Dislikes = dislikes, Matches = matches };
            return Ok(response);
        }
    }
}
