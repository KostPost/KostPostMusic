using System.Windows;
using System.Windows.Controls;

namespace KostPostMusic;

public partial class AuthenticationWindow : Window
{
    public event EventHandler AuthenticationSucceeded;

    public AuthenticationWindow()
    {
        InitializeComponent();
    }
    

    
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        // Откройте окно регистрации
        RegistrationWindow registrationWindow = new RegistrationWindow();
        registrationWindow.ShowDialog();
    }
    


    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text;
        string password = PasswordBox.Password;

        if (AuthenticateUser(username, password))
        {
            AuthenticationSucceeded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            MessageBox.Show("Invalid username or password.");
        }
    }

    private bool AuthenticateUser(string username, string password)
    {
        return username == "qwe" && password == "zxc";
    }


}