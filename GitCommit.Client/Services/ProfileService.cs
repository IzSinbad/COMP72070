using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GitCommit.Shared.Interfaces;
using GitCommit.Shared.Models;
using GitCommit.Shared.Utilities;

namespace GitCommit.Client.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _logFilePath;
        private readonly string _token;

        public ProfileService(HttpClient httpClient, string baseUrl, string logFilePath, string token)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _logFilePath = logFilePath;
            _token = token;
        }

        public async Task<ProfileResponse> GetProfile(int userId)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, new { Action = "GetProfile", UserId = userId });
                var response = await _httpClient.GetAsync<ProfileResponse>($"{_baseUrl}/api/profile/{userId}", _token);
                Logger.LogReceive(_logFilePath, response);
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return new ProfileResponse
                {
                    User = null
                };
            }
        }

        public async Task<List<User>> GetProfilesToExplore(int userId)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, new { Action = "GetProfilesToExplore", UserId = userId });
                var response = await _httpClient.GetAsync<List<User>>($"{_baseUrl}/api/profile/explore/{userId}", _token);
                Logger.LogReceive(_logFilePath, response);
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return new List<User>();
            }
        }

        public async Task<bool> UpdateProfile(User user)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, user);
                var response = await _httpClient.PutAsync<dynamic>($"{_baseUrl}/api/profile/{user.UserId}", user, _token);
                Logger.LogReceive(_logFilePath, response);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return false;
            }
        }

        public async Task<bool> UpdatePreferences(int userId, UserPreferences preferences)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, preferences);
                var response = await _httpClient.PutAsync<dynamic>($"{_baseUrl}/api/profile/{userId}/preferences", preferences, _token);
                Logger.LogReceive(_logFilePath, response);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return false;
            }
        }

        public async Task<bool> AddProfileImage(int userId, UserImage image)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, new { UserId = userId, ImageSize = image.ImageData.Length });
                var response = await _httpClient.PostAsync<dynamic>($"{_baseUrl}/api/profile/{userId}/images", image, _token);
                Logger.LogReceive(_logFilePath, response);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return false;
            }
        }

        public async Task<UserImage> GetProfileImage(int imageId)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, new { Action = "GetProfileImage", ImageId = imageId });
                var response = await _httpClient.GetAsync<UserImage>($"{_baseUrl}/api/profile/images/{imageId}", _token);
                Logger.LogReceive(_logFilePath, new { ImageId = imageId, ImageSize = response.ImageData.Length });
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return null;
            }
        }
    }
}
