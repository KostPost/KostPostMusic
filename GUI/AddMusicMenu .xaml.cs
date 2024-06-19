using System.Collections.Generic;
using System.IO;
using System.Windows;
using DataBaseActions;
using DataBaseActions.Music;
using Microsoft.Win32;
using MusicAPI;

namespace KostPostMusic
{
    public partial class AddMusicMenu : Window
    {
        private bool isAddingAlbum;

        public AddMusicMenu()
        {
            InitializeComponent();
        }

        private void SingleSongRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isAddingAlbum = false;
            AlbumNameTextBlockLabel.Visibility = Visibility.Collapsed;
            AlbumNameTextBox.Visibility = Visibility.Collapsed;
        }

        private void AlbumRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isAddingAlbum = true;
            AlbumNameTextBlockLabel.Visibility = Visibility.Visible;
            AlbumNameTextBox.Visibility = Visibility.Visible;
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = isAddingAlbum ? "MP3 Files (*.mp3)|*.mp3|All Files (*.*)|*.*" : "MP3 Files (*.mp3)|*.mp3";
            openFileDialog.Multiselect = isAddingAlbum;

            if (openFileDialog.ShowDialog() == true)
            {
                if (isAddingAlbum)
                {
                    SelectedFileTextBlock.Text = $"{openFileDialog.FileNames.Length} files selected";
                }
                else
                {
                    SelectedFileTextBlock.Text = openFileDialog.FileName;
                }
            }
        }

        private async void AddMusicButton_Click(object sender, RoutedEventArgs e)
        {
            // Get input values (musicName, authors, albumName) from UI elements
            string musicName = MusicNameTextBox.Text;
            string authors = AuthorsTextBox.Text;
            string albumName = isAddingAlbum ? AlbumNameTextBox.Text : string.Empty;

            // Create instances of MusicService and AzureBlobs
            using (var dbContext = new KostPostMusicContext())
            {
                var azureBlobs = new AzureBlobs();
                var musicService = new MusicService(dbContext, azureBlobs);

                // Add the music file(s)
                if (isAddingAlbum)
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*",
                        Multiselect = true
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        var filePaths = openFileDialog.FileNames;
                        await musicService.AddAlbumAsync(musicName, albumName, authors, filePaths);
                    }
                }
                else
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*",
                        Multiselect = false
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        string filePath = openFileDialog.FileName;
                        await musicService.AddMusicFileAsync(musicName, albumName, authors, filePath);
                    }
                }
            }

            // Optionally close the menu or perform other UI updates
            Close();
        }
    }
}