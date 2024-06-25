public class SearchResult
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string FileName { get; set; }
    public bool IsCurrentlyPlaying { get; set; }
    public string AuthorName { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime DateAdded { get; set; }
    public string FormattedDuration => Duration.ToString(@"mm\:ss"); // Add FormattedDuration
}