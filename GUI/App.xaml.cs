using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using ClassesData;
using DataBaseActions;
using Microsoft.EntityFrameworkCore;

namespace KostPostMusic;

using System.Windows;
using Microsoft.Win32;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Account account = getCurrentAccountInfo();

        if (IsUserLoggedIn())
        {
            OpenMainWindow(account);
        }
        else
        {
            AuthenticationWindow loginWindow = new AuthenticationWindow();
            loginWindow.Show();
        }
    }

    private void StartApplication()
    {
    }

    private Account getCurrentAccountInfo()
    {
        try
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\KostPostMusic");
            if (key != null)
            {
                string username = (string)key.GetValue("Username");
                string password = (string)key.GetValue("Password");
                string accountType = (string)key.GetValue("AccountType");
                int id = (int)key.GetValue("Id");
                key.Close();

                Account account = new Account(id,username, password, accountType);
                //Account account = new Account(username, password, accountType);
                return account;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving current user information: {ex.Message}");
        }

        return null; // Return null or handle accordingly if user information retrieval fails
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

    public static void SaveCredentials(int id, string username, string password, string accountType)
    {
        using (RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\KostPostMusic"))
        {
            key.SetValue("Username", username, RegistryValueKind.String);
            key.SetValue("Password", password, RegistryValueKind.String);
            key.SetValue("AccountType", accountType, RegistryValueKind.String);
            key.SetValue("Id", id, RegistryValueKind.DWord);
        }
    }
    public static void ClearCredentials()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\KostPostMusic", true);
        if (key != null)
        {
            key.DeleteValue("Username");
            key.DeleteValue("Password");
            key.DeleteValue("AccountType");
            key.DeleteValue("Id");
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


    private void OpenMainWindow(Account account)
    {
        Console.WriteLine(account.AccountType);
        // Console.WriteLine("User");
        // Console.WriteLine("Admin");


        //RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\KostPostMusic");


        if (account.AccountType == AccountType.User.ToString())
        {
            UserMainPage userMainPage = new UserMainPage(account);
            userMainPage.Show();
        }
        else if (account.AccountType == AccountType.Admin.ToString())

        {
            AdminMainPage adminMainPage = new AdminMainPage(account);
            adminMainPage.Show();
        }
    }
}