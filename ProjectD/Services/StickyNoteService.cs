using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Services
{
    public interface IStickyNoteService
    {
        Task<StickyNote?> GetNoteAsync();
        Task<StickyNote> SaveNoteAsync(string content);
    }

    public class StickyNoteService : IStickyNoteService
    {
        private readonly ApplicationDbContext _context;

        public StickyNoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StickyNote?> GetNoteAsync()
        {
            return await _context.StickyNotes.FirstOrDefaultAsync();
        }

        public async Task<StickyNote> SaveNoteAsync(string content)
        {
            var note = await _context.StickyNotes.FirstOrDefaultAsync();

            if (note == null)
            {
                note = new StickyNote { Content = content };
                _context.StickyNotes.Add(note);
            }
            else
            {
                note.Content = content;
                note.LastModified = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return note;
        }
    }
}
