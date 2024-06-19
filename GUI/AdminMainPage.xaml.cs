using System.Windows;
using System.Windows.Controls;
using Azure.Storage.Blobs;
using ClassesData;
using MusicAPI;

namespace KostPostMusic;

public partial class AdminMainPage : Window
    {
        public string AdminName { get; set; }
        private bool isMenuOpen = false;

        // private readonly BlobServiceClient _blobServiceClient;
        // private readonly AzureBlobs _azureBlobs;


        public AdminMainPage(Account account)
        {
            InitializeComponent();

            // _azureBlobs = new AzureBlobs();
            // _azureBlobs.UploadBlobAsync("virus", "C:\\Users\\KostPost\\Downloads\\virus.mp3");
            
            AdminName = account.Username;
            DataContext = this;
        }
        private void AddMusicButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the "Add Music" menu
            AddMusicMenu addMusicMenu = new AddMusicMenu();
            addMusicMenu.Owner = this; // Set the owner window
            addMusicMenu.ShowDialog(); // Show the menu as a modal dialog
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