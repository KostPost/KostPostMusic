using System.Collections.Generic;
using System.Windows;
using ClassesData.Music;

namespace KostPostMusic
{
    public partial class AddToPlaylistDialog : Window
    {
        public Playlist SelectedPlaylist { get; private set; }

        public AddToPlaylistDialog(List<Playlist> playlists)
        {
            InitializeComponent();
            PlaylistListBox.ItemsSource = playlists;
            PlaylistListBox.DisplayMemberPath = "Name";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPlaylist = PlaylistListBox.SelectedItem as Playlist;
            if (SelectedPlaylist != null)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a playlist.", "No Playlist Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}