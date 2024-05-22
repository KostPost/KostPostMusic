using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using ClassesData;


namespace KostPostMusic;

public partial class MainWindow : Window
{
    private UserAccount UserAccount;
    private bool isMenuOpen = false;
    private MediaPlayer mediaPlayer = new MediaPlayer();
    private DispatcherTimer timer;
    private bool isPlaying = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(UserAccount userAccount)
    {
        InitializeComponent();
        UserAccount = userAccount;
        UpdateButtonContent();

        mediaPlayer.Open(new Uri(@"P:\KostPostMusic\Music\kish.mp3", UriKind.RelativeOrAbsolute));
        mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(100); // Update the slider every 100 milliseconds
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (!isDraggingSlider)
        {
            MusicSlider.Value = mediaPlayer.Position.TotalMilliseconds;
        }
        CurrentTimeTextBlock.Text = mediaPlayer.Position.ToString(@"mm\:ss");
    }

    private void MediaPlayer_MediaOpened(object sender, EventArgs e)
    {
        MusicSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
        timer.Start();
        AttachSliderThumbEvents();
        TotalTimeTextBlock.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
    }

    private void AttachSliderThumbEvents()
    {
        var track = MusicSlider.Template.FindName("PART_Track", MusicSlider) as Track;
        if (track != null)
        {
            var thumb = track.Thumb;
            if (thumb != null)
            {
                thumb.DragStarted += MusicSlider_DragStarted;
                thumb.DragCompleted += MusicSlider_DragCompleted;
            }
        }
    }
    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (isPlaying)
        {
            mediaPlayer.Pause();
            PlayPauseButton.Content = "Play";
        }
        else
        {
            mediaPlayer.Play();
            PlayPauseButton.Content = "Pause";
        }
        isPlaying = !isPlaying;
    }

    private bool isDraggingSlider = false;

    private void MusicSlider_DragStarted(object sender, DragStartedEventArgs e)
    {
        isDraggingSlider = true;
    }

    private void MusicSlider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        isDraggingSlider = false;
        mediaPlayer.Position = TimeSpan.FromMilliseconds(MusicSlider.Value);
    }

    private void MusicSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!isDraggingSlider)
        {
            mediaPlayer.Position = TimeSpan.FromMilliseconds(e.NewValue);
        }
    }

    private void UpdateButtonContent()
    {
        UsernameButton.Content = $"{UserAccount.Username} \u25BC"; // Unicode for down arrow
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