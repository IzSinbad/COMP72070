using System;
using System.IO;
using System.Windows;

namespace GitCommit.ServerAdmin
{
    public partial class App : Application
    {
        public static string LogDirectory { get; private set; }
        public static string BaseUrl { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize log directory
            LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(LogDirectory);

            // Initialize server URL
            BaseUrl = "https://localhost:7001"; // Default URL, can be changed in settings
        }
    }
}
