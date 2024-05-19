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

namespace KostPostMusic;

// MainWindow.xaml.cs
public partial class MainWindow : Window
{
    public MainWindow() 
    {
        if (!Application.Current.Windows.OfType<MainWindow>().Any())
        {
            Console.WriteLine("MainPage trigger");
            InitializeComponent();
            if (UsernameLabel == null)
            {
                throw new Exception("UsernameLabel is null!");
            }
        }
    }

    public MainWindow(string username)  // This is your existing constructor
    {
        InitializeComponent();
    
        Console.WriteLine("username MainPage trigger \t" + username);

        UsernameLabel.Content = username;
    }
    
    private void ShowLoginPage()
    {
        AuthenticationWindow loginWindow = new AuthenticationWindow();
        loginWindow.RegisterButtonClick += ShowRegistrationPage;
        ContentArea.Content = loginWindow;
    }

    private void ShowRegistrationPage(object sender, RoutedEventArgs e)
    {
        RegistrationWindow registrationWindow = new RegistrationWindow();
        registrationWindow.BackButtonClick += ShowLoginPage;
        ContentArea.Content = registrationWindow;
    }
}