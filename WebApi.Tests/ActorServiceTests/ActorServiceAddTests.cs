using FilmsWebApi.Model;
using FilmsWebApi.Service;
using NUnit.Framework;
using System;

namespace FilmsWebApi.Tests
{
    public class ActorServiceAddTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "NewActors")]
        public void CanAddActorWithoutAnyFilm(Actor source)
        {
            using (var context = GetContext())
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
        [TestCaseSource(typeof(TestData), "NewActorWithExistingFilms")]
        public void CanAddActorToExistingFilm(Actor source)
        {
            using (var context = GetContext())
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
        [TestCaseSource(typeof(TestData), "NewActorWithNonexistingFilms")]
        public void CannotAddActorToNonexistentFilm(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.AddActor(source), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "BrokenActors")]
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
    }
}
