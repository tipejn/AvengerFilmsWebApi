using FilmsWebApi.Data;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilmsWebApi.Tests
{
    public class ActorServiceTests
    {
        private DbContextOptions<FilmContext> _options;

        [SetUp]
        public void Setup()
        {
            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            _options = new DbContextOptionsBuilder<FilmContext>()
                .UseInMemoryDatabase("FilmDb")
                .UseInternalServiceProvider(provider)
                .Options;
        }

        [Test]
        public void CanGetAllActorsWithoutFilms()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            var actors = service.GetAllActors();

            Assert.That(actors, 
                Has.Count.EqualTo(2)
                .And.All.Property("Films").Empty);
        }

        [Test]
        public void CanGetAllActorsWithFilms()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            var actors = service.GetAllActorsWithFilms();

            Assert.That(actors, Has.Some.Property("Films").Not.Empty);
        }

        [Test]
        public void CanGetSingleActorWithoutFilms()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            var actor = service.GetActor(1);

            Assert.That(actor, 
                Is.Not.Null
                .And.Property("Films").Empty
                .And.Property("FirstName").EqualTo("Robert")
                .And.Property("LastName").EqualTo("Downey"));
        }

        [Test]
        public void CanGetSingleActorWithFilms()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            var actor = service.GetActorWithFilms(2);

            Assert.That(actor,
                Is.Not.Null
                .And.Property("Films").Not.Empty
                .And.Property("FirstName").EqualTo("Scarlett")
                .And.Property("LastName").EqualTo("Johanson"));
        }

        [Test]
        public void CannotGetNonexistentActorWithoutFilms()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            var actor = service.GetActor(44);

            Assert.That(actor, Is.Null);
        }

        [Test]
        public void CannotGetNonexistentActorWithFilms()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            var actor = service.GetActorWithFilms(44);

            Assert.That(actor, Is.Null);
        }

        [Test]
        public void CanAddActorWithoutAnyFilm()
        {
            var newActor = new Actor() { Id = 3, FirstName = "Chris", LastName = "Evans" };

            using (var context = GetMockFilmContext())
            {
                var service = new ActorService(context);
                service.AddActor(newActor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(newActor.Id);
                Assert.That(actor,
                    Is.Not.Null
                    .And.Property("FirstName").EqualTo(newActor.FirstName)
                    .And.Property("LastName").EqualTo(newActor.LastName));
            }
        }
        [Test]
        public void CanAddActorToExistingFilm()
        {
            var newActor = new Actor() { Id = 3, FirstName = "Chris", LastName = "Evans" };
            newActor.ActorFilms.Add(new ActorFilm() { FilmId = 2 });

            using(var context = GetMockFilmContext()) 
            {
                var service = new ActorService(context);
                service.AddActor(newActor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(newActor.Id);
                Assert.That(actor, Has.Property("Films").Exactly(1).Matches<Film>(f => f.Title == "Avengers"));
            }
        }

        [Test]
        public void CanAddActorWithNewFilm()
        {
            var newActor = new Actor() { Id = 3, FirstName = "Chris", LastName = "Evans" };
            var actorFilm = new ActorFilm()
            {
                Film = new Film() { Id = 3, Title = "Captain America", ReleaseDate = new DateTime(2011, 08, 05) }
            };
            newActor.ActorFilms.Add(actorFilm);

            using (var context = GetMockFilmContext())
            {
                var service = new ActorService(context);
                service.AddActor(newActor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(newActor.Id);
                Assert.That(actor, Has.Property("Films").Exactly(1).Matches<Film>(f => f.Title == actorFilm.Film.Title));
            }
        }

        [Test]
        public void CannotAddActorToNonexistentFilm()
        {
            var newActor = new Actor() { Id = 3, FirstName = "Chris", LastName = "Evans" };
            newActor.ActorFilms.Add(new ActorFilm() { FilmId = 55 });

            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(() => service.AddActor(newActor), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void CannotAddActorWithExistingId()
        {
            var newActor = new Actor() { Id = 2, FirstName = "Chris", LastName = "Evans" };

            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(() => service.AddActor(newActor), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void CannotAddNull()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(() => service.AddActor(null), Throws.InstanceOf<ArgumentNullException>());
        }
        [Test]
        public void CanUpdateActor()
        {
            var actorToUpdate = new Actor() { Id = 1, FirstName = "Robert", LastName = "Downey Junior" };

            using (var context = GetMockFilmContext())
            {
                var service = new ActorService(context);
                service.UpdateActor(actorToUpdate);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActor(actorToUpdate.Id);
                Assert.That(actor, Has.Property("LastName").EqualTo(actorToUpdate.LastName));
            }
        }

        [Test]
        public void CanAddFilmToActor()
        {
            var filmTitle = "Iron Man 2";
            var actorId = 1;

            using (var context = GetMockFilmContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(actorId);
                actor.ActorFilms.Add(new ActorFilm()
                {
                    Film = new Film() { Title = filmTitle, ReleaseDate = new DateTime(2010, 04, 30) }
                });
                service.UpdateActor(actor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(actorId);
                Assert.That(actor.Films,
                    Has.Exactly(3).Items
                    .And.Exactly(1).Matches<Film>(f => f.Title == filmTitle));
            }
        }

        [Test]
        public void CanRemoveFilmFromActor()
        {
            var actorId = 1;
            var filmId = 2;

            using (var context = GetMockFilmContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(actorId);
                var actorfilm = actor.ActorFilms.Single(f => f.Film.Id == filmId);
                actor.ActorFilms.Remove(actorfilm);
                service.UpdateActor(actor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(actorId);
                Assert.That(actor.Films,
                    Has.Exactly(1).Items
                    .And.Exactly(0).Matches<Film>(f => f.Id == filmId));
            }
        }

        [Test]
        public void CannotUpdateNonexistentActor()
        {
            var actor = new Actor() { Id = 44, FirstName = "John", LastName = "Smith" };
            var context = GetMockFilmContext();
            var service = new ActorService(context);
            
            Assert.That(() => service.UpdateActor(actor), Throws.InstanceOf<DbUpdateConcurrencyException>());
        }
        [Test]
        public void CannotUpdateNull()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(() => service.UpdateActor(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void CanDeleteActor()
        {
            var actorId = 1;
            using (var context = GetMockFilmContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(actorId);
                service.DeleteActor(actor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActor(actorId);
                Assert.That(actor, Is.Null);
            }
        }
        [Test]
        public void CannotDeleteNonexistentActor()
        {
            var actor = new Actor() { Id = 44, FirstName = "John", LastName = "Smith" };
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(() => service.DeleteActor(actor), Throws.InstanceOf<DbUpdateConcurrencyException>());
        }
        [Test]
        public void CannotDeleteNull()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(() => service.DeleteActor(null), Throws.InstanceOf<ArgumentNullException>());
        }
        [Test]
        public void CanCheckThatActorExists()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(service.ActorExists(1));
        }

        [Test]
        public void CanCheckThatActorDoesNotExist()
        {
            var context = GetMockFilmContext();
            var service = new ActorService(context);

            Assert.That(!service.ActorExists(0));
        }

        private FilmContext GetMockFilmContext()
        {
            var context = new FilmContext(_options);
            Seed(context);
            ResetEntities(context);
            return context;

        }

        private void Seed(FilmContext context)
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
        private void ResetEntities(FilmContext context)
        {
            var entries = context.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
