using FilmsWebApi.Data;
using FilmsWebApi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace FilmsWebApi.Tests
{
    class TestFilmContextHelper
    {
        private FilmContext _context;
        public TestFilmContextHelper(FilmContext context)
        {
            _context = context;
        }
        public void SeedContext()
        {
            var actor1 = new Actor { Id = 1, FirstName = "Robert", LastName = "Downey" };
            var actor2 = new Actor { Id = 2, FirstName = "Scarlett", LastName = "Johanson" };

            var film1 = new Film { Id = 1, Title = "Iron Man", ReleaseDate = new DateTime(2008, 4, 30) };
            var film2 = new Film { Id = 2, Title = "Avengers", ReleaseDate = new DateTime(2012, 4, 11) };

            actor1.ActorFilms.Add(new ActorFilm { Film = film1 });
            actor1.ActorFilms.Add(new ActorFilm { Film = film2 });
            actor2.ActorFilms.Add(new ActorFilm { Film = film2 });

            var actors = new List<Actor> { actor1, actor2 };

            _context.Actors.AddRange(actors);
            _context.SaveChanges();
        }
        public void ResetEntities()
        {
            var entries = _context.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
