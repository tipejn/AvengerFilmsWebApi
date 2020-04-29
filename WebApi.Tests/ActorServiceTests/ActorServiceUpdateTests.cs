using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FilmsWebApi.Tests
{
    public class ActorServiceUpdateTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "UpdatedActors")]
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
        [TestCaseSource(typeof(TestData), "ExistingActors")]
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
        [TestCaseSource(typeof(TestData), "NewActors")]
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
    }
}
