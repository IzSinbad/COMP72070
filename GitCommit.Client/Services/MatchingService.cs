using System;
using System.Net.Http;
using System.Threading.Tasks;
using GitCommit.Shared.Interfaces;
using GitCommit.Shared.Models;
using GitCommit.Shared.Utilities;

namespace GitCommit.Client.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _logFilePath;
        private readonly string _token;

        public MatchingService(HttpClient httpClient, string baseUrl, string logFilePath, string token)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _logFilePath = logFilePath;
            _token = token;
        }

        public async Task<bool> LikeProfile(LikeAction likeAction)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, likeAction);
                var response = await _httpClient.PostAsync<dynamic>($"{_baseUrl}/api/matching/like", likeAction, _token);
                Logger.LogReceive(_logFilePath, response);
                
                // Check if it's a match
                if (response.IsMatch)
                {
                    // Log the match
                    Logger.LogReceive(_logFilePath, new { Match = response.Match });
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return false;
            }
        }

        public async Task<bool> DislikeProfile(DislikeAction dislikeAction)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, dislikeAction);
                var response = await _httpClient.PostAsync<dynamic>($"{_baseUrl}/api/matching/dislike", dislikeAction, _token);
                Logger.LogReceive(_logFilePath, response);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return false;
            }
        }

        public async Task<MatchesResponse> GetMatches(int userId)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, new { Action = "GetMatches", UserId = userId });
                var response = await _httpClient.GetAsync<MatchesResponse>($"{_baseUrl}/api/matching/matches/{userId}", _token);
                Logger.LogReceive(_logFilePath, response);
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return new MatchesResponse();
            }
        }
    }
}
