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
    public MainWindow() 
    {
        InitializeComponent();
        VerifyUserAuthorization();
    }

    public MainWindow(UserAccount userAccount)  
    {
        InitializeComponent();
        UsernameLabel.Content = userAccount.Username;
    }

    private void VerifyUserAuthorization()
    {
        if (!Application.Current.Windows.OfType<AuthenticationWindow>().Any())
        {
            AuthenticationWindow authenticationWindow = new AuthenticationWindow();
            if (authenticationWindow.ShowDialog() == true)
            {
                string username = authenticationWindow.UserAccount.Username;
                Console.WriteLine("Authorized user: " + username);
                UsernameLabel.Content = username;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}