using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GitCommit.Shared.Models;
using GitCommit.Shared.Utilities;

namespace GitCommit.ServerAdmin
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string _baseUrl = "https://localhost:7001";
        private string _logDirectory = "logs";
        private bool _autoExportLogs = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load settings
                LoadSettings();

                // Initialize UI
                dpLogDate.SelectedDate = DateTime.Today;

                // Load dashboard data
                await LoadDashboardData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                // In a real application, these would be loaded from a settings file
                txtServerUrl.Text = _baseUrl;
                txtLogDirectory.Text = _logDirectory;
                chkAutoExportLogs.IsChecked = _autoExportLogs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadDashboardData()
        {
            try
            {
                // Show loading state
                txtServerStatus.Text = "Connecting...";
                txtServerStatus.Foreground = System.Windows.Media.Brushes.Orange;

                // Get users
                var users = await GetAllUsers();
                txtTotalUsers.Text = users.Count.ToString();
                txtActiveUsers.Text = users.Count(u => u.Status == UserStatus.Active).ToString();

                // Get matches
                var matches = await GetAllMatches();
                txtTotalMatches.Text = matches.Count.ToString();
                txtTodayMatches.Text = matches.Count(m => m.MatchedAt.Date == DateTime.Today).ToString();

                // Load recent activity
                await LoadRecentActivity();

                // Update server status
                txtServerStatus.Text = "Running";
                txtServerStatus.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }
            catch (Exception ex)
            {
                txtServerStatus.Text = "Offline";
                txtServerStatus.Foreground = System.Windows.Media.Brushes.Red;
                MessageBox.Show($"Error connecting to server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadRecentActivity()
        {
            try
            {
                // Clear existing items
                lvRecentActivity.Items.Clear();

                // In a real application, this would load from the server
                // For this demo, we'll create some sample data
                var activities = new List<ActivityItem>
                {
                    new ActivityItem { Time = DateTime.Now.AddMinutes(-5), Action = "Login", User = "user1", Details = "Successful login" },
                    new ActivityItem { Time = DateTime.Now.AddMinutes(-10), Action = "Match", User = "user2", Details = "Matched with user3" },
                    new ActivityItem { Time = DateTime.Now.AddMinutes(-15), Action = "Like", User = "user4", Details = "Liked user5" },
                    new ActivityItem { Time = DateTime.Now.AddMinutes(-20), Action = "Register", User = "user6", Details = "New user registration" },
                    new ActivityItem { Time = DateTime.Now.AddMinutes(-25), Action = "Dislike", User = "user7", Details = "Disliked user8" }
                };

                // Add to list view
                foreach (var activity in activities)
                {
                    lvRecentActivity.Items.Add(activity);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading activity: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<User>> GetAllUsers()
        {
            try
            {
                // In a real application, this would call the server API
                // For this demo, we'll create some sample data
                var users = new List<User>
                {
                    new User { UserId = 1, Username = "user1", Gender = "Male", Age = 25, Status = UserStatus.Active },
                    new User { UserId = 2, Username = "user2", Gender = "Female", Age = 28, Status = UserStatus.Active },
                    new User { UserId = 3, Username = "user3", Gender = "Non-binary", Age = 22, Status = UserStatus.Paused },
                    new User { UserId = 4, Username = "user4", Gender = "Male", Age = 30, Status = UserStatus.Offline },
                    new User { UserId = 5, Username = "user5", Gender = "Female", Age = 26, Status = UserStatus.Active }
                };

                return users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<User>();
            }
        }

        private async Task<List<MatchViewModel>> GetAllMatches()
        {
            try
            {
                // In a real application, this would call the server API
                // For this demo, we'll create some sample data
                var matches = new List<MatchViewModel>
                {
                    new MatchViewModel { MatchId = 1, User1Id = 1, User2Id = 2, User1Name = "user1", User2Name = "user2", MatchedAt = DateTime.Now.AddDays(-1) },
                    new MatchViewModel { MatchId = 2, User1Id = 3, User2Id = 4, User1Name = "user3", User2Name = "user4", MatchedAt = DateTime.Now.AddDays(-2) },
                    new MatchViewModel { MatchId = 3, User1Id = 5, User2Id = 1, User1Name = "user5", User2Name = "user1", MatchedAt = DateTime.Now },
                    new MatchViewModel { MatchId = 4, User1Id = 2, User2Id = 3, User1Name = "user2", User2Name = "user3", MatchedAt = DateTime.Now }
                };

                return matches;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting matches: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<MatchViewModel>();
            }
        }

        private async Task<List<LogItem>> GetLogs(string logType, DateTime? date)
        {
            try
            {
                // In a real application, this would read from log files or call the server API
                // For this demo, we'll create some sample data
                var logs = new List<LogItem>
                {
                    new LogItem { Timestamp = DateTime.Now.AddMinutes(-5), Action = "LOGIN", Data = "{ \"username\": \"user1\" }" },
                    new LogItem { Timestamp = DateTime.Now.AddMinutes(-10), Action = "MATCH", Data = "{ \"user1\": \"user2\", \"user2\": \"user3\" }" },
                    new LogItem { Timestamp = DateTime.Now.AddMinutes(-15), Action = "LIKE", Data = "{ \"liker\": \"user4\", \"liked\": \"user5\" }" },
                    new LogItem { Timestamp = DateTime.Now.AddMinutes(-20), Action = "REGISTER", Data = "{ \"username\": \"user6\" }" },
                    new LogItem { Timestamp = DateTime.Now.AddMinutes(-25), Action = "DISLIKE", Data = "{ \"disliker\": \"user7\", \"disliked\": \"user8\" }" }
                };

                // Filter by type if specified
                if (logType != "All Logs")
                {
                    string actionFilter = logType.Replace(" Logs", "").ToUpper();
                    logs = logs.Where(l => l.Action.Contains(actionFilter)).ToList();
                }

                // Filter by date if specified
                if (date.HasValue)
                {
                    logs = logs.Where(l => l.Timestamp.Date == date.Value.Date).ToList();
                }

                return logs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<LogItem>();
            }
        }

        private async void navDashboard_Checked(object sender, RoutedEventArgs e)
        {
            dashboardPanel.Visibility = Visibility.Visible;
            usersPanel.Visibility = Visibility.Collapsed;
            matchesPanel.Visibility = Visibility.Collapsed;
            logsPanel.Visibility = Visibility.Collapsed;
            settingsPanel.Visibility = Visibility.Collapsed;

            await LoadDashboardData();
        }

        private async void navUsers_Checked(object sender, RoutedEventArgs e)
        {
            dashboardPanel.Visibility = Visibility.Collapsed;
            usersPanel.Visibility = Visibility.Visible;
            matchesPanel.Visibility = Visibility.Collapsed;
            logsPanel.Visibility = Visibility.Collapsed;
            settingsPanel.Visibility = Visibility.Collapsed;

            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                // Clear existing items
                lvUsers.Items.Clear();

                // Get users
                var users = await GetAllUsers();

                // Add to list view
                foreach (var user in users)
                {
                    lvUsers.Items.Add(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void navMatches_Checked(object sender, RoutedEventArgs e)
        {
            dashboardPanel.Visibility = Visibility.Collapsed;
            usersPanel.Visibility = Visibility.Collapsed;
            matchesPanel.Visibility = Visibility.Visible;
            logsPanel.Visibility = Visibility.Collapsed;
            settingsPanel.Visibility = Visibility.Collapsed;

            await LoadMatches();
        }

        private async Task LoadMatches()
        {
            try
            {
                // Clear existing items
                lvMatches.Items.Clear();

                // Get matches
                var matches = await GetAllMatches();

                // Add to list view
                foreach (var match in matches)
                {
                    lvMatches.Items.Add(match);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading matches: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void navLogs_Checked(object sender, RoutedEventArgs e)
        {
            dashboardPanel.Visibility = Visibility.Collapsed;
            usersPanel.Visibility = Visibility.Collapsed;
            matchesPanel.Visibility = Visibility.Collapsed;
            logsPanel.Visibility = Visibility.Visible;
            settingsPanel.Visibility = Visibility.Collapsed;

            await LoadLogs();
        }

        private async Task LoadLogs()
        {
            try
            {
                // Clear existing items
                lvLogs.Items.Clear();

                // Get selected log type
                string logType = ((ComboBoxItem)cmbLogType.SelectedItem).Content.ToString();

                // Get selected date
                DateTime? date = dpLogDate.SelectedDate;

                // Get logs
                var logs = await GetLogs(logType, date);

                // Add to list view
                foreach (var log in logs)
                {
                    lvLogs.Items.Add(log);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void navSettings_Checked(object sender, RoutedEventArgs e)
        {
            dashboardPanel.Visibility = Visibility.Collapsed;
            usersPanel.Visibility = Visibility.Collapsed;
            matchesPanel.Visibility = Visibility.Collapsed;
            logsPanel.Visibility = Visibility.Collapsed;
            settingsPanel.Visibility = Visibility.Visible;
        }

        private async void btnExportLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // In a real application, this would call the Logger.ExportLogsAsCsv method
                // For this demo, we'll just show a message
                MessageBox.Show("Logs exported successfully to logs/export.csv", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save settings
                _baseUrl = txtServerUrl.Text;
                _logDirectory = txtLogDirectory.Text;
                _autoExportLogs = chkAutoExportLogs.IsChecked ?? false;

                // In a real application, these would be saved to a settings file

                MessageBox.Show("Settings saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnViewUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get user ID from button tag
                int userId = Convert.ToInt32(((Button)sender).Tag);

                // In a real application, this would open a user details window
                MessageBox.Show($"Viewing user {userId}", "User Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSetUserActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get user ID from button tag
                int userId = Convert.ToInt32(((Button)sender).Tag);

                // In a real application, this would call the server API to update the user status
                MessageBox.Show($"User {userId} set to Active", "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh users list
                await LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSetUserPaused_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get user ID from button tag
                int userId = Convert.ToInt32(((Button)sender).Tag);

                // In a real application, this would call the server API to update the user status
                MessageBox.Show($"User {userId} set to Paused", "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh users list
                await LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnViewMatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get match ID from button tag
                int matchId = Convert.ToInt32(((Button)sender).Tag);

                // In a real application, this would open a match details window
                MessageBox.Show($"Viewing match {matchId}", "Match Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing match: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void cmbLogType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadLogs();
        }

        private async void dpLogDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadLogs();
        }

        private async void btnRefreshLogs_Click(object sender, RoutedEventArgs e)
        {
            await LoadLogs();
        }
    }

    // View Models
    public class ActivityItem
    {
        public DateTime Time { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public string Details { get; set; }
    }

    public class LogItem
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
    }

    public class MatchViewModel
    {
        public int MatchId { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public string User1Name { get; set; }
        public string User2Name { get; set; }
        public DateTime MatchedAt { get; set; }
    }
}
