using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly DataContext _context;

    public MoviesController(DataContext context)
    {
        _context = context;
    }

	[HttpGet]
	public ActionResult<IEnumerable<Movie>> GetMovies(string? Title = null)
	{
		var query = _context.Movies!.AsQueryable();

		if (!string.IsNullOrEmpty(Title))
			query = query.Where(x => x.Title != null && x.Title.ToLower().Contains(Title.ToLower()));

		return query.ToList();
	}

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetMovie(int id)
    {
        var test = _context.Movies!.Find(id);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

	[HttpPut("{id}")]
	public IActionResult PutMovie(int id, Movie movie)
	{
		if (id != movie.Id)
			return BadRequest();

		var existing = _context.Movies!.Find(id);
		if (existing == null)
			return NotFound();

		existing.Title = movie.Title;
		_context.SaveChanges();
		return NoContent();
	}

	[HttpPost]
	public ActionResult<Movie> PostMovie(Movie movie)
	{
		if (movie.Id != 0)
			return BadRequest("Id should not be provided");

		_context.Movies!.Add(movie);
		_context.SaveChanges();
		return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
	}

	[HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {
        var test = _context.Movies!.Find(id);
        if (test == null)
        {
            return NotFound();
        }

        _context.Remove(test);
        _context.SaveChanges();

        return NoContent();
    }
}
