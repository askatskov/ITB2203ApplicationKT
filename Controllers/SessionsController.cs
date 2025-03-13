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

	[HttpGet("{id}")]
	public ActionResult<Session> GetSession(int id)
	{
		var session = _context.Sessions!.Find(id);

		if (session == null)
		{
			return NotFound();
		}

		return Ok(session);
	}

	[HttpGet]
	public ActionResult<IEnumerable<Session>> GetSessions(DateTime? periodStart, DateTime? periodEnd, string? movieTitle)
	{
		var query = _context.Sessions!.AsQueryable();

		if (periodStart.HasValue)
			query = query.Where(s => s.StartTime >= periodStart.Value);

		if (periodEnd.HasValue)
			query = query.Where(s => s.StartTime <= periodEnd.Value);

		if (!string.IsNullOrEmpty(movieTitle))
			query = query.Where(s => _context.Movies!.Any(m => m.Id == s.MovieId && m.Title == movieTitle));

		return query.ToList();
	}

	[HttpGet]
	[Route("{id}/tickets")]
	public ActionResult<IEnumerable<Ticket>> GetSessionTickets(int id)
	{
		var session = _context.Sessions!.Find(id);

		if (session == null)
		{
			return NotFound();
		}

		List<Ticket>? sessionTickets = _context.Tickets!.AsNoTracking().Where(x => x.SessionId == session.Id).ToList();

		if (sessionTickets == null)
		{
			return NotFound();
		}

		return Ok(sessionTickets);
	}

	[HttpPut("{id}")]
    public IActionResult PutSession(int id, Session session)
    {
        var dbSession = _context.Sessions!.AsNoTracking().FirstOrDefault(x => x.Id == session.Id);
        if (id != session.Id || dbSession == null)
        {
            return NotFound();
        }

		DateTime currentTime = DateTime.Now;
		int dateComparison = DateTime.Compare(session.StartTime, currentTime);

		if (dateComparison <= 0)
		{
            return BadRequest("Seansi algusaeg peab olema tulevikus");
        }

		Movie? sessionMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == session.MovieId);
		if (sessionMovie == null)
		{
			return BadRequest();
		}

		_context.Update(session);
        _context.SaveChanges();

        return NoContent();
    }

	[HttpPost]
	public ActionResult<Session> PostSession(Session session)
	{
		if (_context.Sessions!.Any())
		{
			int maxSessionId = _context.Sessions!.Max(x => x.Id);
			session.Id = maxSessionId + 1;
		}

		DateTime currentTime = DateTime.Now;
		int dateComparison = DateTime.Compare(session.StartTime, currentTime);

		if (dateComparison <= 0)
		{
			return BadRequest();
		}

		Movie? sessionMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == session.MovieId);
		if (sessionMovie == null)
		{
			return BadRequest();
		}

		_context.Add(session);
		_context.SaveChanges();

		return CreatedAtAction(nameof(GetSession), new { Id = session.Id }, session);
	}

	[HttpDelete("{id}")]
	public IActionResult DeleteSession(int id)
	{
		var session = _context.Sessions!.Find(id);
		if (session == null)
		{
			return NotFound();
		}

		_context.Remove(session);
		_context.SaveChanges();

		return Ok();
	}
}

