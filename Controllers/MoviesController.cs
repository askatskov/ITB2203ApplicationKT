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
    public ActionResult<IEnumerable<Movie>> GetMovies(string? name = null)
    {
        var query = _context.Movies!.AsQueryable();

        if (name != null)
            query = query.Where(x => x.Title != null && x.Title.ToUpper().Contains(name.ToUpper()));

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
        var dbMovie = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == movie.Id);
        if (id != movie.Id || dbMovie == null)
        {
            return NotFound();
        }


        _context.Update(movie);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Movie> PostMovie(Movie movie)
    {
        var dbExercise = _context.Movies!.Find(movie.Id);
        if (dbExercise == null)
        {
            _context.Add(movie);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMovie), new { Id = movie.Id }, movie);
        }
        else
        {
            return Conflict();
        }
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
