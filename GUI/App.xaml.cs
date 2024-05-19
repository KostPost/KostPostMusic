using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using ClassesData;

namespace KostPostMusic;

using System.Windows;

using Microsoft.Win32;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (IsUserLoggedIn())
        {
            OpenMainWindow();
        }
        else
        {
            AuthenticationWindow loginWindow = new AuthenticationWindow();
            loginWindow.Show();
        }
    }

    private bool IsUserLoggedIn()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\KostPostMusic");
        if (key != null)
        {
            string username = (string)key.GetValue("Username");
            string password = (string)key.GetValue("Password");
            key.Close();
            
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }
        return false;
    }

    public static void SaveCredentials(string username, string password)
    {
        RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\KostPostMusic");
        key.SetValue("Username", username, RegistryValueKind.String);
        key.SetValue("Password", password, RegistryValueKind.String);
        key.Close();
    }

    public static void ClearCredentials()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\KostPostMusic", true);
        if (key != null)
        {
            key.DeleteValue("Username");
            key.DeleteValue("Password");
            key.Close();
        }
    }
    public static void RestartApplication()
    {
        // Get the location of the current executable
        string assemblyLocation = Assembly.GetEntryAssembly().Location;

        // Get the path to the executable file
        string executablePath = assemblyLocation.Replace(".dll", ".exe");

        // Start a new instance of the application
        Process.Start(new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = true
        });

        // Exit the current instance of the application
        Application.Current.Shutdown();
    }
    
    private void OpenMainWindow()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\KostPostMusic");
        if (key != null)
        {
            string username = (string)key.GetValue("Username");
            key.Close();
        
            MainWindow mainWindow = new MainWindow(new UserAccount(username, " "));
            mainWindow.Show();
        }
    }

}
