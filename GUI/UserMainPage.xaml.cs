    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using ClassesData;
    using ClassesData.Music;
    using DataBaseActions;
    using Microsoft.EntityFrameworkCore;
    using MusicAPI;

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
        private LinkedList<SearchResult> musicQueue = new LinkedList<SearchResult>();
        private LinkedListNode<SearchResult> currentSong;
        
        private LinkedList<SearchResult> playListSongs = new LinkedList<SearchResult>();
        
        private LinkedList<SearchResult> originalPlaylist = new LinkedList<SearchResult>();


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

            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;



            UpdateCurrentMusicDisplay();

            LoadUserPlaylists();

        }


        private async void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (currentSong != null)
                {
                    if (currentSong.Next != null)
                    {
                        currentSong = currentSong.Next;
                    }
                    else
                    {
                        currentSong = musicQueue.First;
                    }
                    await PlayCurrentSong();
                }
            });
        }
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

            PlaylistsListBox.ItemsSource = null; 
            PlaylistsListBox.ItemsSource = userPlaylists;
        }
        private void TrackOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var track = button.DataContext as SearchResult;

            ContextMenu contextMenu = new ContextMenu();
            MenuItem addToPlaylistItem = new MenuItem { Header = "Add to Playlist" };
            addToPlaylistItem.Click += (s, args) => ShowAddToPlaylistDialog(track);
            contextMenu.Items.Add(addToPlaylistItem);

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
                    
                        if (dbPlaylist.SongAddedTimes == null)
                        {
                            dbPlaylist.SongAddedTimes = new Dictionary<int, DateTime>();
                        }
                        dbPlaylist.SongAddedTimes[track.Id] = DateTime.UtcNow;
                
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


        private async void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistsListBox.SelectedItem is Playlist selectedPlaylist)
            {
                await DisplayPlaylistSongs(selectedPlaylist);
            }
        }


        private async Task DisplayPlaylistSongs(Playlist playlist)
        {
            using (var context = new KostPostMusicContext())
            {
                var playlistSongs = new List<SearchResult>();

                foreach (var songId in playlist.SongIds)
                {
                    var musicFile = await context.MusicFiles.FirstOrDefaultAsync(m => m.Id == songId);
                    if (musicFile != null)
                    {
                        playlistSongs.Add(new SearchResult
                        {
                            Id = musicFile.Id,
                            DisplayName = $"{musicFile.AuthorName} - {Path.GetFileNameWithoutExtension(musicFile.FileName)}",
                            FileName = musicFile.FileName,
                            IsCurrentlyPlaying = false,
                            AuthorName = musicFile.AuthorName,
                            Duration = musicFile.Duration,
                            DateAdded = playlist.SongAddedTimes.ContainsKey(musicFile.Id) ? playlist.SongAddedTimes[musicFile.Id] : DateTime.MinValue,
                        });
                    }
                }
                musicQueue = new LinkedList<SearchResult>(playlistSongs);
                originalPlaylist = new LinkedList<SearchResult>(playlistSongs);
                
                UpdatePlaylistView();
                PlaylistSongsListBox.Visibility = Visibility.Visible;
            }
        }
        private void UpdatePlaylistView()
        {
            PlaylistSongsListBox.ItemsSource = null;
            PlaylistSongsListBox.ItemsSource = originalPlaylist;
        }

        private async Task SortPlaylistSongs(Playlist playlist)
        {
            var sortOption = (SortOptionsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            using (var context = new KostPostMusicContext())
            {
                var musicFiles = await context.MusicFiles.ToListAsync();

                switch (sortOption)
                {
                    case "Sort by Date Added":
                        playlist.SongIds = playlist.SongIds.OrderBy(id => playlist.SongAddedTimes[id]).ToList();
                        break;
                    case "Sort by Author Name":
                        playlist.SongIds = playlist.SongIds.OrderBy(id => musicFiles.First(m => m.Id == id).AuthorName).ToList();
                        break;
                    case "Sort by Duration":
                        playlist.SongIds = playlist.SongIds.OrderBy(id => musicFiles.First(m => m.Id == id).Duration).ToList();
                        break;
                    case "Sort by ID":
                        playlist.SongIds = playlist.SongIds.OrderBy(id => id).ToList();
                        break;
                    default:
                        // Default sorting (e.g., by Date Added)
                        playlist.SongIds = playlist.SongIds.OrderBy(id => playlist.SongAddedTimes[id]).ToList();
                        break;
                }

                // Update the playlist in the database
                context.Playlists.Update(playlist);
                await context.SaveChangesAsync();
            }

            await DisplayPlaylistSongs(playlist);
        }
        private async void SortOptionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistsListBox.SelectedItem is Playlist selectedPlaylist)
            {
                await SortPlaylistSongs(selectedPlaylist);
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
        private async void PlaylistSongsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistSongsListBox.SelectedItem is SearchResult selectedItem)
            {
                currentSong = musicQueue.Find(selectedItem);
                await PlayCurrentSong();
            }
        }
        
        private void PlaylistSongOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var track = button.DataContext as SearchResult;

            ContextMenu contextMenu = new ContextMenu();

            MenuItem deleteFromPlaylistItem = new MenuItem { Header = "Delete from Playlist" };
            deleteFromPlaylistItem.Click += (s, args) => DeleteFromPlaylist(track);
            contextMenu.Items.Add(deleteFromPlaylistItem);

            MenuItem addToQueueItem = new MenuItem { Header = "Add to Queue" };
            addToQueueItem.Click += (s, args) => AddToQueue(track);
            contextMenu.Items.Add(addToQueueItem);

            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
        }

        private async void DeleteFromPlaylist(SearchResult track)
        {
            if (PlaylistsListBox.SelectedItem is Playlist selectedPlaylist)
            {
                using (var context = new KostPostMusicContext())
                {
                    var playlist = await context.Playlists
                        .FirstOrDefaultAsync(p => p.Id == selectedPlaylist.Id);

                    if (playlist != null)
                    {
                        if (playlist.SongIds.Contains(track.Id))
                        {
                            playlist.SongIds.Remove(track.Id);
                        }

                        if (playlist.SongAddedTimes != null && playlist.SongAddedTimes.ContainsKey(track.Id))
                        {
                            playlist.SongAddedTimes.Remove(track.Id);
                        }

                        context.Entry(playlist).State = EntityState.Modified;

                        await context.SaveChangesAsync();
                    }
                }
                musicQueue.Remove(track);

                playListSongs.Remove(track);

                if (originalPlaylist != null)
                {
                    originalPlaylist.Remove(track);
                }

                UpdatePlaylistView();

                if (currentSong != null && currentSong.Value.Id == track.Id)
                {
                    await PlayNextSong();
                }

                MessageBox.Show($"Removed '{track.DisplayName}' from playlist '{selectedPlaylist.Name}'", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadUserPlaylists();
            }
        }



        private void AddToQueue(SearchResult track)
        {
            if (currentSong != null)
            {
                musicQueue.AddAfter(musicQueue.Find(currentSong.Value), track);
            }
            else
            {
                musicQueue.AddFirst(track);
            }

            UpdatePlaylistView();
        }
       
        
        private async Task PlayCurrentSong()
        {
            if (currentSong != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    foreach (var song in musicQueue)
                    {
                        song.IsCurrentlyPlaying = false;
                    }

                    currentSong.Value.IsCurrentlyPlaying = true;

                    PlaylistSongsListBox.Items.Refresh();

                    await PlayMusicAsync(currentSong.Value.FileName);
                    
                    while (musicQueue.First != currentSong)
                    {
                        var songToRemove = musicQueue.First.Value;
                        if (!originalPlaylist.Contains(songToRemove))
                        {
                            musicQueue.RemoveFirst();
                        }
                        else
                        {
                            break;
                        }
                    }
                });
            }
        }
        private async Task PlayNextSong()
        {
            if (currentSong != null)
            {
                currentSong.Value.IsCurrentlyPlaying = false;
                if (currentSong.Next != null)
                {
                    currentSong = currentSong.Next;
                }
                else
                {
                    currentSong = musicQueue.First;
                }
                await PlayCurrentSong();
            }
        }

        private async Task PlayPreviousSong()
        {
            if (currentSong != null)
            {
                currentSong.Value.IsCurrentlyPlaying = false;
                var currentInOriginal = originalPlaylist.Find(currentSong.Value);
                if (currentInOriginal != null && currentInOriginal.Previous != null)
                {
                    var prevSong = currentInOriginal.Previous.Value;
                    currentSong = musicQueue.Find(prevSong);
                }
                else
                {
                    currentSong = musicQueue.Last;
                }
                await PlayCurrentSong();
            }
        }
       private async Task PlayMusicAsync(string trackName)
    {
        try
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    AzureBlobs azureBlobs = new AzureBlobs();
                    string sasUrl = azureBlobs.GetBlobSasUri(trackName);

                    mediaPlayer.Stop();

                    mediaPlayer.Open(new Uri(sasUrl, UriKind.Absolute));
                    
                    var openMediaTaskCompletionSource = new TaskCompletionSource<bool>();
                    mediaPlayer.MediaOpened += (s, e) => openMediaTaskCompletionSource.TrySetResult(true);
                    mediaPlayer.MediaFailed += (s, e) => openMediaTaskCompletionSource.TrySetException(new Exception("Media failed to open"));
                    
                    await openMediaTaskCompletionSource.Task;

                    mediaPlayer.Play();
                    
                    UpdatePlayPauseButton(true);
                    PlayPauseButton.Content = "Pause";

                    MusicSlider.Value = 0;
                    UpdateTimeDisplay();

                    timer.Start();

                    using (var context = new KostPostMusicContext())
                    {
                        currentMusic = await context.MusicFiles.FirstOrDefaultAsync(m => m.FileName == trackName);
                    }

                    UpdateCurrentMusicDisplay();

                    Console.WriteLine($"Now playing: {trackName}"); // Debug output
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in UI thread: {ex.Message}", ex);
                }
            });
        }
        catch (Exception ex)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show($"Failed to play music: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
            Console.WriteLine($"Error playing {trackName}: {ex.Message}"); // Debug output
        }
    }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePlayPause();
        }
        
        private async void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            await Application.Current.Dispatcher.InvokeAsync(PlayPreviousSong);
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            await Application.Current.Dispatcher.InvokeAsync(PlayNextSong);
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!isDraggingSlider)
                    {
                        MusicSlider.Value = mediaPlayer.Position.TotalSeconds;
                    }
                    UpdateTimeDisplay();
                });
            }
        }
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