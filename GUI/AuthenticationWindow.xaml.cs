using System.Windows;
using ClassesData;
using DataBaseActions;
using Microsoft.EntityFrameworkCore;

namespace KostPostMusic;

public partial class AuthenticationWindow : Window
{

    public UserAccount UserAccount { get; set; }

    private readonly UserService _userService;

    public AuthenticationWindow()
    {
        InitializeComponent();
        SwitchToLoginView();

        var options = new DbContextOptionsBuilder<UserAccountDbContext>()
            .UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025")
            .Options;
        var dbContext = new UserAccountDbContext(options);
        _userService = new UserService(dbContext);
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string username = LoginUsernameTextBox.Text;
        string password = LoginPasswordBox.Password;

        AuthenticationResult authResult = await AuthenticateUser(username, password);

        switch (authResult)
        {
            case AuthenticationResult.Success:
                App.SaveCredentials(username, password); 
                OpenMainWindow(UserAccount);
                MessageTextBlock.Text = "Authorization successful!";
                break;
            case AuthenticationResult.UserNotFound:
                MessageTextBlock.Text = "User with this username does not exist.";
                break;
            case AuthenticationResult.IncorrectPassword:
                MessageTextBlock.Text = "Incorrect password.";
                break;
            default:
                MessageTextBlock.Text = "Unexpected authentication result.";
                break;
        }
    }



    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string username = RegisterUsernameTextBox.Text;
        string password = RegisterPasswordBox.Password;

        RegistrationResult registrationResult = await RegisterUser(username, password);

        switch (registrationResult)
        {
            case RegistrationResult.Success:
                MessageTextBlock.Text = "Account created successfully! Please log in.";
                SwitchToLoginView();
                break;
            case RegistrationResult.UserAlreadyExists:
                MessageTextBlock.Text = "Registration failed. User already exists.";
                break;
            case RegistrationResult.Error:
                MessageTextBlock.Text = "Registration failed due to an error.";
                break;
        }
    }


    private void SwitchToLogin_Click(object sender, RoutedEventArgs e)
    {
        SwitchToLoginView();
    }

    private void SwitchToRegister_Click(object sender, RoutedEventArgs e)
    {
        SwitchToRegisterView();
    }

    private void SwitchToLoginView()
    {
        LoginPanel.Visibility = Visibility.Visible;
        RegisterPanel.Visibility = Visibility.Collapsed;
        MessageTextBlock.Text = "Please log in to continue.";
    }

    private void SwitchToRegisterView()
    {
        LoginPanel.Visibility = Visibility.Collapsed;
        RegisterPanel.Visibility = Visibility.Visible;
        MessageTextBlock.Text = "Create a new account.";
    }

    private async Task<AuthenticationResult> AuthenticateUser(string username, string password)
    {
        var user = await _userService.GetUserByUsername(username);

        if (user == null)
        {
            return AuthenticationResult.UserNotFound;
        }

        if (user.Password != password)
        {
            return AuthenticationResult.IncorrectPassword;
        }

        UserAccount = user;

        return AuthenticationResult.Success;
    }


    private async Task<RegistrationResult> RegisterUser(string username, string password)
    {
        var newUser = new UserAccount
        {
            Username = username,
            Password = password, 
            AccountType = AccountType.User
        };

        bool isAdded = await _userService.AddUserAccount(newUser);
        if (isAdded)
        {
            return RegistrationResult.Success;
        }
        else
        {
            return RegistrationResult.UserAlreadyExists;
        }
    }


    private void OpenMainWindow(UserAccount user)
    {
        MainWindow mainWindow = new MainWindow(user);
        mainWindow.Show();
        Close();
    }
}