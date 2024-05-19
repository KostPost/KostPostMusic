using System.Configuration;
using System.Data;
using System.Windows;

namespace KostPostMusic;

using System.Windows;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AuthenticationWindow loginWindow = new AuthenticationWindow();
        loginWindow.Show();
    }
}