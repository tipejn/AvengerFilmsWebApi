using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi;
using WebApi.Data;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly FilmContext _context;

        public FilmsController(FilmContext context)
        {
            _context = context;
        }

        // GET: Films
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms()
        {
            return await _context.Films
                .ToListAsync();
        }

        // GET: Films
        [HttpGet]
        [Route("actors")]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilmsWithActors()
        {
            return await _context.Films
                .Include(f => f.ActorFilms)
                    .ThenInclude(f => f.Actor)
                .ToListAsync();
        }

        // GET: Films/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Film>> GetFilm(int id)
        {
            var film = await _context.Films.FindAsync(id);

            if (film == null)
            {
                return NotFound();
            }

            return film;
        }

        // GET: Films/5/actors
        [HttpGet("{id}/actors")]
        public async Task<ActionResult<Film>> GetFilmWithActors(int id)
        {
            var film = await _context.Films
                .Include(a => a.ActorFilms)
                    .ThenInclude(a => a.Actor)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (film == null)
            {
                return NotFound();
            }

            return film;
        }

        // PUT: Films/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilm(int id, Film film)
        {
            if (id != film.Id)
            {
                return BadRequest();
            }

            EnsureHasActors(film);

            _context.Entry(film).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Films
        [HttpPost]
        public async Task<ActionResult<Film>> PostFilm(Film film)
        {
            EnsureHasActors(film);

            _context.Films.Add(film);
            
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFilm", new { id = film.Id }, film);
        }

        // DELETE: Films/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Film>> DeleteFilm(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            _context.Films.Remove(film);
            await _context.SaveChangesAsync();

            return film;
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }

        private void EnsureHasActors(Film film)
        {
            foreach (var actorFilm in film.ActorFilms)
            {
                if (actorFilm.ActorId != 0)
                {
                    actorFilm.Actor = _context.Actors.FirstOrDefault(a => a.Id == actorFilm.ActorId);
                }
            }
        }
    }
}
