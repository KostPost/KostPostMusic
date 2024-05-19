using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassesData;

namespace KostPostMusic;

// MainWindow.xaml.cs
public partial class MainWindow : Window
{
    private UserAccount UserAccount;
    private bool isMenuOpen = false;

    public MainWindow() 
    {
        InitializeComponent();
    }

    public MainWindow(UserAccount userAccount)  
    {
        InitializeComponent();
        UserAccount = userAccount;
        UpdateButtonContent();
    }

    private void UpdateButtonContent()
    {
        UsernameButton.Content = $"{UserAccount.Username} \u25BC"; // Unicode for down arrow
    }
    

    private void UsernameButton_Click(object sender, RoutedEventArgs e)
    {
        if (!isMenuOpen)
        {
            ContextMenu menu = new ContextMenu();

            // Add actions for the menu items
            MenuItem profileItem = new MenuItem() { Header = "Profile" };
            profileItem.Click += Profile_Click;
            menu.Items.Add(profileItem);

            MenuItem settingsItem = new MenuItem() { Header = "Settings" };
            settingsItem.Click += Settings_Click;
            menu.Items.Add(settingsItem);

            MenuItem logoutItem = new MenuItem() { Header = "Log out" };
            logoutItem.Click += Logout_Click;
            menu.Items.Add(logoutItem);

            menu.Closed += (sender, args) => isMenuOpen = false;

            menu.PlacementTarget = UsernameButton;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.IsOpen = true;

            isMenuOpen = true;
        }
        else
        {
            ContextMenu menu = UsernameButton.ContextMenu;
            if (menu != null)
            {
                menu.IsOpen = false;
                isMenuOpen = false;
            }
        }
    }

    private void Profile_Click(object sender, RoutedEventArgs e)
    {
        // Add logic for the "Profile" action here
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        // Add logic for the "Settings" action here
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        App.ClearCredentials();

        App.RestartApplication();
    }






    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateButtonContent();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!isMenuOpen)
            UpdateButtonContent();
    }
}






// private void VerifyUserAuthorization()
// {
//     if (!Application.Current.Windows.OfType<AuthenticationWindow>().Any())
//     {
//         AuthenticationWindow authenticationWindow = new AuthenticationWindow();
//         if (authenticationWindow.ShowDialog() == true)
//         {
//             string username = authenticationWindow.UserAccount.Username;
//             Console.WriteLine("Authorized user: " + username);
//             UsernameLabel.Content = username;
//         }
//         else
//         {
//             Application.Current.Shutdown();
//         }
//     }
// }