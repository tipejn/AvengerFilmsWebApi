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
        private FilmContext _context;

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

            _context = new FilmContext(_options);

            Seed(_context);
            ResetEntities(_context);
        }

        [Test]
        public void CanGetAllActorsWithoutFilms()
        {
            using (_context)
            {
                var service = new ActorService(_context);
                var actors = service.GetAllActors();

                Assert.That(actors,
                    Has.Count.EqualTo(2)
                    .And.All.Property("Films").Empty);
            }
        }

        [Test]
        public void CanGetAllActorsWithFilms()
        {
            using (_context)
            {
                var service = new ActorService(_context);
                var actors = service.GetAllActorsWithFilms();

                Assert.That(actors, Has.Some.Property("Films").Not.Empty);
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "ExistingActors")]
        public void CanGetSingleActorWithoutFilms(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);
                var actor = service.GetActor(source.Id);

                Assert.That(actor,
                    Is.Not.Null
                    .And.Property("Films").Empty
                    .And.Property("FirstName").EqualTo(source.FirstName)
                    .And.Property("LastName").EqualTo(source.LastName));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "ExistingActors")]
        public void CanGetSingleActorWithFilms(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);
                var actor = service.GetActorWithFilms(source.Id);

                Assert.That(actor,
                    Is.Not.Null
                    .And.Property("Films").Not.Empty
                    .And.Property("FirstName").EqualTo(source.FirstName)
                    .And.Property("LastName").EqualTo(source.LastName));
            }
        }

        [Test]
        public void CannotGetNonexistentActorWithoutFilms()
        {
            using(_context)
            {
                var service = new ActorService(_context);
                var actor = service.GetActor(44);

                Assert.That(actor, Is.Null);
            }
        }

        [Test]
        public void CannotGetNonexistentActorWithFilms()
        {
            using (_context)
            {
                var service = new ActorService(_context);
                var actor = service.GetActorWithFilms(44);

                Assert.That(actor, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "NewActors")]
        public void CanAddActorWithoutAnyFilm(Actor source)
        {
            using(_context)
            {
                var service = new ActorService(_context);
                service.AddActor(source);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(source.Id);
                Assert.That(actor,
                    Is.Not.Null
                    .And.Property("FirstName").EqualTo(source.FirstName)
                    .And.Property("LastName").EqualTo(source.LastName)
                    .And.Property("Films").Empty);
            }
        }
        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "NewActorWithExistingFilms")]
        public void CanAddActorToExistingFilm(Actor source)
        {
            using(_context) 
            {
                var service = new ActorService(_context);
                service.AddActor(source);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(source.Id);
                Assert.That(actor.Films, new ContainsAllFilmTitlesConstraint(source.Films));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "NewActorWithNonexistingFilms")]
        public void CannotAddActorToNonexistentFilm(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(() => service.AddActor(source), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "BrokenActors")]
        public void CannotAddActorWithExistingId(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(() => service.AddActor(source), Throws.InstanceOf<ArgumentException>());
            }

        }

        [Test]
        public void CannotAddNull()
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(() => service.AddActor(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }
        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "UpdatedActors")]
        public void CanUpdateActor(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);
                service.UpdateActor(source);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);
                Assert.That(actor, 
                    Has.Property("LastName").EqualTo(source.LastName)
                    .And.Property("FirstName").EqualTo(source.FirstName));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "ExistingActors")]
        public void CanRemoveFilmFromActor(Actor source)
        {
            var filmToDelete = source.Films.First();

            using (_context)
            {
                var service = new ActorService(_context);
                var actor = service.GetActorWithFilms(source.Id);
                var actorfilm = actor.ActorFilms.Single(f => f.Film.Id == filmToDelete.Id);
                actor.ActorFilms.Remove(actorfilm);
                service.UpdateActor(actor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(source.Id);
                Assert.That(actor.Films,
                    Has.Exactly(source.Films.Count - 1).Items
                    .And.Exactly(0).Matches<Film>(f => f.Id == filmToDelete.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "NewActors")]
        public void CannotUpdateNonexistentActor(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(() => service.UpdateActor(source), Throws.InstanceOf<DbUpdateConcurrencyException>());
            }

        }
        [Test]
        public void CannotUpdateNull()
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(() => service.UpdateActor(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "ExistingActors")]
        public void CanDeleteActor(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);
                var actor = service.GetActor(source.Id);
                service.DeleteActor(actor);
            }

            using (var context = new FilmContext(_options))
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);
                Assert.That(actor, Is.Null);
            }
        }
        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "NewActors")]
        public void CannotDeleteNonexistentActor(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(() => service.DeleteActor(source), Throws.InstanceOf<DbUpdateConcurrencyException>());
            }
        }
        [Test]
        public void CannotDeleteNull()
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(() => service.DeleteActor(null), Throws.InstanceOf<ArgumentNullException>());
            }

        }
        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "ExistingActors")]
        public void CanCheckThatActorExists(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(service.ActorExists(source.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorServiceTestData), "NewActors")]
        public void CanCheckThatActorDoesNotExist(Actor source)
        {
            using (_context)
            {
                var service = new ActorService(_context);

                Assert.That(!service.ActorExists(source.Id));
            }
        }

        private void Seed(FilmContext context)
        {
            var actor1 = new Actor { Id = 1, FirstName = "Robert", LastName = "Downey" };
            var actor2 = new Actor { Id = 2, FirstName = "Scarlett", LastName = "Johanson" };

            var film1 = new Film { Id = 1, Title = "Iron Man", ReleaseDate = new DateTime(2008, 4, 30) };
            var film2 = new Film { Id = 2, Title = "Avengers", ReleaseDate = new DateTime(2012, 4, 11) };

            actor1.ActorFilms.Add(new ActorFilm { Film = film1 });
            actor1.ActorFilms.Add(new ActorFilm { Film = film2 });
            actor2.ActorFilms.Add(new ActorFilm { Film = film2 });

            var actors =  new List<Actor> { actor1, actor2 };

            context.Actors.AddRange(actors);
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
