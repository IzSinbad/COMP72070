using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProfileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _logFilePath;
        private static readonly Dictionary<int, User> _users = new Dictionary<int, User>();
        private static readonly Dictionary<int, UserImage> _images = new Dictionary<int, UserImage>();
        private static int _nextImageId = 1;

        // Sample profile pictures for demo
        private static readonly string[] _sampleImagePaths = new[]
        {
            "SampleData/profile1.jpg",
            "SampleData/profile2.jpg",
            "SampleData/profile3.jpg",
            "SampleData/profile4.jpg",
            "SampleData/profile5.jpg"
        };

        public ProfileController(IConfiguration configuration)
        {
            _configuration = configuration;
            _logFilePath = _configuration["LogFilePath"] ?? "logs/profile.log";
            
            // Initialize with sample data if empty
            if (_users.Count == 0)
            {
                InitializeSampleData();
            }
        }

        [HttpGet("{userId}")]
        public IActionResult GetProfile(int userId)
        {
            Logger.LogReceive(_logFilePath, new { UserId = userId });

            if (!_users.ContainsKey(userId))
            {
                return NotFound(new { Success = false, Message = "User not found" });
            }

            var response = new ProfileResponse { User = _users[userId] };
            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        [HttpGet("explore/{userId}")]
        public IActionResult GetProfilesToExplore(int userId)
        {
            Logger.LogReceive(_logFilePath, new { UserId = userId });

            if (!_users.ContainsKey(userId))
            {
                return NotFound(new { Success = false, Message = "User not found" });
            }

            // Get all users except the current user
            var profiles = _users.Values
                .Where(u => u.UserId != userId)
                .ToList();

            Logger.LogTransmit(_logFilePath, profiles);
            return Ok(profiles);
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateProfile(int userId, [FromBody] User user)
        {
            Logger.LogReceive(_logFilePath, user);

            if (userId != user.UserId)
            {
                return BadRequest(new { Success = false, Message = "User ID mismatch" });
            }

            if (!_users.ContainsKey(userId))
            {
                return NotFound(new { Success = false, Message = "User not found" });
            }

            _users[userId] = user;

            var response = new { Success = true, Message = "Profile updated successfully" };
            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        [HttpPut("{userId}/preferences")]
        public IActionResult UpdatePreferences(int userId, [FromBody] UserPreferences preferences)
        {
            Logger.LogReceive(_logFilePath, preferences);

            if (!_users.ContainsKey(userId))
            {
                return NotFound(new { Success = false, Message = "User not found" });
            }

            _users[userId].Preferences = preferences;

            var response = new { Success = true, Message = "Preferences updated successfully" };
            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        [HttpPost("{userId}/images")]
        public IActionResult AddProfileImage(int userId, [FromBody] UserImage image)
        {
            Logger.LogReceive(_logFilePath, new { UserId = userId, ImageId = image.ImageId });

            if (!_users.ContainsKey(userId))
            {
                return NotFound(new { Success = false, Message = "User not found" });
            }

            image.ImageId = _nextImageId++;
            image.UploadedAt = DateTime.Now;
            
            _images[image.ImageId] = image;
            _users[userId].Images.Add(image);

            var response = new { Success = true, ImageId = image.ImageId, Message = "Image added successfully" };
            Logger.LogTransmit(_logFilePath, response);
            return Ok(response);
        }

        [HttpGet("images/{imageId}")]
        public IActionResult GetProfileImage(int imageId)
        {
            Logger.LogReceive(_logFilePath, new { ImageId = imageId });

            if (!_images.ContainsKey(imageId))
            {
                return NotFound(new { Success = false, Message = "Image not found" });
            }

            var image = _images[imageId];
            Logger.LogTransmit(_logFilePath, new { ImageId = imageId, Size = image.ImageData.Length });
            return Ok(image);
        }

        private void InitializeSampleData()
        {
            // Create sample users with profile pictures
            for (int i = 1; i <= 5; i++)
            {
                var user = new User
                {
                    UserId = i,
                    Username = $"user{i}",
                    Bio = $"This is the bio for user {i}",
                    Gender = i % 2 == 0 ? "Female" : "Male",
                    Age = 20 + i,
                    Status = UserStatus.Active
                };

                // Load sample profile picture
                if (i <= _sampleImagePaths.Length)
                {
                    try
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), _sampleImagePaths[i - 1]);
                        if (File.Exists(imagePath))
                        {
                            var imageData = File.ReadAllBytes(imagePath);
                            var image = new UserImage
                            {
                                ImageId = _nextImageId++,
                                ImageData = imageData,
                                UploadedAt = DateTime.Now.AddDays(-i)
                            };
                            
                            _images[image.ImageId] = image;
                            user.Images.Add(image);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading sample image: {ex.Message}");
                    }
                }

                _users[user.UserId] = user;
            }
        }
    }
}
