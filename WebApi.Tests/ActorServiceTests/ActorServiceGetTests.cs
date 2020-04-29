using FilmsWebApi.Model;
using FilmsWebApi.Service;
using NUnit.Framework;

namespace FilmsWebApi.Tests
{
    public class ActorServiceGetTests : TestsSetup
    {
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
        [TestCaseSource(typeof(TestData), "ExistingActors")]
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
        [TestCaseSource(typeof(TestData), "ExistingActors")]
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
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetActorWhenPassingExtremeValues(int id)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(id);

                Assert.That(actor, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetActorWithFilmsWhenPassingExtremeValues(int id)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActorWithFilms(id);

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
    }
}
