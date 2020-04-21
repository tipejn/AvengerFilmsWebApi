using NUnit.Framework;
using FilmsWebApi.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FilmsWebApi.Controllers;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FilmsWebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace FilmsWebApi.Tests
{
    public class Tests
    {
        [Test]
        public void CanGetActors()
        {
            var context = GetFilmContextMock();

            var target = new ActorsController(context);
            var actors = target.GetActors();

            Assert.That(actors, Has.Exactly(2).Items);
        }
        [Test]
        public void CanGetActorsWithFilms()
        {
            var context = GetFilmContextMock();

            var target = new ActorsController(context);
            var actors = target.GetActorsWithFilms();

            Assert.That(actors, Has.All.Property("Films").Count.GreaterThan(0));
        }

        [Test]
        public void CanGetSingleActorWithoutFilms()
        {
            var context = GetFilmContextMock();

            var target = new ActorsController(context);
            var actor = target.GetActor(1);

            Assert.That(actor.Value, 
                Has.Property("FirstName").EqualTo("Robert")
                .And
                .Property("Films").Empty);
        }

        [Test]
        public void CanGetSingleActorWithFilms()
        {
            var context = GetFilmContextMock();

            var target = new ActorsController(context);
            var actor = target.GetActorWithFilms(2);

            Assert.That(actor.Value, 
                Has.Property("LastName").EqualTo("Johanson")
                .And
                .Property("Films").Count.EqualTo(1));
        }

        [Test]
        public void CanAddActor()
        {
            var actor = new Actor() { FirstName = "Chris", LastName = "Evans" };

            var context = GetFilmContextMock();
            var target = new ActorsController(context);

            target.PostActor(actor);
            var actors = context.Actors.ToList();

            Assert.That(actors, Has.Exactly(1).Matches<Actor>(
                a => a.FirstName == actor.FirstName && a.LastName == actor.LastName));
        }

        [Test]
        public void CanAddActorWithNewFilm()
        {
            var actor = new Actor() { FirstName = "Chris", LastName = "Evans" };
            actor.ActorFilms.Add(new ActorFilm()
            {
                Actor = actor,
                Film = new Film()
                {
                    Title = "Captain America",
                    ReleaseDate = new DateTime(2011, 08, 05)
                }
            });

            var context = GetFilmContextMock();
            var target = new ActorsController(context);

            target.PostActor(actor);
            var resultActor = context.Actors
                .FirstOrDefault(a => a.FirstName == actor.FirstName && a.LastName == actor.LastName);

            Assert.That(resultActor, 
                Is.Not.Null
                .And
                .Property("Films").Count.EqualTo(1));
        }

        [Test]
        public void CanAddActorToExistingFilm()
        {
            var actor = new Actor() { FirstName = "Chris", LastName = "Evans" };
            var filmId = 2;
            actor.ActorFilms.Add(new ActorFilm { FilmId = filmId });

            var context = GetFilmContextMock();
            var target = new ActorsController(context);

            target.PostActor(actor);
            var resultActor = context.Actors
                .FirstOrDefault(a => a.FirstName == actor.FirstName && a.LastName == actor.LastName);

            Assert.That(resultActor, 
                Is.Not.Null
                .And
                .Property("Films").Exactly(1).Matches<Film>(
                    f => f.Id == filmId && f.Title == "Avengers"));
        }

        [Test]
        public void CanUpdateActor()
        {
            var id = 1;
            var newLastName = "Downey Junior";
            var actor = new Actor() { Id = id, FirstName = "Robert", LastName = newLastName};

            var context = GetFilmContextMock();
            var target = new ActorsController(context);

            target.PutActor(id, actor);
            var resultActor = context.Actors
                .FirstOrDefault(a => a.Id == id);

            Assert.That(resultActor, Has.Property("LastName").EqualTo(newLastName));
        }

        [Test]
        public void CanDeleteActor()
        {
            var id = 1;
            var context = GetFilmContextMock();
            var target = new ActorsController(context);
            
            target.DeleteActor(id);

            var actors = context.Actors.ToList();

            Assert.That(actors, Has.Exactly(0).Matches<Actor>(a => a.Id == id));
        }

        [Test]
        public void CannotFindNonexistentActor()
        {
            var context = GetFilmContextMock();
            var target = new ActorsController(context);
            var result = target.GetActor(44);

            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }

        private FilmContext GetFilmContextMock()
        {
            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<FilmContext>()
                .UseInMemoryDatabase("FilmDb")
                .UseInternalServiceProvider(provider)
                .Options;

            var context = new FilmContext(options);

            PopulateContext(context);
            ResetEntities(context);

            return context;
        }

        private void PopulateContext(FilmContext context)
        {
            var actor1 = new Actor { Id = 1, FirstName = "Robert", LastName = "Downey" };
            var actor2 = new Actor { Id = 2, FirstName = "Scarlett", LastName = "Johanson" };

            var film1 = new Film { Id = 1, Title = "Iron Man", ReleaseDate = new DateTime(2008, 4, 30) };
            var film2 = new Film { Id = 2, Title = "Avengers", ReleaseDate = new DateTime(2012, 4, 11) };

            film1.ActorFilms.Add(new ActorFilm { Actor = actor1, Film = film1 });
            film2.ActorFilms.Add(new ActorFilm { Actor = actor1, Film = film2 });
            film2.ActorFilms.Add(new ActorFilm { Actor = actor2, Film = film2 });

            context.Films.AddRange(film1, film2);
            context.SaveChanges();
        }
         private void ResetEntities(DbContext context)
        {
            var entries = context.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }

    }
}