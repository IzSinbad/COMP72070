using System;
using System.Net.Http;
using System.Threading.Tasks;
using GitCommit.Shared.Interfaces;
using GitCommit.Shared.Models;
using GitCommit.Shared.Utilities;

namespace GitCommit.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _logFilePath;

        public AuthService(HttpClient httpClient, string baseUrl, string logFilePath)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _logFilePath = logFilePath;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, request);
                var response = await _httpClient.PostAsync<LoginResponse>($"{_baseUrl}/api/auth/login", request);
                Logger.LogReceive(_logFilePath, response);
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, request);
                var response = await _httpClient.PostAsync<RegisterResponse>($"{_baseUrl}/api/auth/register", request);
                Logger.LogReceive(_logFilePath, response);
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> Logout(string token)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, new { Action = "Logout" });
                var response = await _httpClient.PostAsync<dynamic>($"{_baseUrl}/api/auth/logout", null, token);
                Logger.LogReceive(_logFilePath, response);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return false;
            }
        }

        public async Task<bool> UpdateUserStatus(int userId, UserStatus status)
        {
            try
            {
                Logger.LogTransmit(_logFilePath, new { UserId = userId, Status = status });
                var response = await _httpClient.PutAsync<dynamic>($"{_baseUrl}/api/auth/status", status);
                Logger.LogReceive(_logFilePath, response);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogReceive(_logFilePath, new { Error = ex.Message });
                return false;
            }
        }
    }
}
