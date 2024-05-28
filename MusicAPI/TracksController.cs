using Microsoft.AspNetCore.Mvc;

namespace MusicAPI;

[ApiController]
[Route("api/[controller]")]
public class TracksController : ControllerBase
{
    
    private readonly List<Track> _tracks = new List<Track>
    {
        new Track { Id = 1, Title = "Song 1", Artist = "Artist 1", Duration = "3:45" },
        new Track { Id = 2, Title = "Song 2", Artist = "Artist 2", Duration = "4:20" },
    };

    [HttpGet]
    public ActionResult<IEnumerable<Track>> Get()
    {
        return Ok(_tracks);
    }

    [HttpGet("{id}")]
    public ActionResult<Track> Get(int id)
    {
        var track = _tracks.FirstOrDefault(t => t.Id == id);
        if (track == null)
        {
            return NotFound();
        }
        return Ok(track);
    }
}
