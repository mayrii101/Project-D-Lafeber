// Models/StickyNote.cs
namespace ProjectD.Models
{
    public class StickyNote
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }

    public class StickyNoteDto
    {
        public string Content { get; set; } = string.Empty;
    }
}
