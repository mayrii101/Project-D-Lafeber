using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;

namespace ProjectD.Controllers
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
        public async Task<ActionResult<StickyNoteDto>> GetNote()
        {
            var note = await _noteService.GetNoteAsync();
            return Ok(new StickyNoteDto { Content = note?.Content ?? "" });
        }

        [HttpPost]
        public async Task<ActionResult> SaveNote([FromBody] string content)
        {
            await _noteService.SaveNoteAsync(content);
            return NoContent();
        }
    }
}
