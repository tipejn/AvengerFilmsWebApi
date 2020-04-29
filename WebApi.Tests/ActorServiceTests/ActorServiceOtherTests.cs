using FilmsWebApi.Model;
using FilmsWebApi.Service;
using NUnit.Framework;

namespace FilmsWebApi.Tests
{
    public class ActorServiceOtherTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "ExistingActors")]
        public void CanCheckThatActorExists(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(service.ActorExists(source.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewActors")]
        public void CanCheckThatActorDoesNotExist(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(!service.ActorExists(source.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CanCheckIfActorExistsWhenPassingExtremeValues(int id)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(!service.ActorExists(id));
            }
        }
    }
}
