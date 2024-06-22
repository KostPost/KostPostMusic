using System.Windows;
using ClassesData;
using DataBaseActions;
using DataBaseActions.Music;
using Microsoft.Win32;
using MusicAPI;

namespace KostPostMusic
{
    public partial class AddMusicMenu : Window
    {
        private Account currentUser;
        private string selectedFilePath;

        public AddMusicMenu(Account account)
        {
            currentUser = account;
            InitializeComponent();
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 Files (*.mp3)|*.mp3|All Files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                SelectedFileTextBlock.Text = selectedFilePath;
            }
        }

        private async void AddMusicButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedFilePath))
            {
                MessageBox.Show("Please select an MP3 file first.");
                return;
            }

            string musicName = MusicNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(musicName))
            {
                MessageBox.Show("Please enter a music name.");
                return;
            }

            try
            {
                using (var dbContext = new KostPostMusicContext())
                {
                    var azureBlobs = new AzureBlobs();
                    var musicService = new MusicService(dbContext, azureBlobs);

                    var result = await musicService.AddMusicFileAsync(musicName, selectedFilePath, currentUser);

                    if (result.Success)
                    {
                        MessageBox.Show("Music file added successfully.");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show($"An error occurred: {result.Message}");
                    }
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}