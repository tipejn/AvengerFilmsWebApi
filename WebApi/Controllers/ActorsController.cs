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
    public class ActorsController : ControllerBase
    {
        private readonly FilmContext _context;

        public ActorsController(FilmContext context)
        {
            _context = context;
        }

        // GET: Actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors()
        {
            return await _context.Actors                
                .ToListAsync();
        }

        // GET: Actors/Films
        [HttpGet]
        [Route("films")]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActorsWithFilms()
        {
            return await _context.Actors
                .Include(f => f.ActorFilms)
                    .ThenInclude(a => a.Film)
                .ToListAsync();
        }

        // GET: api/Actors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(int id)
        {
            var actor = await _context.Actors
                .FirstOrDefaultAsync(a => a.Id == id);
             

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        // GET: Actors/5/Films
        [HttpGet("{id}/Films")]
        public async Task<ActionResult<Actor>> GetActorWithFilms(int id)
        {
            var actor = await _context.Actors
                .Include(f => f.ActorFilms)
                    .ThenInclude(f => f.Film)
                .FirstOrDefaultAsync(a => a.Id == id);


            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        // PUT: Actors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActor(int id, Actor actor)
        {
            if (id != actor.Id)
            {
                return BadRequest();
            }

            _context.Entry(actor).State = EntityState.Modified;

            EnsureHasFilms(actor);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
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

        // POST: Actors
        [HttpPost]
        public async Task<ActionResult<Actor>> PostActor(Actor actor)
        {
            EnsureHasFilms(actor);

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        // DELETE: Actors/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Actor>> DeleteActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            return actor;
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }

        private void EnsureHasFilms(Actor actor)
        {
            foreach (var actorFilm in actor.ActorFilms)
            {
                if (actorFilm.FilmId != 0)
                {
                    actorFilm.Film = _context.Films.FirstOrDefault(f => f.Id == actorFilm.FilmId);
                }
            }
        }
    }
}
