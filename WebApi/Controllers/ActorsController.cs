﻿using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
             
            if (actor is null)
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

            if (actor is null)
            {
                return NotFound();
            }

            return actor;
        }

        // PUT: Actors/5
        [HttpPut("{id}")]
        public IActionResult PutActor(int id, Actor actor)
        {
            if (actor is null || id != actor.Id)
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
            if (actor is null || _service.ActorExists(actor.Id))
            {
                return BadRequest();
            }

            try
            {
                _service.AddActor(actor);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        // DELETE: Actors/5
        [HttpDelete("{id}")]
        public ActionResult<Actor> DeleteActor(int id)
        {
            var actor = _service.GetActor(id);

            if (actor is null)
            {
                return NotFound();
            }

            _service.DeleteActor(actor);

            return NoContent();
        }

    }
}
