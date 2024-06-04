using System.Windows;
using ClassesData;
using DataBaseActions;
using Microsoft.EntityFrameworkCore;

namespace KostPostMusic;

public partial class AuthenticationWindow : Window
{
    public Account Account { get; set; }

    private readonly UserService _userService;

    public AuthenticationWindow()
    {
        InitializeComponent();
        SwitchToLoginView();

        var options = new DbContextOptionsBuilder<KostPostMusicContext>()
            .UseNpgsql("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025")
            .Options;
        var dbContext = new KostPostMusicContext(options);
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
                
                App.SaveCredentials(username, password, Account.AccountType);
                
                OpenMainWindow(Account);
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
        try
        {
            var user = await _userService.GetUserByUsername(username).ConfigureAwait(false);

            if (user == null)
            {
                return AuthenticationResult.UserNotFound;
            }

            if (user.Password != password)
            {
                return AuthenticationResult.IncorrectPassword;
            }

            Account = user;

            Account.AccountType = user.AccountType;

            return AuthenticationResult.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error authenticating user: {ex.Message}");
            return AuthenticationResult.Error;
        }
    }


    public async Task<RegistrationResult> RegisterUser(string username, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return RegistrationResult.InvalidInput;
            }

            var existingUser = await _userService.GetUserByUsername(username).ConfigureAwait(false);
            if (existingUser != null)
            {
                return RegistrationResult.UserAlreadyExists;
            }

            var newUser = new Account(username, password, AccountType.User.ToString());

            bool isAdded = await _userService.AddUserAccount(newUser).ConfigureAwait(false);
            if (isAdded)
            {
                return RegistrationResult.Success;
            }
            else
            {
                return RegistrationResult.DatabaseError;
            }
        }
        catch (DbUpdateException ex)
        {
            // Log specific database exceptions
            Console.WriteLine($"Database error during user registration: {ex.Message}");
            return RegistrationResult.DatabaseError;
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions
            Console.WriteLine($"Error during user registration: {ex.Message}");
            return RegistrationResult.DatabaseError;
        }
    }


    private void OpenMainWindow(Account account)
    {
        Console.WriteLine("OpenMainWindow" + account.AccountType);
        if (account.AccountType == AccountType.User.ToString())
        {
            UserMainPage userMainPage = new UserMainPage(account);
            userMainPage.Show();
        } else if(account.AccountType == AccountType.Admin.ToString())
        {
            UserMainPage userMainPage = new UserMainPage(account);
            userMainPage.Show();
        }
        
        Close();
    }
}