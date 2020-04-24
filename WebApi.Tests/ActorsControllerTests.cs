using FilmsWebApi.Data;
using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace FilmsWebApi.Tests
{
    public class ActorControllerTests
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
                var controller = new ActorsController(service);
                var actors = controller.GetActors();

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
                var controller = new ActorsController(service);
                var actors = controller.GetActorsWithFilms();

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
                var controller = new ActorsController(service);
                var actor = controller.GetActor(source.Id);

                Assert.That(actor.Value,
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
                var controller = new ActorsController(service);
                var actor = controller.GetActorWithFilms(source.Id);

                Assert.That(actor.Value,
                    Is.Not.Null
                    .And.Property("Films").Not.Empty
                    .And.Property("FirstName").EqualTo(source.FirstName)
                    .And.Property("LastName").EqualTo(source.LastName));
            }
        }

        [Test]
        public void CannotGetNonexistentActorWithoutFilms()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.GetActor(44);

                Assert.That(actor.Value, Is.Null);
            }
        }

        [Test]
        public void CannotGetNonexistentActorWithFilms()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.GetActorWithFilms(44);

                Assert.That(actor.Value, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CanAddActorWithoutAnyFilm(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                controller.PostActor(source);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.GetActorWithFilms(source.Id);
                Assert.That(actor.Value,
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
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.PostActor(source);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.GetActorWithFilms(source.Id);
                Assert.That(actor.Value.Films, new ContainsAllFilmTitlesConstraint(source.Films));
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActorWithNonexistingFilms")]
        public void CannotAddActorToNonexistentFilm(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var response = controller.PostActor(source);

                Assert.That(response.Result, Is.TypeOf<BadRequestResult>());
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "BrokenActors")]
        public void CannotAddActorWithExistingId(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);

                var response = controller.PostActor(source);

                Assert.That(response.Result, Is.TypeOf<BadRequestResult>());
            }

        }

        [Test]
        public void CannotAddNull()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);

                var response = controller.PostActor(null);

                Assert.That(response.Result, Is.TypeOf<BadRequestResult>());
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "UpdatedActors")]
        public void CanUpdateActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                controller.PutActor(source.Id, source);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.GetActor(source.Id);
                Assert.That(actor.Value,
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
                var controller = new ActorsController(service);
                controller.PutActor(actor.Id, actor);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.GetActorWithFilms(source.Id);
                Assert.That(actor.Value.Films,
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
                var controller = new ActorsController(service);
                var response = controller.PutActor(source.Id, source);

                Assert.That(response, Is.TypeOf<NotFoundResult>());
            }
        }

        [Test]
        public void CannotUpdateNull()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var response = controller.PutActor(0, null);

                Assert.That(response, Is.TypeOf<BadRequestResult>());
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
                var controller = new ActorsController(service);
                controller.DeleteActor(actor.Id);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var actor = controller.GetActor(source.Id);
                Assert.That(actor.Value, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CannotDeleteNonexistentActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var controller = new ActorsController(service);
                var response = controller.DeleteActor(source.Id);

                Assert.That(response.Result, Is.TypeOf<NotFoundResult>());
            }
        }

        private FilmContext GetContext()
        {
            return new FilmContext(_options);
        }
    }
}
