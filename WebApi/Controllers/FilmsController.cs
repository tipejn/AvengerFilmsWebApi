using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FilmsWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly IFilmService _service;

        public FilmsController(IFilmService service)
        {
            _service = service;
        }

        // GET: Films
        [HttpGet]
        public IEnumerable<Film> GetFilms()
        {
            return _service.GetAllFilms();
        }

        // GET: Films/Actors
        [HttpGet]
        [Route("actors")]
        public IEnumerable<Film> GetFilmsWithActors()
        {
            return _service.GetAllFilmsWithActors();
        }

        // GET: Films/5
        [HttpGet("{id}")]
        public ActionResult<Film> GetFilm(int id)
        {
            var actor = _service.GetFilm(id);

            if (actor is null)
            {
                return NotFound();
            }

            return actor;
        }

        // GET: Films/5/Actors
        [HttpGet("{id}/actors")]
        public ActionResult<Film> GetFilmWithActors(int id)
        {
            var actor = _service.GetFilmWithActors(id);

            if (actor is null)
            {
                return NotFound();
            }

            return actor;
        }

        // PUT: Films/5
        [HttpPut("{id}")]
        public IActionResult PutActor(int id, Film film)
        {
            if (film is null || id != film.Id)
            {
                return BadRequest();
            }

            if (!_service.FilmExists(id))
            {
                return NotFound();
            }

            _service.UpdateFilm(film);

            return NoContent();
        }

        // POST: Films
        [HttpPost]
        public ActionResult<Film> PostFilm(Film film)
        {
            if (film is null || _service.FilmExists(film.Id))
            {
                return BadRequest();
            }

            try
            {
                _service.AddFilm(film);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetFilm", new { id = film.Id }, film);
        }

        // DELETE: Films/5
        [HttpDelete("{id}")]
        public ActionResult<Film> DeleteFilm(int id)
        {
            var film = _service.GetFilm(id);

            if (film is null)
            {
                return NotFound();
            }

            _service.DeleteFilm(film);

            return NoContent();
        }

    }
}
