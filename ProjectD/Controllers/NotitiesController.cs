using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StickyNoteController : ControllerBase
    {
        private readonly IStickyNoteService _noteService;

        public StickyNoteController(IStickyNoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public async Task<ActionResult<StickyNote>> Get()
        {
            var note = await _noteService.GetNoteAsync();
            return Ok(note);
        }

        [HttpPost]
        public async Task<ActionResult<StickyNote>> Save([FromBody] string content)
        {
            var updated = await _noteService.SaveNoteAsync(content);
            return Ok(updated);
        }
    }
}