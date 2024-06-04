using System.Windows;
using System.Windows.Controls;
using ClassesData;

namespace KostPostMusic;

public partial class AdminMainPage : Window
    {
        public string AdminName { get; set; }
        private bool isMenuOpen = false;

        public AdminMainPage(Account account)
        {
            InitializeComponent();
            AdminName = account.Username;
            DataContext = this;
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.ClearCredentials();
            App.RestartApplication();
        }
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            // Add logic for the "Profile" action here
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // Add logic for the "Settings" action here
        }
    }