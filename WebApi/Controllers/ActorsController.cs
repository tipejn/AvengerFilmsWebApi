using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FilmsWebApi.Data;
using FilmsWebApi.Model;
using FilmsWebApi.Service;

namespace FilmsWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _service;

        public ActorsController(IActorService service)
        {
            _service = service;
        }

        // GET: Actors
        [HttpGet]
        public IEnumerable<Actor> GetActors()
        {
            return _service.GetAllActors();
        }

        // GET: Actors/Films
        [HttpGet]
        [Route("films")]
        public IEnumerable<Actor> GetActorsWithFilms()
        {
            return _service.GetAllActorsWithFilms();
        }

        // GET: api/Actors/5
        [HttpGet("{id}")]
        public ActionResult<Actor> GetActor(int id)
        {
            var actor = _service.GetActor(id);
             
            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        // GET: Actors/5/Films
        [HttpGet("{id}/films")]
        public ActionResult<Actor> GetActorWithFilms(int id)
        {
            var actor = _service.GetActorWithFilms(id);

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        // PUT: Actors/5
        [HttpPut("{id}")]
        public IActionResult PutActor(int id, Actor actor)
        {
            if (id != actor.Id)
            {
                return BadRequest();
            }

            if (!_service.ActorExists(id))
            {
                return NotFound();
            }

            _service.UpdateActor(actor);

            return NoContent();
        }

        // POST: Actors
        [HttpPost]
        public ActionResult<Actor> PostActor(Actor actor)
        {
            if (_service.ActorExists(actor.Id))
            {
                return BadRequest();
            }

            _service.AddActor(actor);

            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        // DELETE: Actors/5
        [HttpDelete("{id}")]
        public ActionResult<Actor> DeleteActor(int id)
        {
            var actor = _service.GetActor(id);

            if (actor == null)
            {
                return NotFound();
            }

            _service.DeleteActor(actor);

            return actor;
        }

    }
}
