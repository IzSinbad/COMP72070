using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GitCommit.Shared.Models;
using Microsoft.Win32;

namespace GitCommit.Client.Views
{
    public partial class MainWindow : Window
    {
        private List<User> _profilesQueue = new List<User>();
        private User _currentProfile;
        private User _currentUser;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set username in header
                txtUsername.Text = App.CurrentUser.Username;

                // Load user profile
                await LoadUserProfile();

                // Load profiles to explore
                await LoadProfilesToExplore();

                // Show first profile
                ShowNextProfile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadUserProfile()
        {
            try
            {
                var profileService = App.ServiceFactory.CreateProfileService();
                var response = await profileService.GetProfile(App.CurrentUser.UserId);

                if (response != null && response.User != null)
                {
                    _currentUser = response.User;

                    // Set user profile data
                    txtUserUsername.Text = _currentUser.Username;
                    txtUserBio.Text = _currentUser.Bio;
                    txtUserAge.Text = _currentUser.Age.ToString();

                    // Set gender
                    foreach (ComboBoxItem item in cmbUserGender.Items)
                    {
                        if (item.Content.ToString() == _currentUser.Gender)
                        {
                            cmbUserGender.SelectedItem = item;
                            break;
                        }
                    }

                    // Set preferences
                    if (_currentUser.Preferences != null)
                    {
                        txtMinAge.Text = _currentUser.Preferences.MinAge.ToString();
                        txtMaxAge.Text = _currentUser.Preferences.MaxAge.ToString();

                        // Set preferred genders
                        chkMale.IsChecked = _currentUser.Preferences.PreferredGenders.Contains("Male");
                        chkFemale.IsChecked = _currentUser.Preferences.PreferredGenders.Contains("Female");
                        chkNonBinary.IsChecked = _currentUser.Preferences.PreferredGenders.Contains("Non-binary");
                        chkOther.IsChecked = _currentUser.Preferences.PreferredGenders.Contains("Other");
                    }

                    // Load profile image if available
                    if (_currentUser.Images != null && _currentUser.Images.Count > 0)
                    {
                        var image = _currentUser.Images[0];
                        var bitmap = new BitmapImage();
                        using (var ms = new MemoryStream(image.ImageData))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                        }
                        imgUserProfile.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadProfilesToExplore()
        {
            try
            {
                // Show loading message
                txtLoading.Visibility = Visibility.Visible;
                txtNoProfiles.Visibility = Visibility.Collapsed;
                
                // Disable action buttons
                btnLike.IsEnabled = false;
                btnDislike.IsEnabled = false;

                // Get profiles to explore
                var profileService = App.ServiceFactory.CreateProfileService();
                _profilesQueue = await profileService.GetProfilesToExplore(App.CurrentUser.UserId);

                // Hide loading message
                txtLoading.Visibility = Visibility.Collapsed;
                
                // Enable action buttons if profiles are available
                btnLike.IsEnabled = _profilesQueue.Count > 0;
                btnDislike.IsEnabled = _profilesQueue.Count > 0;

                // Show no profiles message if no profiles are available
                if (_profilesQueue.Count == 0)
                {
                    txtNoProfiles.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                txtLoading.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Error loading profiles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowNextProfile()
        {
            if (_profilesQueue.Count > 0)
            {
                // Get next profile
                _currentProfile = _profilesQueue[0];
                _profilesQueue.RemoveAt(0);

                // Set profile data
                txtProfileName.Text = _currentProfile.Username + ",";
                txtProfileAge.Text = _currentProfile.Age.ToString();
                txtProfileBio.Text = _currentProfile.Bio;
                txtProfileGender.Text = _currentProfile.Gender;

                // Load profile image if available
                if (_currentProfile.Images != null && _currentProfile.Images.Count > 0)
                {
                    var image = _currentProfile.Images[0];
                    var bitmap = new BitmapImage();
                    using (var ms = new MemoryStream(image.ImageData))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                    }
                    imgProfile.Source = bitmap;
                }
                else
                {
                    imgProfile.Source = null;
                }

                // Show profile card
                txtNoProfiles.Visibility = Visibility.Collapsed;
                btnLike.IsEnabled = true;
                btnDislike.IsEnabled = true;
            }
            else
            {
                // No more profiles
                txtProfileName.Text = "";
                txtProfileAge.Text = "";
                txtProfileBio.Text = "";
                txtProfileGender.Text = "";
                imgProfile.Source = null;

                txtNoProfiles.Visibility = Visibility.Visible;
                btnLike.IsEnabled = false;
                btnDislike.IsEnabled = false;
            }
        }

        private async void btnLike_Click(object sender, RoutedEventArgs e)
        {
            if (_currentProfile != null)
            {
                try
                {
                    // Disable buttons to prevent multiple clicks
                    btnLike.IsEnabled = false;
                    btnDislike.IsEnabled = false;

                    // Create like action
                    var likeAction = new LikeAction
                    {
                        LikerId = App.CurrentUser.UserId,
                        LikedId = _currentProfile.UserId,
                        LikedAt = DateTime.Now
                    };

                    // Call like service
                    var matchingService = App.ServiceFactory.CreateMatchingService();
                    var response = await matchingService.LikeProfile(likeAction);

                    // Check if it's a match
                    if (response is dynamic dynamicResponse && dynamicResponse.IsMatch)
                    {
                        // Show match popup
                        ShowMatchPopup(_currentProfile);
                    }
                    else
                    {
                        // Show next profile
                        ShowNextProfile();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error liking profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    btnLike.IsEnabled = true;
                    btnDislike.IsEnabled = true;
                }
            }
        }

        private async void btnDislike_Click(object sender, RoutedEventArgs e)
        {
            if (_currentProfile != null)
            {
                try
                {
                    // Disable buttons to prevent multiple clicks
                    btnLike.IsEnabled = false;
                    btnDislike.IsEnabled = false;

                    // Create dislike action
                    var dislikeAction = new DislikeAction
                    {
                        DislikerId = App.CurrentUser.UserId,
                        DislikedId = _currentProfile.UserId,
                        DislikedAt = DateTime.Now
                    };

                    // Call dislike service
                    var matchingService = App.ServiceFactory.CreateMatchingService();
                    await matchingService.DislikeProfile(dislikeAction);

                    // Show next profile
                    ShowNextProfile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error disliking profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    btnLike.IsEnabled = true;
                    btnDislike.IsEnabled = true;
                }
            }
        }

        private void ShowMatchPopup(User matchedUser)
        {
            // Set match data
            txtMatchName.Text = matchedUser.Username;

            // Set match image
            if (matchedUser.Images != null && matchedUser.Images.Count > 0)
            {
                var image = matchedUser.Images[0];
                var bitmap = new BitmapImage();
                using (var ms = new MemoryStream(image.ImageData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                }
                imgMatchProfile.Source = bitmap;
            }
            else
            {
                imgMatchProfile.Source = null;
            }

            // Show popup
            matchPopup.Visibility = Visibility.Visible;
        }

        private void btnCloseMatchPopup_Click(object sender, RoutedEventArgs e)
        {
            // Hide popup
            matchPopup.Visibility = Visibility.Collapsed;

            // Show next profile
            ShowNextProfile();
        }

        private async void tabMatches_Checked(object sender, RoutedEventArgs e)
        {
            // Show matches tab
            exploreTab.Visibility = Visibility.Collapsed;
            matchesTab.Visibility = Visibility.Visible;
            profileTab.Visibility = Visibility.Collapsed;

            // Load matches
            await LoadMatches();
        }

        private async System.Threading.Tasks.Task LoadMatches()
        {
            try
            {
                // Clear matches list
                matchesList.Children.Clear();
                
                // Show loading message
                txtLoadingMatches.Visibility = Visibility.Visible;
                txtNoMatches.Visibility = Visibility.Collapsed;

                // Get matches
                var matchingService = App.ServiceFactory.CreateMatchingService();
                var response = await matchingService.GetMatches(App.CurrentUser.UserId);

                // Hide loading message
                txtLoadingMatches.Visibility = Visibility.Collapsed;

                // Show no matches message if no matches are available
                if (response.Matches.Count == 0)
                {
                    txtNoMatches.Visibility = Visibility.Visible;
                    return;
                }

                // Add matches to list
                foreach (var match in response.Matches)
                {
                    var matchCard = new Border
                    {
                        Background = System.Windows.Media.Brushes.White,
                        CornerRadius = new CornerRadius(8),
                        Margin = new Thickness(0, 0, 0, 15),
                        Padding = new Thickness(15)
                    };

                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    // Profile image
                    var image = new Image
                    {
                        Width = 60,
                        Height = 60,
                        Stretch = System.Windows.Media.Stretch.UniformToFill,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0, 0, 15, 0)
                    };

                    // Set image source if available
                    if (match.MatchedUser != null && match.MatchedUser.Images != null && match.MatchedUser.Images.Count > 0)
                    {
                        var userImage = match.MatchedUser.Images[0];
                        var bitmap = new BitmapImage();
                        using (var ms = new MemoryStream(userImage.ImageData))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                        }
                        image.Source = bitmap;
                    }

                    Grid.SetColumn(image, 0);
                    grid.Children.Add(image);

                    // Match info
                    var infoPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 0) };

                    // Username
                    var username = new TextBlock
                    {
                        Text = match.MatchedUser?.Username ?? "Unknown User",
                        FontWeight = System.Windows.FontWeights.SemiBold,
                        FontSize = 16,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    infoPanel.Children.Add(username);

                    // Match date
                    var matchDate = new TextBlock
                    {
                        Text = $"Matched on {match.MatchedAt.ToShortDateString()}",
                        Foreground = System.Windows.Media.Brushes.Gray,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    infoPanel.Children.Add(matchDate);

                    Grid.SetColumn(infoPanel, 1);
                    grid.Children.Add(infoPanel);

                    matchCard.Child = grid;
                    matchesList.Children.Add(matchCard);
                }
            }
            catch (Exception ex)
            {
                txtLoadingMatches.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Error loading matches: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tabExplore_Checked(object sender, RoutedEventArgs e)
        {
            // Show explore tab
            exploreTab.Visibility = Visibility.Visible;
            matchesTab.Visibility = Visibility.Collapsed;
            profileTab.Visibility = Visibility.Collapsed;
        }

        private void tabProfile_Checked(object sender, RoutedEventArgs e)
        {
            // Show profile tab
            exploreTab.Visibility = Visibility.Collapsed;
            matchesTab.Visibility = Visibility.Collapsed;
            profileTab.Visibility = Visibility.Visible;
        }

        private async void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtUserBio.Text))
                {
                    MessageBox.Show("Please enter a bio.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbUserGender.SelectedItem == null)
                {
                    MessageBox.Show("Please select a gender.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtUserAge.Text, out int age) || age < 18 || age > 120)
                {
                    MessageBox.Show("Please enter a valid age (18-120).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtMinAge.Text, out int minAge) || minAge < 18 || minAge > 120)
                {
                    MessageBox.Show("Please enter a valid minimum age (18-120).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtMaxAge.Text, out int maxAge) || maxAge < 18 || maxAge > 120 || maxAge < minAge)
                {
                    MessageBox.Show("Please enter a valid maximum age (18-120, and greater than minimum age).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update user profile
                _currentUser.Bio = txtUserBio.Text;
                _currentUser.Gender = ((ComboBoxItem)cmbUserGender.SelectedItem).Content.ToString();
                _currentUser.Age = age;

                // Update preferences
                _currentUser.Preferences.MinAge = minAge;
                _currentUser.Preferences.MaxAge = maxAge;
                _currentUser.Preferences.PreferredGenders.Clear();

                if (chkMale.IsChecked == true) _currentUser.Preferences.PreferredGenders.Add("Male");
                if (chkFemale.IsChecked == true) _currentUser.Preferences.PreferredGenders.Add("Female");
                if (chkNonBinary.IsChecked == true) _currentUser.Preferences.PreferredGenders.Add("Non-binary");
                if (chkOther.IsChecked == true) _currentUser.Preferences.PreferredGenders.Add("Other");

                // Save profile
                var profileService = App.ServiceFactory.CreateProfileService();
                var success = await profileService.UpdateProfile(_currentUser);

                if (success)
                {
                    MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update profile.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open file dialog
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                    Title = "Select Profile Image"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Read image file
                    var imageData = File.ReadAllBytes(openFileDialog.FileName);

                    // Create user image
                    var userImage = new UserImage
                    {
                        ImageData = imageData,
                        UploadedAt = DateTime.Now
                    };

                    // Upload image
                    var profileService = App.ServiceFactory.CreateProfileService();
                    var success = await profileService.AddProfileImage(_currentUser.UserId, userImage);

                    if (success)
                    {
                        // Add image to user's images
                        if (_currentUser.Images == null)
                        {
                            _currentUser.Images = new List<UserImage>();
                        }
                        _currentUser.Images.Add(userImage);

                        // Display image
                        var bitmap = new BitmapImage();
                        using (var ms = new MemoryStream(imageData))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                        }
                        imgUserProfile.Source = bitmap;

                        MessageBox.Show("Image uploaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to upload image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clear user data
                App.CurrentUser = null;
                App.AuthToken = null;

                // Show login window
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                // Close main window
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error logging out: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
