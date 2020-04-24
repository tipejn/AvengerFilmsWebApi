using FilmsWebApi.Data;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

            using (var context = GetContext())
            {
                var helper = new ActorTestFilmContextHelper(context);
                helper.SeedContext();
                helper.ResetEntities();
            }
        }

        [Test]
        public void CanGetAllActorsWithoutFilms()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actors = service.GetAllActors();

                Assert.That(actors,
                    Has.Count.EqualTo(2)
                    .And.All.Property("Films").Empty);
            }
        }

        [Test]
        public void CanGetAllActorsWithFilms()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actors = service.GetAllActorsWithFilms();

                Assert.That(actors, Has.Some.Property("Films").Not.Empty);
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "ExistingActors")]
        public void CanGetSingleActorWithoutFilms(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);

                Assert.That(actor,
                    Is.Not.Null
                    .And.Property("Films").Empty
                    .And.Property("FirstName").EqualTo(source.FirstName)
                    .And.Property("LastName").EqualTo(source.LastName));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "ExistingActors")]
        public void CanGetSingleActorWithFilms(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
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
            using(var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(44);

                Assert.That(actor, Is.Null);
            }
        }

        [Test]
        public void CannotGetNonexistentActorWithFilms()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(44);

                Assert.That(actor, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CanAddActorWithoutAnyFilm(Actor source)
        {
            using(var context = GetContext())
            {
                var service = new ActorService(context);
                service.AddActor(source);
            }

            using (var context = GetContext())
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
        [TestCaseSource(typeof(ActorTestData), "NewActorWithExistingFilms")]
        public void CanAddActorToExistingFilm(Actor source)
        {
            using(var context = GetContext()) 
            {
                var service = new ActorService(context);
                service.AddActor(source);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(source.Id);
                Assert.That(actor.Films, new ContainsAllFilmTitlesConstraint(source.Films));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActorWithNonexistingFilms")]
        public void CannotAddActorToNonexistentFilm(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.AddActor(source), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "BrokenActors")]
        public void CannotAddActorWithExistingId(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.AddActor(source), Throws.InstanceOf<ArgumentException>());
            }

        }

        [Test]
        public void CannotAddNull()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.AddActor(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }
        [Test]
        [TestCaseSource(typeof(ActorTestData), "UpdatedActors")]
        public void CanUpdateActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                service.UpdateActor(source);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);
                Assert.That(actor, 
                    Has.Property("LastName").EqualTo(source.LastName)
                    .And.Property("FirstName").EqualTo(source.FirstName));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "ExistingActors")]
        public void CanRemoveFilmFromActor(Actor source)
        {
            var filmToDelete = source.Films.First();

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(source.Id);
                var actorfilm = actor.ActorFilms.Single(f => f.Film.Id == filmToDelete.Id);
                actor.ActorFilms.Remove(actorfilm);
                service.UpdateActor(actor);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(source.Id);
                Assert.That(actor.Films,
                    Has.Exactly(source.Films.Count - 1).Items
                    .And.Exactly(0).Matches<Film>(f => f.Id == filmToDelete.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CannotUpdateNonexistentActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.UpdateActor(source), Throws.InstanceOf<DbUpdateConcurrencyException>());
            }

        }
        [Test]
        public void CannotUpdateNull()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.UpdateActor(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "ExistingActors")]
        public void CanDeleteActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);
                service.DeleteActor(actor);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);
                Assert.That(actor, Is.Null);
            }
        }
        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CannotDeleteNonexistentActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.DeleteActor(source), Throws.InstanceOf<DbUpdateConcurrencyException>());
            }
        }
        [Test]
        public void CannotDeleteNull()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.DeleteActor(null), Throws.InstanceOf<ArgumentNullException>());
            }

        }
        [Test]
        [TestCaseSource(typeof(ActorTestData), "ExistingActors")]
        public void CanCheckThatActorExists(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(service.ActorExists(source.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CanCheckThatActorDoesNotExist(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(!service.ActorExists(source.Id));
            }
        }

        private FilmContext GetContext()
        {
            return new FilmContext(_options);
        }
    }
}
