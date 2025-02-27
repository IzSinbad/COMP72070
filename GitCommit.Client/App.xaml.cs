using System;
using System.IO;
using System.Windows;
using GitCommit.Client.Services;

namespace GitCommit.Client
{
    public partial class App : Application
    {
        public static ServiceFactory ServiceFactory { get; private set; }
        public static string LogDirectory { get; private set; }
        public static string BaseUrl { get; private set; }
        public static User CurrentUser { get; set; }
        public static string AuthToken { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize log directory
            LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(LogDirectory);

            // Initialize service factory
            BaseUrl = "https://localhost:7001"; // Default URL, can be changed in settings
            ServiceFactory = new ServiceFactory(BaseUrl, Path.Combine(LogDirectory, "client.log"));
        }
    }

    // Simple User class for App-level storage
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
