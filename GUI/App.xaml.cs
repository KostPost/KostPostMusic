using System.Configuration;
using System.Data;
using System.Windows;

namespace KostPostMusic;

using System.Windows;

public partial class App : Application
{
    private MainWindow mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ShutdownMode = ShutdownMode.OnMainWindowClose;
        ShowAuthenticationPage();
    }

    private void AuthPage_AuthenticationSucceeded(object sender, EventArgs e)
    {
        if (sender is AuthenticationWindow authWindow)
        {
            string qwe = "qwe";
            mainWindow = new MainWindow(qwe);
            
            mainWindow.Show();
            Current.MainWindow = mainWindow;  
            authWindow.Close();
        }
    }



    private void ShowAuthenticationPage()
    {
        AuthenticationWindow authWindow = new AuthenticationWindow();
        authWindow.Title = "Authentication";
        authWindow.AuthenticationSucceeded += AuthPage_AuthenticationSucceeded;
        authWindow.ShowDialog();
    }
}