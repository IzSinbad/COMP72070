using System;
using System.Windows;
using System.Windows.Input;
using GitCommit.Shared.Models;

namespace GitCommit.Client.Views
{
    public partial class LoginWindow : Window
    {
        private bool _isRegisterMode = false;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void txtRegisterLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isRegisterMode = !_isRegisterMode;
            
            if (_isRegisterMode)
            {
                registerPanel.Visibility = Visibility.Visible;
                txtRegisterLink.Text = "Back to Login";
            }
            else
            {
                registerPanel.Visibility = Visibility.Collapsed;
                txtRegisterLink.Text = "Register";
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clear previous error messages
                txtErrorMessage.Visibility = Visibility.Collapsed;
                
                // Validate input
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    txtErrorMessage.Text = "Please enter both username and password.";
                    txtErrorMessage.Visibility = Visibility.Visible;
                    return;
                }
                
                // Disable login button to prevent multiple clicks
                btnLogin.IsEnabled = false;
                
                // Create login request
                var request = new LoginRequest
                {
                    Username = txtUsername.Text,
                    Password = txtPassword.Password
                };
                
                // Call login service
                var authService = App.ServiceFactory.CreateAuthService();
                var response = await authService.Login(request);
                
                if (response.Success)
                {
                    // Store user info and token in App
                    App.CurrentUser = new App.User
                    {
                        UserId = response.User.UserId,
                        Username = response.User.Username
                    };
                    App.AuthToken = response.Token;
                    App.ServiceFactory.SetToken(response.Token);
                    
                    // Open main window
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    
                    // Close login window
                    this.Close();
                }
                else
                {
                    txtErrorMessage.Text = response.Message;
                    txtErrorMessage.Visibility = Visibility.Visible;
                    btnLogin.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                txtErrorMessage.Text = $"An error occurred: {ex.Message}";
                txtErrorMessage.Visibility = Visibility.Visible;
                btnLogin.IsEnabled = true;
            }
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clear previous error messages
                txtRegisterErrorMessage.Visibility = Visibility.Collapsed;
                
                // Validate input
                if (string.IsNullOrWhiteSpace(txtRegisterUsername.Text) || string.IsNullOrWhiteSpace(txtRegisterPassword.Password))
                {
                    txtRegisterErrorMessage.Text = "Please enter both username and password.";
                    txtRegisterErrorMessage.Visibility = Visibility.Visible;
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtBio.Text))
                {
                    txtRegisterErrorMessage.Text = "Please enter a bio.";
                    txtRegisterErrorMessage.Visibility = Visibility.Visible;
                    return;
                }
                
                if (cmbGender.SelectedItem == null)
                {
                    txtRegisterErrorMessage.Text = "Please select a gender.";
                    txtRegisterErrorMessage.Visibility = Visibility.Visible;
                    return;
                }
                
                if (!int.TryParse(txtAge.Text, out int age) || age < 18 || age > 120)
                {
                    txtRegisterErrorMessage.Text = "Please enter a valid age (18-120).";
                    txtRegisterErrorMessage.Visibility = Visibility.Visible;
                    return;
                }
                
                // Disable register button to prevent multiple clicks
                btnRegister.IsEnabled = false;
                
                // Create register request
                var gender = ((ComboBoxItem)cmbGender.SelectedItem).Content.ToString();
                var request = new RegisterRequest
                {
                    Username = txtRegisterUsername.Text,
                    Password = txtRegisterPassword.Password,
                    Bio = txtBio.Text,
                    Gender = gender,
                    Age = age
                };
                
                // Call register service
                var authService = App.ServiceFactory.CreateAuthService();
                var response = await authService.Register(request);
                
                if (response.Success)
                {
                    // Store user info in App
                    App.CurrentUser = new App.User
                    {
                        UserId = response.User.UserId,
                        Username = response.User.Username
                    };
                    
                    // Switch back to login mode
                    _isRegisterMode = false;
                    registerPanel.Visibility = Visibility.Collapsed;
                    txtRegisterLink.Text = "Register";
                    
                    // Show success message
                    MessageBox.Show("Registration successful! Please login with your new account.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Pre-fill login fields
                    txtUsername.Text = txtRegisterUsername.Text;
                    txtPassword.Password = txtRegisterPassword.Password;
                    
                    // Reset register fields
                    txtRegisterUsername.Text = string.Empty;
                    txtRegisterPassword.Password = string.Empty;
                    txtBio.Text = string.Empty;
                    cmbGender.SelectedIndex = -1;
                    txtAge.Text = string.Empty;
                    
                    btnRegister.IsEnabled = true;
                }
                else
                {
                    txtRegisterErrorMessage.Text = response.Message;
                    txtRegisterErrorMessage.Visibility = Visibility.Visible;
                    btnRegister.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                txtRegisterErrorMessage.Text = $"An error occurred: {ex.Message}";
                txtRegisterErrorMessage.Visibility = Visibility.Visible;
                btnRegister.IsEnabled = true;
            }
        }
    }
}
