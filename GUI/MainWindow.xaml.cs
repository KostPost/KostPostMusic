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
    public MainWindow() 
    {
        InitializeComponent();
    }

    public MainWindow(UserAccount userAccount)  
    {
        InitializeComponent();
        UserAccount = userAccount;
        // UsernameLabel.Content = UserAccount.Username;
        Console.WriteLine(userAccount.Username);
    }
    
    private void UsernameButton_Click(object sender, RoutedEventArgs e)
    {
        ContextMenu menu = new ContextMenu();
        menu.Items.Add(new MenuItem() { Header = "Action 1" });
        menu.Items.Add(new MenuItem() { Header = "Action 2" });
        menu.Items.Add(new MenuItem() { Header = "Action 3" });
        menu.Items.Add(new MenuItem() { Header = "Action 4" });
        menu.Items.Add(new MenuItem() { Header = "Action 5" });

        // Set the content of the button to display the username and an arrow icon
        UsernameButton.Content = $"{UserAccount.Username} \u25BC"; // Unicode for down arrow

        // When the menu closes, reset the button content to just the username
        menu.Closed += (sender, args) => UsernameButton.Content = UserAccount.Username;

        menu.PlacementTarget = UsernameButton;
        menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        menu.IsOpen = true;
    }


    
    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        App.ClearCredentials();
        Close();
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