using FilmsWebApi.Data;
using FilmsWebApi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmsWebApi.Service
{
    public class ActorService : IActorService
    {
        private FilmContext _context;
        public ActorService(FilmContext context)
        {
            _context = context;
        }

        public void AddActor(Actor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException();
            }

            EnsureHasFilms(actor);
            _context.Actors.Add(actor);
            _context.SaveChanges();
        }

        public void DeleteActor(Actor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException();
            }
            _context.Actors.Remove(actor);
            _context.SaveChanges();
        }

        public Actor GetActor(int id)
        {
            return _context.Actors.Find(id);
        }

        public Actor GetActorWithFilms(int id)
        {
            return _context.Actors
                .Include(f => f.ActorFilms)
                    .ThenInclude(a => a.Film)
                .FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Actor> GetAllActors()
        {
            return _context.Actors.ToList();
        }

        public IEnumerable<Actor> GetAllActorsWithFilms()
        {
            return _context.Actors
                .Include(f => f.ActorFilms)
                    .ThenInclude(a => a.Film)
                .ToList();
        }

        public void UpdateActor(Actor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException();
            }

            EnsureHasFilms(actor);
            _context.Entry(actor).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
        private void EnsureHasFilms(Actor actor)
        {
            foreach (var actorFilm in actor.ActorFilms)
            {
                if (actorFilm.FilmId != 0)
                {
                    actorFilm.Film = _context.Films.Single(f => f.Id == actorFilm.FilmId);
                }
            }
        }
    }
}
