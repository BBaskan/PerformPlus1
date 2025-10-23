using Microsoft.Data.SqlClient;
using PerformPlus.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace PerformPlus.Views
{
    public partial class Login : Window, INotifyPropertyChanged
    {
        private DateTime _currentDate = DateTime.Now;
        private DateTime _currentTime = DateTime.Now;

        public DateTime CurrentDate
        {
            get => _currentDate;
            set { _currentDate = value; OnPropertyChanged(nameof(CurrentDate)); }
        }

        public DateTime CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime)); }
        }

        public Login()
        {
            InitializeComponent();
            DataContext = this;


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                CurrentDate = DateTime.Now;
                CurrentTime = DateTime.Now;
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private void BtnEnglish_Click(object sender, RoutedEventArgs e)
        {
            App.SwitchLanguage("en-US");
        }

        private void BtnTurkish_Click(object sender, RoutedEventArgs e)
        {
            App.SwitchLanguage("tr-TR");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = (chkShowPassword.IsChecked == true) ? txtPassword.Text : pwdBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    (string)Application.Current.Resources["EmptyFields"],
                    (string)Application.Current.Resources["InputErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT EmployeeID, FullName, Role FROM Employees WHERE Username = @Username AND PasswordHash = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int employeeId = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                string role = reader.GetString(2);

                                SessionManager.EmployeeID = employeeId;
                                SessionManager.FullName = name;
                                SessionManager.Role = role;


                                string welcomeMsg = string.Format((string)Application.Current.Resources["LoginWelcome"], name);
                                MessageBox.Show(welcomeMsg,
                                    (string)Application.Current.Resources["LoginSuccessTitle"],
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                                Window targetWindow;
                                switch (role)
                                {
                                    case "Admin":
                                        targetWindow = new AdminWindow();
                                        break;
                                    case "Leader":
                                        targetWindow = new LeaderWindow();
                                        break;
                                    case "User":
                                    default:
                                        targetWindow = new UserWindow(employeeId);

                                        break;
                                }

                                targetWindow.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show(
                                    (string)Application.Current.Resources["LoginFailed"],
                                    (string)Application.Current.Resources["LoginFailedTitle"],
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format((string)Application.Current.Resources["LoginError"], ex.Message);
                MessageBox.Show(errorMessage,
                    (string)Application.Current.Resources["GeneralErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {

            txtPassword.Text = pwdBox.Password;
            txtPassword.Visibility = Visibility.Visible;
            pwdBox.Visibility = Visibility.Collapsed;
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {

            pwdBox.Password = txtPassword.Text;
            pwdBox.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Collapsed;
        }
    }
}