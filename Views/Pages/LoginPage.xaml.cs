using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using gentech_services.Services;

namespace gentech_services.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public event EventHandler LoginSuccess;
        private bool isPasswordVisible = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
            {
                lblPasswordPlaceholder.Visibility = string.IsNullOrEmpty(TxtPassword.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void TxtPasswordVisible_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (isPasswordVisible)
            {
                lblPasswordPlaceholder.Visibility = string.IsNullOrEmpty(TxtPasswordVisible.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void BtnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Show password as text
                TxtPasswordVisible.Text = TxtPassword.Password;
                TxtPassword.Visibility = Visibility.Collapsed;
                TxtPasswordVisible.Visibility = Visibility.Visible;

                // Use Dispatcher to ensure the UI is updated before setting cursor position
                Dispatcher.InvokeAsync(() =>
                {
                    TxtPasswordVisible.Focus();
                    TxtPasswordVisible.SelectionStart = TxtPasswordVisible.Text.Length;
                    TxtPasswordVisible.SelectionLength = 0;
                }, System.Windows.Threading.DispatcherPriority.Input);

                // Change icon to eye-off
                EyeIcon.Data = Geometry.Parse("M12,7A5,5 0 0,1 17,12C17,12.35 16.94,12.69 16.87,13L19.53,15.66C20.45,14.54 21.15,13.21 21.61,12C20.68,9.61 18.07,7.27 14.24,6.43L12,4.18M2.81,2.81L1.39,4.22L4.46,7.29C3.16,8.53 2.24,10.04 1.61,12C3.27,16.39 7,19.5 12,19.5C13.55,19.5 15.03,19.2 16.38,18.66L19.73,22L21.15,20.59L12,11.45M12,9A3,3 0 0,1 15,12C15,12.35 14.94,12.69 14.87,13L11,9.14C11.31,9.05 11.65,9 12,9M12,17A5,5 0 0,1 7,12C7,11.65 7.06,11.31 7.13,11L10,13.87C10,13.91 10,13.95 10,14A2,2 0 0,0 12,16C12.05,16 12.09,16 12.13,16L14.87,18.74C14,18.9 13.06,19 12,19");
            }
            else
            {
                // Hide password
                TxtPassword.Password = TxtPasswordVisible.Text;
                TxtPasswordVisible.Visibility = Visibility.Collapsed;
                TxtPassword.Visibility = Visibility.Visible;

                // Use Dispatcher to ensure the UI is updated before setting focus
                Dispatcher.InvokeAsync(() =>
                {
                    TxtPassword.Focus();
                }, System.Windows.Threading.DispatcherPriority.Input);

                // Change icon back to eye
                EyeIcon.Data = Geometry.Parse("M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z");
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string pin = isPasswordVisible ? TxtPasswordVisible.Text.Trim() : TxtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter your username.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(pin))
            {
                MessageBox.Show("Please enter your PIN.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool loginResult = AuthenticationService.Instance.Login(username, pin);

            if (loginResult)
            {
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Invalid username or PIN. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtPassword.Clear();
            }
        }
    }
}
