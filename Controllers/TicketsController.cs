using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly DataContext _context;

    List<Ticket> Tickets = new List<Ticket>();


    public TicketsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Ticket>> GetTickets(string? name = null)
    {
        var query = _context.Tickets!.AsQueryable();

        if (name != null)
            query = query.Where(x => x.SeatNo != null && x.SeatNo.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetTicket(int id)
    {
        var ticket = _context.Tickets!.Find(id);

        if (ticket == null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

	[HttpPut("{id}")]
	public IActionResult PutTicket(int id, Ticket ticket)
	{
		if (id != ticket.Id)
			return BadRequest();

		var existing = _context.Tickets!
			.Include(t => t.Session)
			.FirstOrDefault(t => t.Id == id);

		if (existing == null)
			return NotFound();

		if (!_context.Sessions!.Any(s => s.Id == ticket.SessionId))
			return NotFound("Session not found");

		if (ticket.Price <= 0)
			return BadRequest("Price must be positive");

		if (_context.Tickets!.Any(t =>
			t.SessionId == ticket.SessionId &&
			t.SeatNo == ticket.SeatNo))
			return BadRequest("Seat number must be unique");

		existing.SessionId = ticket.SessionId;
		existing.SeatNo = ticket.SeatNo;
		existing.Price = ticket.Price;

		_context.SaveChanges();
		return NoContent();
	}

	[HttpPost]
	public ActionResult<Ticket> PostTicket(Ticket ticket)
	{
		if (ticket.Id != 0)
			return BadRequest("Id should not be provided");

		if (!_context.Sessions!.Any(s => s.Id == ticket.SessionId))
			return BadRequest("Session not found");

		if (ticket.Price <= 0)
			return BadRequest("Price must be positive");

		if (_context.Tickets!.Any(t =>
			t.SessionId == ticket.SessionId &&
			t.SeatNo == ticket.SeatNo))
			return BadRequest("Seat number must be unique");

		_context.Tickets!.Add(ticket);
		_context.SaveChanges();
		return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
	}

	[HttpDelete("{id}")]
	public IActionResult DeleteTicket(int id)
	{
		var ticket = _context.Tickets!.Find(id);
		if (ticket == null)
			return NotFound();

		_context.Tickets.Remove(ticket);
		_context.SaveChanges();
		return NoContent();
	}
}
