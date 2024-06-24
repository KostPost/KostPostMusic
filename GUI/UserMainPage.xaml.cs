using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ClassesData;
using ClassesData.Music;
using DataBaseActions;
using Microsoft.EntityFrameworkCore;
using MusicAPI;
using Track = System.Windows.Controls.Primitives.Track;

namespace KostPostMusic;

public partial class UserMainPage : Window
{
    private Account _account;
    private bool isMenuOpen = false;
    private MediaPlayer mediaPlayer = new MediaPlayer();
    private DispatcherTimer timer;
    private bool isPlaying = false;

    private MusicData currentMusic;
    private List<Playlist> userPlaylists;


    public UserMainPage(Account account)
    {
        InitializeComponent();
        _account = account;
        UpdateButtonContent();

        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        timer.Tick += Timer_Tick;

        mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

        UpdateCurrentMusicDisplay();

        LoadUserPlaylists();
    }


    // private async void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
    // {
    //     // Prompt the user for playlist name
    //     var dialog = new InputDialog("Create Playlist", "Enter playlist name:");
    //     if (dialog.ShowDialog() == true)
    //     {
    //         string playlistName = dialog.ResponseText;
    //
    //         if (!string.IsNullOrWhiteSpace(playlistName))
    //         {
    //             // Create a new playlist
    //             var newPlaylist = new Playlist
    //             {
    //                 Name = playlistName,
    //                 Description = "", // You can add a description input if needed
    //                 CreatedBy = _account.Id
    //             };
    //
    //             // Save the playlist to the database
    //             using (var context = new KostPostMusicContext())
    //             {
    //                 context.Playlists.Add(newPlaylist);
    //                 await context.SaveChangesAsync();
    //             }
    //
    //             // Refresh the playlists list
    //             await LoadUserPlaylists();
    //         }
    //     }
    // }

    
    private async void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new InputDialog("Create Playlist", "Enter playlist name:");
        if (dialog.ShowDialog() == true)
        {
            string playlistName = dialog.ResponseText;

            if (!string.IsNullOrWhiteSpace(playlistName))
            {
                var newPlaylist = new Playlist
                {
                    Name = playlistName,
                    CreatedBy = _account.Id,
           
                };

                using (var context = new KostPostMusicContext())
                {
                    context.Playlists.Add(newPlaylist);
                    await context.SaveChangesAsync();
                }

                await LoadUserPlaylists();
            }
        }
    }

    public async Task LoadUserPlaylists()
    {
        using (var context = new KostPostMusicContext())
        {
            userPlaylists = await context.Playlists
                .Where(p => p.CreatedBy == _account.Id)
                .ToListAsync();
        }

        PlaylistsListBox.ItemsSource = null; // Clear the current items
        PlaylistsListBox.ItemsSource = userPlaylists;
    }
    private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        var contextMenu = menuItem.Parent as ContextMenu;
        var textBlock = contextMenu.PlacementTarget as TextBlock;
        var selectedTrack = textBlock.DataContext as SearchResult;

        if (selectedTrack != null)
        {
            ShowAddToPlaylistDialog(selectedTrack);
        }
    }
    private void TrackOptionsButton_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var track = button.DataContext as SearchResult;

        ContextMenu contextMenu = new ContextMenu();
        MenuItem addToPlaylistItem = new MenuItem { Header = "Add to Playlist" };
        addToPlaylistItem.Click += (s, args) => ShowAddToPlaylistDialog(track);
        contextMenu.Items.Add(addToPlaylistItem);

        // You can add more menu items here for other actions

        contextMenu.PlacementTarget = button;
        contextMenu.IsOpen = true;
    }
    private void ShowAddToPlaylistDialog(SearchResult track)
    {
        var dialog = new AddToPlaylistDialog(userPlaylists);
        if (dialog.ShowDialog() == true)
        {
            var selectedPlaylist = dialog.SelectedPlaylist;
            if (selectedPlaylist != null)
            {
                AddTrackToPlaylist(selectedPlaylist, track);
            }
        }
    }
    private async void AddTrackToPlaylist(Playlist playlist, SearchResult track)
    {
        using (var context = new KostPostMusicContext())
        {
            var dbPlaylist = await context.Playlists.FindAsync(playlist.Id);
            if (dbPlaylist != null)
            {
                if (!dbPlaylist.SongIds.Contains(track.Id))
                {
                    dbPlaylist.SongIds.Add(track.Id);
                
                    context.Entry(dbPlaylist).State = EntityState.Modified;

                    try
                    {
                        await context.SaveChangesAsync();
                        MessageBox.Show($"Added '{track.DisplayName}' to playlist '{playlist.Name}'", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        if (PlaylistsListBox.SelectedItem is Playlist selectedPlaylist &&
                            selectedPlaylist.Id == playlist.Id)
                        {
                            await DisplayPlaylistSongs(selectedPlaylist);
                        }

                        await LoadUserPlaylists();
                    }
                    catch (Exception ex)
                    {
                        var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        MessageBox.Show($"Error: {innerException}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("This track is already in the playlist.", "Information", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }
    }





    private void SearchResults_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var listBoxItem = VisualTreeHelper.GetParent(e.OriginalSource as DependencyObject) as ListBoxItem;
        if (listBoxItem != null)
        {
            listBoxItem.IsSelected = true;
            e.Handled = true;
        }
    }

    private void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PlaylistsListBox.SelectedItem is Playlist selectedPlaylist)
        {
            // Load and display the songs in the selected playlist
            DisplayPlaylistSongs(selectedPlaylist);
        }
    }


    private async Task DisplayPlaylistSongs(Playlist playlist)
    {
        using (var context = new KostPostMusicContext())
        {
            var playlistSongs = await context.MusicFiles
                .Where(m => playlist.SongIds.Contains(m.Id))
                .Select(m => new SearchResult
                {
                    Id = m.Id,
                    DisplayName = $"{m.AuthorName} - {Path.GetFileNameWithoutExtension(m.FileName)}",
                    FileName = m.FileName
                })
                .ToListAsync();

            PlaylistSongsListBox.ItemsSource = playlistSongs;
            PlaylistSongsListBox.Visibility = Visibility.Visible;
        }
    }


    private void AddMusicButton_Click(object sender, RoutedEventArgs e)
    {
        AddMusicMenu addMusicMenu = new AddMusicMenu(_account);
        addMusicMenu.Owner = this;
        addMusicMenu.ShowDialog();
    }

    private void DeleteMusicButton_Click(object sender, RoutedEventArgs e)
    {
        DeleteMusicWindow deleteMusicWindow = new DeleteMusicWindow(_account);
        deleteMusicWindow.Owner = this;
        deleteMusicWindow.ShowDialog();
    }

    private ListBox GetSearchResultsListBox()
    {
        return this.FindName("SearchResults") as ListBox;
    }

    private async void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchTerm = SearchInput.Text.Trim().ToLower();
        if (string.IsNullOrEmpty(searchTerm))
        {
            SearchResults.Visibility = Visibility.Collapsed;
            return;
        }

        using (var context = new KostPostMusicContext())
        {
            var results = await context.MusicFiles
                .Where(m => m.FileName.ToLower().Contains(searchTerm) || m.AuthorName.ToLower().Contains(searchTerm))
                .Select(m => new SearchResult
                {
                    Id = m.Id,
                    DisplayName = $"{m.AuthorName} - {System.IO.Path.GetFileNameWithoutExtension(m.FileName)}",
                    FileName = m.FileName
                })
                .ToListAsync();

            SearchResults.ItemsSource = results;
            SearchResults.Visibility = results.Any() ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private async void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SearchResults.SelectedItem is SearchResult selectedItem)
        {
            string fileName = selectedItem.FileName;
            await PlayMusicAsync(fileName);
            SearchInput.Text = string.Empty;
            SearchResults.Visibility = Visibility.Collapsed;
        }
    }

    private async Task PlayMusicAsync(string trackName)
    {
        try
        {
            AzureBlobs azureBlobs = new AzureBlobs();
            string sasUrl = azureBlobs.GetBlobSasUri(trackName);
            mediaPlayer.Open(new Uri(sasUrl, UriKind.Absolute));
            mediaPlayer.Play();
            UpdatePlayPauseButton(true);

            MusicSlider.Value = 0;
            UpdateTimeDisplay();

            // Start the timer
            timer.Start();

            // Update current music
            using (var context = new KostPostMusicContext())
            {
                currentMusic = await context.MusicFiles.FirstOrDefaultAsync(m => m.FileName == trackName);
            }

            UpdateCurrentMusicDisplay();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to play music: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {
        TogglePlayPause();
    }

    private void TogglePlayPause()
    {
        if (isPlaying)
        {
            mediaPlayer.Pause();
            PlayPauseButton.Content = "Play";
            timer.Stop();
        }
        else
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Play();
                PlayPauseButton.Content = "Pause";
                timer.Start();
            }
            else
            {
                MessageBox.Show("No music selected. Please choose a track to play.", "No Music", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
        }

        isPlaying = !isPlaying;
    }

    private void UpdatePlayPauseButton(bool playing)
    {
        PlayPauseButton.Content = playing ? "Pause" : "Play";
        isPlaying = playing;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            if (!isDraggingSlider)
            {
                MusicSlider.Value = mediaPlayer.Position.TotalSeconds;
            }

            UpdateTimeDisplay();
        }
    }

    private void MediaPlayer_MediaOpened(object sender, EventArgs e)
    {
        if (mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            MusicSlider.Minimum = 0;
            MusicSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            MusicSlider.SmallChange = 1;
            MusicSlider.LargeChange = Math.Min(10, mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds / 10);
        }

        UpdateTimeDisplay();
    }

    // private void AttachSliderThumbEvents()
    // {
    //     var track = MusicSlider.Template.FindName("PART_Track", MusicSlider) as Track;
    //     if (track != null)
    //     {
    //         var thumb = track.Thumb;
    //         if (thumb != null)
    //         {
    //             thumb.DragStarted += MusicSlider_DragStarted;
    //             thumb.DragCompleted += MusicSlider_DragCompleted;
    //         }
    //     }
    // }


    private bool isDraggingSlider = false;

    private void MusicSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        isDraggingSlider = true;
    }

    private void MusicSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        isDraggingSlider = false;
        mediaPlayer.Position = TimeSpan.FromSeconds(MusicSlider.Value);
    }

    private void MusicSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!isDraggingSlider)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            CurrentTimeTextBlock.Text = mediaPlayer.Position.ToString(@"mm\:ss");
            TotalTimeTextBlock.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }
        else
        {
            CurrentTimeTextBlock.Text = "00:00";
            TotalTimeTextBlock.Text = "00:00";
        }
    }

    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        mediaPlayer.Volume = e.NewValue;
    }

    private void UpdateButtonContent()
    {
        UsernameButton.Content = $"{_account.Username} \u25BC"; // Unicode for down arrow
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

    private void UpdateCurrentMusicDisplay()
    {
        if (currentMusic != null)
        {
            CurrentMusicTextBlock.Text =
                $"Now Playing: {currentMusic.AuthorName} - {System.IO.Path.GetFileNameWithoutExtension(currentMusic.FileName)}";
        }
        else
        {
            CurrentMusicTextBlock.Text = "No music playing";
        }
    }

    private void Profile_Click(object sender, RoutedEventArgs e)
    {
        // Add logic for the "Profile" action here
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        // Add logic for the "Settings" action here
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        App.ClearCredentials();

        App.RestartApplication();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateButtonContent();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!isMenuOpen)
            UpdateButtonContent();
    }
}