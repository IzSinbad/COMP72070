using System;
using System.Net.Http;
using GitCommit.Shared.Interfaces;

namespace GitCommit.Client.Services
{
    public class ServiceFactory
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _logFilePath;
        private string _token;

        public ServiceFactory(string baseUrl, string logFilePath)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
            _logFilePath = logFilePath;
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public IAuthService CreateAuthService()
        {
            return new AuthService(_httpClient, _baseUrl, _logFilePath);
        }

        public IProfileService CreateProfileService()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new InvalidOperationException("Token is required for profile service. Call SetToken first.");
            }

            return new ProfileService(_httpClient, _baseUrl, _logFilePath, _token);
        }

        public IMatchingService CreateMatchingService()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new InvalidOperationException("Token is required for matching service. Call SetToken first.");
            }

            return new MatchingService(_httpClient, _baseUrl, _logFilePath, _token);
        }
    }
}
