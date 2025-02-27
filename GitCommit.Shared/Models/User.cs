using System;
using System.Collections.Generic;

namespace GitCommit.Shared.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public List<UserImage> Images { get; set; } = new List<UserImage>();
        public UserPreferences Preferences { get; set; } = new UserPreferences();
        public UserStatus Status { get; set; } = UserStatus.Offline;
    }

    public class UserImage
    {
        public int ImageId { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class UserPreferences
    {
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public List<string> PreferredGenders { get; set; } = new List<string>();
        public int MinHeight { get; set; } = 0;
        public int MaxHeight { get; set; } = 300;
        // Additional preferences can be added here
    }

    public enum UserStatus
    {
        Active,
        Paused,
        Offline
    }
}
