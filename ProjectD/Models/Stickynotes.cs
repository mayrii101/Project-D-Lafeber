public class StickyNote
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = "";

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
