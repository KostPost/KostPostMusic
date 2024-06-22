using System;
using System.Linq;
using System.Windows;
using ClassesData;
using ClassesData.Music;
using DataBaseActions;
using MusicAPI;

namespace KostPostMusic
{
    public partial class DeleteMusicWindow : Window
    {
        private Account _account;

        public DeleteMusicWindow(Account account)
        {
            InitializeComponent();
            _account = account;
            LoadUserMusic();
        }

        private void LoadUserMusic()
        {
            using (var context = new KostPostMusicContext())
            {
                var userMusic = context.MusicFiles
                    .Where(m => m.AuthorID == _account.Id)
                    .ToList();

                musicListBox.ItemsSource = userMusic;
                musicListBox.DisplayMemberPath = "FileName";
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
{
    if (musicListBox.SelectedItem == null)
    {
        MessageBox.Show("Please select a music file to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    if (!(musicListBox.SelectedItem is MusicData selectedMusic))
    {
        MessageBox.Show("Invalid selection. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    if (string.IsNullOrWhiteSpace(selectedMusic.FileName))
    {
        MessageBox.Show("The selected music file has an invalid name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    var result = MessageBox.Show($"Are you sure you want to delete '{selectedMusic.FileName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

    if (result == MessageBoxResult.Yes)
    {
        try
        {
            using (var context = new KostPostMusicContext())
            {
                var musicToDelete = await context.MusicFiles.FindAsync(selectedMusic.Id);
                if (musicToDelete == null)
                {
                    MessageBox.Show("The selected music file no longer exists in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadUserMusic(); // Refresh the list
                    return;
                }

                if (musicToDelete.AuthorID != _account.Id)
                {
                    MessageBox.Show("You don't have permission to delete this music file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                context.MusicFiles.Remove(musicToDelete);
                await context.SaveChangesAsync();
            }

            // Delete from Azure Blob Storage
            var azureBlobs = new AzureBlobs();
            bool blobDeleted = await azureBlobs.DeleteBlobAsync(selectedMusic.FileName);

            if (!blobDeleted)
            {
                MessageBox.Show("The music file was removed from the database, but could not be deleted from storage. Please contact support.", "Partial Success", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("Music deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadUserMusic(); // Refresh the list
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting music: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
    }
}