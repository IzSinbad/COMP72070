using System;
using System.Collections.Generic;

namespace GitCommit.Shared.Models
{
    public class LikeAction
    {
        public int LikeId { get; set; }
        public int LikerId { get; set; }
        public int LikedId { get; set; }
        public DateTime LikedAt { get; set; }
    }

    public class DislikeAction
    {
        public int DislikeId { get; set; }
        public int DislikerId { get; set; }
        public int DislikedId { get; set; }
        public DateTime DislikedAt { get; set; }
    }

    public class Match
    {
        public int MatchId { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime MatchedAt { get; set; }
        public User MatchedUser { get; set; } // This will be populated with the other user's info when sending to client
    }

    public class MatchesResponse
    {
        public List<Match> Matches { get; set; } = new List<Match>();
    }

    public class ProfileResponse
    {
        public User User { get; set; }
    }

    public class MatchNotification
    {
        public Match Match { get; set; }
    }
}
