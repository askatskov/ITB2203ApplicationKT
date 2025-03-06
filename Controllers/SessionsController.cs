using ITB2203Application.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly DataContext _context;
    List<Session> Sessions = new List<Session>();

    public SessionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Session>> GetSessions(DateTime? periodStart = null, DateTime? periodEnd = null, string? name = null)
    {
        var filteredSession = Sessions.AsQueryable();

        if (periodStart.HasValue)
            filteredSession = filteredSession.Where(s => s.StartTime >= periodStart.Value);

        if (periodEnd.HasValue)
            filteredSession = filteredSession.Where(s => s.StartTime <= periodEnd.Value);

        return Ok(filteredSession);
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetSession(int id)
    {
        var test = _context.Sessions!.Find(id);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

    [HttpPut("{id}")]
    public IActionResult PutSession(int id, Session session)
    {
        var dbSession = _context.Sessions!.AsNoTracking().FirstOrDefault(x => x.Id == session.Id);
        if (id != session.Id || dbSession == null)
        {
            return NotFound();
        }

        if (session.StartTime.Date < DateTime.Now.Date)
        {
            return BadRequest("Seansi algusaeg peab olema tulevikus");
        }

        _context.Update(session);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Session> PostSession(Session session)
    {
        var dbExercise = _context.Sessions!.Find(session.Id);
        if (dbExercise == null)
        {
            _context.Add(session);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSession), new { Id = session.Id }, session);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSession(int id)
    {
        var test = _context.Sessions!.Find(id);
        if (test == null)
        {
            return NotFound();
        }

        _context.Remove(test);
        _context.SaveChanges();

        return NoContent();
    }
}
